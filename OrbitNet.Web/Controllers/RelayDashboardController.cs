using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrbitNet.Web.Configuration;
using OrbitNet.Web.Models.Entities;
using OrbitNet.Web.Models.Enums;
using OrbitNet.Web.Models.ViewModels;
using OrbitNet.Web.Services;
using OrbitNet.Web.Services.Communication;

namespace OrbitNet.Web.Controllers;

[Route("Relay")]
public class RelayDashboardController : Controller
{
    private readonly AppInstanceSettings _settings;
    private readonly RelayHttpService _relayHttpService;
    private readonly BasicAuthService _basicAuthService;
    private readonly OrbitNetStore _store;
    private readonly ILogger<RelayDashboardController> _logger;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public RelayDashboardController(
        IOptions<AppInstanceSettings> settings,
        RelayHttpService relayHttpService,
        BasicAuthService basicAuthService,
        OrbitNetStore store,
        ILogger<RelayDashboardController> logger,
        IStringLocalizer<SharedResource> localizer)
    {
        _settings = settings.Value;
        _relayHttpService = relayHttpService;
        _basicAuthService = basicAuthService;
        _store = store;
        _logger = logger;
        _localizer = localizer;
    }

    [HttpGet]
    public IActionResult Index()
    {
        EnsureStoreInitialized();
        var model = BuildDashboardModel();
        return View("~/Views/Relay/Index.cshtml", model);
    }

    [HttpGet("refresh")]
    public IActionResult Refresh()
    {
        EnsureStoreInitialized();
        var model = BuildDashboardModel();

        return Ok(new
        {
            status = "success",
            data = new
            {
                status = model.Status,
                routes = model.Routes,
                buffers = model.Buffers,
                recentEvents = model.RecentEvents,
                lastUpdated = model.LastUpdated
            }
        });
    }

    [HttpPost("forcesend")]
    public async Task<IActionResult> ForceSend([FromBody] ForceSendRequest? request)
    {
        if (request == null)
        {
            return BadRequest(new { status = "error", message = "No se recibió una solicitud válida." });
        }

        try
        {
            var packet = new MessagePacket
            {
                CodHex = $"PKT-{DateTime.UtcNow:yyyyMMddHHmmss}",
                SenderId = request.FromSatellite,
                DestinationIp = request.ToAntenna,
                Priority = (PriorityLevel)request.Priority,
                Content = request.PacketData
            };

            var sent = await _relayHttpService.EnviarPaqueteAlHemisferioHermanoAsync(packet);

            if (!sent)
            {
                return StatusCode(502, new { status = "error", message = "No se pudo enviar el paquete al hemisferio hermano." });
            }

            var route = _store.Routes.Find(x =>
                string.Equals(x.FromSatellite, request.FromSatellite, StringComparison.OrdinalIgnoreCase)
                && string.Equals(x.ToAntenna, request.ToAntenna, StringComparison.OrdinalIgnoreCase));

            if (route != null)
            {
                route.PacketCount += 1;
                route.Status = "Resent";
                route.LastSeen = DateTime.Now;
                route.PacketData = $"{route.PacketCount} paquetes reenviados";
                route.QueueOccupancyPercentage = Math.Min(100, route.QueueOccupancyPercentage + 5);
            }

            return StatusCode(201, new
            {
                status = "success",
                message = $"Paquete enviado desde {request.FromSatellite} hacia {request.ToAntenna}.",
                packet
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al forzar el envío de un paquete relay");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpPost("clearbuffer")]
    public IActionResult ClearBuffer([FromQuery] string? bufferId)
    {
        if (string.IsNullOrWhiteSpace(bufferId))
        {
            return BadRequest(new { status = "error", message = "Se requiere un identificador de buffer." });
        }

        try
        {
            EnsureStoreInitialized();

            var buffer = _store.Buffers.Find(x => string.Equals(x.BufferId, bufferId, StringComparison.OrdinalIgnoreCase));
            if (buffer == null)
            {
                return NotFound(new { status = "error", message = $"Buffer {bufferId} no encontrado." });
            }

            buffer.ItemsInQueue = 0;
            buffer.CapacityPercentage = 0;
            _store.EventsProcessed += 1;

            double totalCap = 0;
            int bufCount = 0;
            _store.Buffers.ForEach(b => { totalCap += b.CapacityPercentage; bufCount++; });
            _store.QueueOccupancyPercentage = bufCount > 0 ? totalCap / bufCount : 0.0;

            return Ok(new
            {
                status = "success",
                message = $"Buffer {bufferId} limpiado correctamente.",
                bufferId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al limpiar el buffer {BufferId}", bufferId);
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpGet("testconnectivity")]
    public async Task<IActionResult> TestConnectivity()
    {
        if (!_settings.EnableCrossHemisphereRelay)
        {
            return Ok(new
            {
                status = "success",
                connected = false,
                message = "La conectividad relay está deshabilitada por configuración."
            });
        }

        try
        {
            var targetPort = _settings.Hemisphere.Equals("North", StringComparison.OrdinalIgnoreCase) ? 5001 : 5000;
            var targetHemisphere = _settings.Hemisphere.Equals("North", StringComparison.OrdinalIgnoreCase) ? "Sur" : "Norte";
            var targetUrl = $"http://127.0.0.1:{targetPort}/Relay/Refresh";

            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
            using var response = await client.GetAsync(targetUrl);

            if (response.IsSuccessStatusCode)
            {
                return Ok(new
                {
                    status = "success",
                    connected = true,
                    message = $"Conexión disponible con el puerto del hemisferio {targetHemisphere} ({targetPort})."
                });
            }

            return Ok(new
            {
                status = "success",
                connected = false,
                message = $"No fue posible conectar con el puerto del hemisferio {targetHemisphere} ({targetPort})."
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudo validar la conectividad relay");
            return Ok(new
            {
                status = "success",
                connected = false,
                message = "No fue posible completar la prueba de conectividad."
            });
        }
    }

    [HttpGet("ExportBuffersCsv")]
    [HttpGet("exportbuffercsv")]
    public IActionResult ExportBuffersCsv([FromQuery] string? bufferId)
    {
        EnsureStoreInitialized();

        var filteredBuffers = new List<BufferDto>();
        if (string.IsNullOrWhiteSpace(bufferId))
        {
            _store.Buffers.ForEach(b => filteredBuffers.Add(b));
        }
        else
        {
            _store.Buffers.ForEach(b =>
            {
                if (b.BufferId.Equals(bufferId, StringComparison.OrdinalIgnoreCase))
                    filteredBuffers.Add(b);
            });
        }

        var csvLines = new List<string>
        {
            "BufferId,Type,ItemsInQueue,CapacityPercentage"
        };

        filteredBuffers.ForEach(buffer =>
            csvLines.Add($"{buffer.BufferId},{buffer.Type},{buffer.ItemsInQueue},{buffer.CapacityPercentage}"));

        var csvBytes = System.Text.Encoding.UTF8.GetBytes(string.Join("\n", csvLines));
        var safeBufferId = string.IsNullOrWhiteSpace(bufferId) ? "all-buffers" : bufferId.Replace(" ", "-").Replace("/", "-");
        var fileName = string.IsNullOrWhiteSpace(bufferId)
            ? "relay-buffers.csv"
            : $"relay-{safeBufferId}.csv";

        return File(csvBytes, "text/csv", fileName);
    }

    private RelayDashboardViewModel BuildDashboardModel()
    {
        EnsureStoreInitialized();

        var routes = new List<RouteDto>();
        var buffers = new List<BufferDto>();
        double totalCapPct = 0;

        _store.Satellites.ForEach(s =>
        {
            routes.Add(new RouteDto
            {
                FromSatellite = s.Id,
                ToAntenna = "ANT-" + s.Id,
                Status = "active",
                PacketCount = 0,
                QueueOccupancyPercentage = 0,
                LastSeen = DateTime.Now,
                PacketData = ""
            });
            int occupied = s.PaquetesABordo?.Count ?? 0;
            int capacity = 100;
            double percent = capacity > 0 ? (double)occupied / capacity * 100 : 0;
            buffers.Add(new BufferDto
            {
                BufferId = $"BUF-{s.Id}",
                Type = percent >= 80 ? "critico" : percent >= 50 ? "normal" : "bajo",
                ItemsInQueue = occupied,
                CapacityPercentage = percent
            });
            totalCapPct += percent;
        });

        _store.QueueOccupancyPercentage = buffers.Count > 0 ? totalCapPct / buffers.Count : 0.0;

        var model = new RelayDashboardViewModel
        {
            Hemisphere = _settings.Hemisphere,
            LastUpdated = DateTime.Now,
            Status = BuildStatusFromStore(),
            Routes = routes,
            Buffers = buffers,
            RecentEvents = new List<EventDto>
            {
                new() { Timestamp = DateTime.Now.AddMinutes(-2), Level = "Info", DisplayLevel = _localizer["Information"].Value, Message = _localizer["RelayEventProcessed"].Value },
                new() { Timestamp = DateTime.Now.AddMinutes(-5), Level = "Warning", DisplayLevel = _localizer["Warning"].Value, Message = _localizer["RelayEventCongestion"].Value },
                new() { Timestamp = DateTime.Now.AddMinutes(-8), Level = "Info", DisplayLevel = _localizer["Information"].Value, Message = _localizer["RelayEventConnection"].Value }
            },
            ActionMessage = "Datos cargados desde el store."
        };

        return model;
    }

    private RelayStatusDto BuildStatusFromStore()
    {
        var activeRelays = 0;
        var inactiveRelays = 0;
        _store.Routes.ForEach(r =>
        {
            if (r.Status.Contains("active", StringComparison.OrdinalIgnoreCase) || r.Status.Contains("activa", StringComparison.OrdinalIgnoreCase))
                activeRelays++;
            else if (r.Status.Contains("inactive", StringComparison.OrdinalIgnoreCase) || r.Status.Contains("inactiva", StringComparison.OrdinalIgnoreCase))
                inactiveRelays++;
        });

        var totalPackets = 0;
        _store.Routes.ForEach(r => totalPackets += r.PacketCount);

        double northOccupancy = 0;
        double southOccupancy = 0;
        int northCount = 0;
        int southCount = 0;

        _store.Routes.ForEach(r =>
        {
            if (r.FromSatellite.StartsWith("SAT-") || r.ToAntenna.Contains("N", StringComparison.OrdinalIgnoreCase))
            {
                northOccupancy += r.QueueOccupancyPercentage;
                northCount++;
            }
            else
            {
                southOccupancy += r.QueueOccupancyPercentage;
                southCount++;
            }
        });

        return new RelayStatusDto
        {
            ActiveRelays = activeRelays,
            InactiveRelays = Math.Max(0, inactiveRelays > 0 ? inactiveRelays : _store.Routes.Count - activeRelays),
            TotalPacketsProcessed = totalPackets,
            AvgQueueOccupancy = _store.QueueOccupancyPercentage,
            NorthQueueOccupancy = northCount > 0 ? northOccupancy / northCount : _store.QueueOccupancyPercentage,
            SouthQueueOccupancy = southCount > 0 ? southOccupancy / southCount : Math.Max(0, _store.QueueOccupancyPercentage - 10)
        };
    }

    private void EnsureStoreInitialized()
    {
        if (_store.Routes.Count > 0 && _store.Buffers.Count > 0)
        {
            return;
        }

        // Si el store no tiene datos, se usan los que tenga de la simulación o se deja vacío
        double totalCapPct = 0;
        int bufCount = 0;
        _store.Buffers.ForEach(b => { totalCapPct += b.CapacityPercentage; bufCount++; });
        _store.QueueOccupancyPercentage = bufCount > 0 ? totalCapPct / bufCount : 0.0;
    }
}

public class ForceSendRequest
{
    public string FromSatellite { get; set; } = string.Empty;
    public string ToAntenna { get; set; } = string.Empty;
    public string PacketData { get; set; } = string.Empty;
    public int Priority { get; set; } = 1;
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrbitNet.Web.Configuration;
using OrbitNet.Web.Models.ViewModels;
using OrbitNet.Web.Services;

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
                Priority = request.Priority,
                Content = request.PacketData
            };

            var sent = await _relayHttpService.EnviarPaqueteAlHemisferioHermanoAsync(packet);

            if (!sent)
            {
                return StatusCode(502, new { status = "error", message = "No se pudo enviar el paquete al hemisferio hermano." });
            }

            var route = _store.Routes.FirstOrDefault(x =>
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

            var buffer = _store.Buffers.FirstOrDefault(x => string.Equals(x.BufferId, bufferId, StringComparison.OrdinalIgnoreCase));
            if (buffer == null)
            {
                return NotFound(new { status = "error", message = $"Buffer {bufferId} no encontrado." });
            }

            buffer.ItemsInQueue = 0;
            buffer.CapacityPercentage = 0;
            _store.EventsProcessed += 1;
            _store.QueueOccupancyPercentage = _store.Buffers.Any() ? _store.Buffers.Average(x => x.CapacityPercentage) : 0.0;

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

    [HttpGet("exportbuffercsv")]
    public IActionResult ExportBuffersCsv([FromQuery] string? bufferId)
    {
        EnsureStoreInitialized();

        var buffers = string.IsNullOrWhiteSpace(bufferId)
            ? _store.Buffers
            : _store.Buffers.Where(x => x.BufferId.Equals(bufferId, StringComparison.OrdinalIgnoreCase)).ToList();

        var csvLines = new List<string>
        {
            "BufferId,Type,ItemsInQueue,CapacityPercentage"
        };

        csvLines.AddRange(buffers.Select(buffer =>
            $"{buffer.BufferId},{buffer.Type},{buffer.ItemsInQueue},{buffer.CapacityPercentage}"));

        var csvBytes = System.Text.Encoding.UTF8.GetBytes(string.Join("\n", csvLines));
        var fileName = string.IsNullOrWhiteSpace(bufferId)
            ? "relay-buffers.csv"
            : $"relay-buffer-{bufferId}.csv";

        return File(csvBytes, "text/csv", fileName);
    }

    private RelayDashboardViewModel BuildDashboardModel()
    {
        EnsureStoreInitialized();

        var model = new RelayDashboardViewModel
        {
            Hemisphere = _settings.Hemisphere,
            LastUpdated = DateTime.Now,
            Status = BuildStatusFromStore(),
            Routes = _store.Routes,
            Buffers = _store.Buffers,
            RecentEvents = new List<EventDto>
            {
                new() { Timestamp = DateTime.Now.AddMinutes(-2), Level = "Info", DisplayLevel = _localizer["Information"].Value, Message = _localizer["RelayEventProcessed"].Value },
                new() { Timestamp = DateTime.Now.AddMinutes(-5), Level = "Warning", DisplayLevel = _localizer["Warning"].Value, Message = _localizer["RelayEventCongestion"].Value },
                new() { Timestamp = DateTime.Now.AddMinutes(-8), Level = "Info", DisplayLevel = _localizer["Information"].Value, Message = _localizer["RelayEventConnection"].Value }
            },
            ActionMessage = "Snapshot cargado correctamente."
        };

        return model;
    }

    private RelayStatusDto BuildStatusFromStore()
    {
        var activeRelays = _store.Routes.Count(r => r.Status.Contains("active", StringComparison.OrdinalIgnoreCase) || r.Status.Contains("activa", StringComparison.OrdinalIgnoreCase));
        var inactiveRelays = _store.Routes.Count(r => r.Status.Contains("inactive", StringComparison.OrdinalIgnoreCase) || r.Status.Contains("inactiva", StringComparison.OrdinalIgnoreCase));
        var inferredInactive = inactiveRelays > 0 ? inactiveRelays : Math.Max(0, _store.Routes.Count - activeRelays);
        var northOccupancy = _store.Routes.Where(r => r.FromSatellite.StartsWith("SAT-") || r.ToAntenna.Contains("N", StringComparison.OrdinalIgnoreCase)).Select(r => r.QueueOccupancyPercentage).DefaultIfEmpty(_store.QueueOccupancyPercentage).Average();
        var southOccupancy = _store.Routes.Where(r => r.FromSatellite.StartsWith("ANT-S") || r.ToAntenna.Contains("S", StringComparison.OrdinalIgnoreCase)).Select(r => r.QueueOccupancyPercentage).DefaultIfEmpty(_store.QueueOccupancyPercentage - 10).Average();

        return new RelayStatusDto
        {
            ActiveRelays = activeRelays,
            InactiveRelays = inferredInactive,
            TotalPacketsProcessed = _store.Routes.Sum(r => r.PacketCount),
            AvgQueueOccupancy = _store.QueueOccupancyPercentage,
            NorthQueueOccupancy = northOccupancy,
            SouthQueueOccupancy = southOccupancy
        };
    }

    private void EnsureStoreInitialized()
    {
        if (_store.Routes.Any() && _store.Buffers.Any())
        {
            return;
        }

        var routesData = MockDataService.GetRoutesData();
        var buffersData = MockDataService.GetBuffersData();

        _store.Routes = routesData.Routes.Select(route => new RouteDto
        {
            FromSatellite = route.Source,
            ToAntenna = route.Destination,
            Status = route.Status,
            QueueOccupancyPercentage = route.Packets > 0 ? route.Packets * 1.8 : 0,
            LastSeen = DateTime.Now.AddMinutes(-route.Hops),
            PacketData = $"{route.Id}:{route.Packets} paquetes",
            PacketCount = route.Packets
        }).ToList();

        _store.Buffers = buffersData.Buffers.Select(buffer => new BufferDto
        {
            BufferId = buffer.Id,
            Type = buffer.Status,
            ItemsInQueue = buffer.Occupied,
            CapacityPercentage = buffer.OccupancyPercent
        }).ToList();

        _store.QueueOccupancyPercentage = _store.Buffers.Any() ? _store.Buffers.Average(x => x.CapacityPercentage) : 0.0;
    }
}

public class ForceSendRequest
{
    public string FromSatellite { get; set; } = string.Empty;
    public string ToAntenna { get; set; } = string.Empty;
    public string PacketData { get; set; } = string.Empty;
    public int Priority { get; set; } = 1;
}

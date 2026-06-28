using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OrbitNet.Web.Configuration;
using OrbitNet.Web.Models.ViewModels;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers;

public class RelayDashboardController : Controller
{
    private readonly AppInstanceSettings _settings;
    private readonly RelayHttpService _relayHttpService;
    private readonly OrbitNetStore _store;
    private readonly ILogger<RelayDashboardController> _logger;

    public RelayDashboardController(
        IOptions<AppInstanceSettings> settings,
        RelayHttpService relayHttpService,
        OrbitNetStore store,
        ILogger<RelayDashboardController> logger)
    {
        _settings = settings.Value;
        _relayHttpService = relayHttpService;
        _store = store;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var model = BuildDashboardModel();
        return View(model);
    }

    [HttpGet("refresh")]
    public IActionResult Refresh()
    {
        var model = BuildDashboardModel();

        return Json(new
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

            return Ok(new
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
            _store.QueueOccupancyPercentage = 0.0;
            _store.EventsProcessed += 1;

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

    [HttpGet("exportbuffercsv")]
    public IActionResult ExportBuffersCsv([FromQuery] string? bufferId)
    {
        var buffersData = MockDataService.GetBuffersData();
        var buffers = string.IsNullOrWhiteSpace(bufferId)
            ? buffersData.Buffers
            : buffersData.Buffers.Where(x => x.Id.Equals(bufferId, StringComparison.OrdinalIgnoreCase)).ToList();

        var csvLines = new List<string>
        {
            "BufferId,Type,ItemsInQueue,CapacityPercentage,Status"
        };

        csvLines.AddRange(buffers.Select(buffer =>
            $"{buffer.Id},{buffer.Satellite},{buffer.Occupied},{buffer.OccupancyPercent},{buffer.Status}"));

        var csvBytes = System.Text.Encoding.UTF8.GetBytes(string.Join("\n", csvLines));
        var fileName = string.IsNullOrWhiteSpace(bufferId)
            ? "relay-buffers.csv"
            : $"relay-buffer-{bufferId}.csv";

        return File(csvBytes, "text/csv", fileName);
    }

    private RelayDashboardViewModel BuildDashboardModel()
    {
        var routesData = MockDataService.GetRoutesData();
        var buffersData = MockDataService.GetBuffersData();

        return new RelayDashboardViewModel
        {
            Hemisphere = _settings.Hemisphere,
            LastUpdated = DateTime.Now,
            Status = new RelayStatusDto
            {
                ActiveRelays = routesData.ActiveRoutes,
                InactiveRelays = Math.Max(0, routesData.TotalRoutes - routesData.ActiveRoutes),
                TotalPacketsProcessed = routesData.Routes.Sum(r => r.Packets),
                AvgQueueOccupancy = buffersData.AverageOccupancy
            },
            Routes = routesData.Routes.Select(route => new RouteDto
            {
                FromSatellite = route.Source,
                ToAntenna = route.Destination,
                Status = route.Status,
                QueueOccupancyPercentage = route.Packets > 0 ? route.Packets * 1.8 : 0,
                LastSeen = DateTime.Now.AddMinutes(-route.Hops),
                PacketData = $"{route.Id}:{route.Packets} paquetes"
            }).ToList(),
            Buffers = buffersData.Buffers.Select(buffer => new BufferDto
            {
                BufferId = buffer.Id,
                Type = buffer.Status,
                ItemsInQueue = buffer.Occupied,
                CapacityPercentage = buffer.OccupancyPercent
            }).ToList(),
            RecentEvents = new List<EventDto>
            {
                new() { Timestamp = DateTime.Now.AddMinutes(-2), Level = "Info", Message = "Paquete relay procesado correctamente." },
                new() { Timestamp = DateTime.Now.AddMinutes(-5), Level = "Warning", Message = "Se detectó congestión leve en el buffer principal." },
                new() { Timestamp = DateTime.Now.AddMinutes(-8), Level = "Info", Message = "Conexión con el hemisferio hermano establecida." }
            },
            ActionMessage = "Snapshot cargado correctamente."
        };
    }
}

public class ForceSendRequest
{
    public string FromSatellite { get; set; } = string.Empty;
    public string ToAntenna { get; set; } = string.Empty;
    public string PacketData { get; set; } = string.Empty;
    public int Priority { get; set; } = 1;
}

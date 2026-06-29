using Microsoft.AspNetCore.Mvc;
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
        return View("~/Views/Relay/Index.cshtml", model);
    }

    [HttpGet("refresh")]
    public IActionResult Refresh()
    {
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

    [HttpPost("clearbuffer")]
    public IActionResult ClearBuffer([FromQuery] string? bufferId)
    {
        if (string.IsNullOrWhiteSpace(bufferId))
        {
            return BadRequest(new { status = "error", message = "Se requiere un identificador de buffer." });
        }

        var buffer = _store.Buffers.Find(x => string.Equals(x.BufferId, bufferId, StringComparison.OrdinalIgnoreCase));
        if (buffer == null)
        {
            return NotFound(new { status = "error", message = $"Buffer {bufferId} no encontrado." });
        }

        buffer.ItemsInQueue = 0;
        buffer.CapacityPercentage = 0;
        _store.EventsProcessed += 1;
        var totalCap = 0.0;
        var bufCount = 0;
        _store.Buffers.ForEach(b => { totalCap += b.CapacityPercentage; bufCount++; });
        _store.QueueOccupancyPercentage = bufCount > 0 ? totalCap / bufCount : 0.0;

        return Ok(new
        {
            status = "success",
            message = $"Buffer {bufferId} limpiado correctamente.",
            bufferId
        });
    }

    [HttpGet("exportbuffercsv")]
    public IActionResult ExportBuffersCsv([FromQuery] string? bufferId)
    {
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
        var fileName = string.IsNullOrWhiteSpace(bufferId)
            ? "relay-buffers.csv"
            : $"relay-buffer-{bufferId}.csv";

        return File(csvBytes, "text/csv", fileName);
    }

    private RelayDashboardViewModel BuildDashboardModel()
    {
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
            RecentEvents = new List<EventDto>(),
            ActionMessage = "Datos cargados desde el store."
        };

        return model;
    }

    private RelayStatusDto BuildStatusFromStore()
    {
        return new RelayStatusDto
        {
            ActiveRelays = _store.ActiveSatellites,
            InactiveRelays = 0,
            TotalPacketsProcessed = _store.EventsProcessed,
            AvgQueueOccupancy = _store.QueueOccupancyPercentage
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

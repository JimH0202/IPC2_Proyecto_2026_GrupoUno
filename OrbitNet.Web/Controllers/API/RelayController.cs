using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;
using OrbitNet.Web.Models.Entities;
using OrbitNet.Web.Models.Enums;

namespace OrbitNet.Web.Controllers.API;

[ApiController]
[Route("api/relay")]
public class RelayController : ControllerBase
{
    private readonly ILogger<RelayController> _logger;
    private readonly OrbitNetDataService _dataService;
    private readonly OrbitNetStore _store;

    public RelayController(
        ILogger<RelayController> logger,
        OrbitNetDataService dataService,
        OrbitNetStore store)
    {
        _logger = logger;
        _dataService = dataService;
        _store = store;
    }

    [HttpGet("routes")]
    public IActionResult GetRoutes()
    {
        try
        {
            var routeData = _dataService.GetReportViewModel("Routing");
            return Ok(new { status = "success", data = routeData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener rutas");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpGet("buffers")]
    public IActionResult GetBuffers()
    {
        try
        {
            var bufferData = _dataService.GetReportViewModel("Buffers");
            return Ok(new { status = "success", data = bufferData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener buffers");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpPost("send-packet")]
    public IActionResult SendPacket([FromBody] RelayRequestDto request)
    {
        try
        {
            _logger.LogInformation($"Enviando paquete desde {request.FromSatellite} a {request.ToAntenna}");

            var packet = new MessagePacket
            {
                CodHex = $"PKT-{DateTime.UtcNow:yyyyMMddHHmmss}",
                SenderId = request.FromSatellite,
                DestinationIp = request.ToAntenna,
                Priority = PriorityLevel.Media,
                Content = request.PacketData
            };

            var receptor = _store.Satellites.FirstOrDefault(s => s.Id == _store.ReceptorSatelliteId);
            receptor?.PaquetesABordo?.Agregar(packet);

            return Ok(new RelaySuccessResponse
            {
                Status = "success",
                Message = $"Paquete encolado en buffer de {_store.ReceptorSatelliteId}",
                QueueOccupancyPercentage = _store.QueueOccupancyPercentage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar paquete");
            return StatusCode(500, new RelayErrorResponse
            {
                Status = "error",
                Details = ex.Message
            });
        }
    }

    [HttpGet("status")]
    public IActionResult GetRelayStatus()
    {
        try
        {
            return Ok(new
            {
                status = "success",
                data = new
                {
                    activeRelays = _store.ActiveSatellites,
                    inactiveRelays = 0,
                    totalPacketsProcessed = _store.EventsProcessed,
                    avgQueueOccupancy = _store.QueueOccupancyPercentage,
                    lastUpdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estado de relés");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }
}

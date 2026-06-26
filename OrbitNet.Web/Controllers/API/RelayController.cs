using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers.API;

[ApiController]
[Route("api/relay")]
public class RelayController : ControllerBase
{
    private readonly ILogger<RelayController> _logger;

    public RelayController(ILogger<RelayController> logger)
    {
        _logger = logger;
    }

    [HttpGet("routes")]
    public IActionResult GetRoutes()
    {
        try
        {
            var routeData = MockDataService.GetRoutesData();
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
            var bufferData = MockDataService.GetBuffersData();
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
            return Ok(new RelaySuccessResponse
            {
                Status = "success",
                Message = $"Paquete enviado correctamente de {request.FromSatellite} a {request.ToAntenna}",
                QueueOccupancyPercentage = 65.5
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
                    activeRelays = 12,
                    inactiveRelays = 2,
                    totalPacketsProcessed = 4523,
                    avgQueueOccupancy = 58.3,
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

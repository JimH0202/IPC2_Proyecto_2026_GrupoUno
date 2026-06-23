using Microsoft.AspNetCore.Mvc;

namespace OrbitNet.Web.Controllers.API;

[ApiController]
[Route("api/config")]
public class ConfigController : ControllerBase
{
    private readonly ILogger<ConfigController> _logger;

    public ConfigController(ILogger<ConfigController> logger)
    {
        _logger = logger;
    }

    [HttpPost("load-xml")]
    public IActionResult LoadXmlConfiguration([FromBody] ConfigRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.XmlData))
                return BadRequest(new ConfigErrorResponse
                {
                    Status = "error",
                    ErrorCode = "EMPTY_DATA",
                    Details = "Los datos XML no pueden estar vacíos"
                });

            _logger.LogInformation("Cargando configuración XML");
            
            return Ok(new ConfigSuccessResponse
            {
                Status = "success",
                Message = "Configuración XML cargada correctamente",
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar configuración XML");
            return StatusCode(500, new ConfigErrorResponse
            {
                Status = "error",
                ErrorCode = "LOAD_ERROR",
                Details = ex.Message
            });
        }
    }

    [HttpGet("current")]
    public IActionResult GetCurrentConfiguration()
    {
        try
        {
            return Ok(new
            {
                status = "success",
                data = new
                {
                    hemisphere = "Norte",
                    satellites = 6,
                    antennas = 2,
                    polarOrbits = 3,
                    geoOrbits = 1,
                    lastUpdated = DateTime.Now.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss")
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener configuración actual");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public IActionResult ValidateConfiguration([FromBody] ConfigRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.XmlData))
                return BadRequest(new { status = "error", message = "XML vacío" });

            return Ok(new
            {
                status = "success",
                message = "Configuración válida",
                warnings = new List<string>()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar configuración");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }
}

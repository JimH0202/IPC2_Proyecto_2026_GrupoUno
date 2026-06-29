using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;
using OrbitNet.Web.Services.Communication;

namespace OrbitNet.Web.Controllers.API;

[ApiController]
[Route("api/config")]
public class ConfigController : ControllerBase
{
    private readonly ILogger<ConfigController> _logger;
    private readonly XmlIngestService _xmlIngestService;
    private readonly BasicAuthService _basicAuthService;
    private readonly OrbitNetStore _store;

    public ConfigController(
        ILogger<ConfigController> logger,
        XmlIngestService xmlIngestService,
        BasicAuthService basicAuthService,
        OrbitNetStore store)
    {
        _logger = logger;
        _xmlIngestService = xmlIngestService;
        _basicAuthService = basicAuthService;
        _store = store;
    }

    private bool Autenticar()
    {
        string? authHeader = Request.Headers.Authorization.FirstOrDefault();
        return _basicAuthService.EsCabeceraValida(authHeader);
    }

    [HttpPost("load-xml")]
    public IActionResult LoadXmlConfiguration([FromBody] ConfigRequestDto request)
    {
        if (!Autenticar())
            return Unauthorized(new { status = "error", message = "Autenticación Basic Auth requerida" });

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

            var result = _xmlIngestService.ProcesarConfiguracion(request.XmlData);

            if (!result.Success)
            {
                return BadRequest(new ConfigErrorResponse
                {
                    Status = "error",
                    ErrorCode = result.ErrorCode ?? "PARSE_ERROR",
                    Details = result.Details ?? "Error al procesar XML"
                });
            }

            return Ok(new ConfigSuccessResponse
            {
                Status = "success",
                Message = $"Configuración XML cargada correctamente. {result.SatellitesLoaded} satélites, {result.AntennasLoaded} antenas.",
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
                    hemisphere = _store.ConfigLoaded ? "Cargada" : "Sin configurar",
                    satellites = _store.ActiveSatellites,
                    antennas = _store.TotalAntennas,
                    polarOrbits = _store.PolarOrbits.Count,
                    geoOrbits = 0,
                    lastUpdated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
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
        if (!Autenticar())
            return Unauthorized(new { status = "error", message = "Autenticación Basic Auth requerida" });

        try
        {
            if (string.IsNullOrEmpty(request.XmlData))
                return BadRequest(new { status = "error", message = "XML vacío" });

            var result = _xmlIngestService.ProcesarConfiguracion(request.XmlData);

            if (result.Success)
            {
                return Ok(new
                {
                    status = "success",
                    message = "Configuración válida",
                    warnings = new string[0]
                });
            }

            return Ok(new
            {
                status = "error",
                message = result.Details ?? "Configuración inválida",
                warnings = new[] { result.ErrorCode }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar configuración");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }
}

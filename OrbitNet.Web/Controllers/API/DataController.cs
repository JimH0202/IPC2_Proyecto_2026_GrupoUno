using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers.API;

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
    private readonly ILogger<DataController> _logger;
    private readonly OrbitNetDataService _dataService;
    private readonly XmlIngestService _xmlIngestService;
    private readonly OrbitNetStore _store;

    public DataController(
        ILogger<DataController> logger,
        OrbitNetDataService dataService,
        XmlIngestService xmlIngestService,
        OrbitNetStore store)
    {
        _logger = logger;
        _dataService = dataService;
        _xmlIngestService = xmlIngestService;
        _store = store;
    }

    [HttpGet("matrix")]
    public IActionResult GetMatrixData()
    {
        try
        {
            var matrixData = _dataService.GetMatrixViewModel();
            return Ok(new { status = "success", data = matrixData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener datos de matriz");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpGet("report/memory")]
    public IActionResult GetMemoryReport()
    {
        try
        {
            var reportData = _dataService.GetReportViewModel("MemoryLayout");
            return Ok(new { status = "success", data = reportData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener reporte de memoria");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpGet("report/{reportType}")]
    public IActionResult GetReport(string reportType)
    {
        try
        {
            var report = _dataService.GetReportViewModel(reportType);
            return Ok(new
            {
                status = "success",
                data = new
                {
                    title = report.Title,
                    description = report.Description,
                    generatedAt = report.GeneratedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    svgContent = report.SvgContent
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener reporte {reportType}");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpGet("live-simulation")]
    public IActionResult GetLiveSimulationData()
    {
        try
        {
            var liveData = _dataService.GetSimulationViewModel();
            return Ok(new { status = "success", data = liveData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener datos en vivo");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpPost("upload-config")]
    public IActionResult UploadConfiguration([FromForm] IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { status = "error", message = "Archivo no válido" });

            using var reader = new StreamReader(file.OpenReadStream());
            string xmlContent = reader.ReadToEnd();

            var result = _xmlIngestService.ProcesarConfiguracion(xmlContent);

            return Ok(new
            {
                status = result.Success ? "success" : "error",
                data = new
                {
                    fileName = file.FileName,
                    success = result.Success,
                    message = result.Success ? "Configuración cargada exitosamente" : "Error al cargar configuración",
                    satellitesLoaded = result.SatellitesLoaded,
                    antennasLoaded = result.AntennasLoaded,
                    polarOrbitsLoaded = result.PolarOrbitsLoaded,
                    errors = result.Success ? Array.Empty<string>() : new[] { result.Details }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar configuración");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }
}

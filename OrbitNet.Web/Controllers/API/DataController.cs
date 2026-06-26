using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers.API;

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
    private readonly ILogger<DataController> _logger;

    public DataController(ILogger<DataController> logger)
    {
        _logger = logger;
    }

    [HttpGet("matrix")]
    public IActionResult GetMatrixData()
    {
        try
        {
            var matrixData = MockDataService.GetMatrixData();
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
            var reportData = MockDataService.GetMemoryReportData();
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
            var report = MockDataService.GetReportViewModel(reportType);
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
            var liveData = MockDataService.GetLiveSimulationData();
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

            var uploadResult = MockDataService.GetUploadResultViewModel();
            return Ok(new
            {
                status = "success",
                data = new
                {
                    fileName = uploadResult.FileName,
                    success = uploadResult.Success,
                    message = uploadResult.Message,
                    satellitesLoaded = uploadResult.SatellitesLoaded,
                    antennasLoaded = uploadResult.AntennasLoaded,
                    polarOrbitsLoaded = uploadResult.PolarOrbitsLoaded,
                    errors = uploadResult.Errors
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

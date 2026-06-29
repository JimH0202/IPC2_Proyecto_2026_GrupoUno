using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;
using OrbitNet.Web.Services.Communication;

namespace OrbitNet.Web.Controllers.API;

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
    private readonly OrbitNetDataService _dataService;
    private readonly XmlIngestService _xmlIngestService;
    private readonly BasicAuthService _basicAuthService;
    private readonly OrbitNetStore _store;

    public DataController(
        OrbitNetDataService dataService,
        XmlIngestService xmlIngestService,
        BasicAuthService basicAuthService,
        OrbitNetStore store)
    {
        _dataService = dataService;
        _xmlIngestService = xmlIngestService;
        _basicAuthService = basicAuthService;
        _store = store;
    }

    private bool Autenticar()
    {
        string? authHeader = Request.Headers.Authorization.FirstOrDefault();
        return _basicAuthService.EsCabeceraValida(authHeader);
    }

    [HttpGet("matrix")]
    public IActionResult GetMatrixData()
    {
        var matrixData = _dataService.GetMatrixViewModel();
        return Ok(new { status = "success", data = matrixData });
    }

    [HttpGet("report/memory")]
    public IActionResult GetMemoryReport()
    {
        var reportData = _dataService.GetReportViewModel("MemoryLayout");
        return Ok(new { status = "success", data = reportData });
    }

    [HttpGet("report/{reportType}")]
    public IActionResult GetReport(string reportType)
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

    [HttpGet("live-simulation")]
    public IActionResult GetLiveSimulationData()
    {
        var liveData = _dataService.GetSimulationViewModel();
        return Ok(new { status = "success", data = liveData });
    }

    [HttpPost("upload-config")]
    public IActionResult UploadConfiguration([FromForm] IFormFile file)
    {
        if (!Autenticar())
            return Unauthorized(new { status = "error", message = "Autenticación Basic Auth requerida" });

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
}

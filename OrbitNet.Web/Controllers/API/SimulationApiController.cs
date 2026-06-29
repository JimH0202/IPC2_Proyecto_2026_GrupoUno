using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;
using OrbitNet.Web.Services.SimulationEngine;

namespace OrbitNet.Web.Controllers.API;

[ApiController]
[Route("api/simulation")]
public class SimulationApiController : ControllerBase
{
    private readonly ILogger<SimulationApiController> _logger;
    private readonly OrbitNetDataService _dataService;
    private readonly OrbitNetStore _store;
    private readonly TickProcessor _tickProcessor;

    public SimulationApiController(
        ILogger<SimulationApiController> logger,
        OrbitNetDataService dataService,
        OrbitNetStore store,
        TickProcessor tickProcessor)
    {
        _logger = logger;
        _dataService = dataService;
        _store = store;
        _tickProcessor = tickProcessor;
    }

    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        try
        {
            var dashboard = _dataService.GetDashboardViewModel();
            return Ok(new
            {
                status = "success",
                data = new
                {
                    currentTick = dashboard.CurrentTick,
                    activeSatellites = dashboard.ActiveSatellites,
                    inactiveSatellites = dashboard.InactiveSatellites,
                    pendingMessages = dashboard.PendingMessages,
                    processedMessages = dashboard.ProcessedMessages,
                    totalAntennas = dashboard.TotalAntennas,
                    hemisphere = dashboard.Hemisphere,
                    isSimulationRunning = dashboard.IsSimulationRunning
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener dashboard");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpGet("satellites")]
    public IActionResult GetSatellites()
    {
        try
        {
            var satellites = _dataService.GetSatelliteList();
            return Ok(new
            {
                status = "success",
                data = satellites.Select(s => new
                {
                    id = s.Id,
                    name = s.Name,
                    orbitId = s.OrbitId,
                    type = s.Type,
                    x = s.X,
                    y = s.Y,
                    isActive = s.IsActive,
                    messagesSent = s.MessagesSent,
                    messagesReceived = s.MessagesReceived
                }).ToArray()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener satélites");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpGet("satellites/{id}")]
    public IActionResult GetSatelliteDetails(string id)
    {
        try
        {
            var satellite = _dataService.GetSatelliteDetails(id);
            return Ok(new
            {
                status = "success",
                data = new
                {
                    id = satellite.Id,
                    name = satellite.Name,
                    orbitId = satellite.OrbitId,
                    type = satellite.Type,
                    x = satellite.X,
                    y = satellite.Y,
                    isActive = satellite.IsActive,
                    messagesSent = satellite.MessagesSent,
                    messagesReceived = satellite.MessagesReceived
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener satélite {id}");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpGet("logs")]
    public IActionResult GetLogs()
    {
        try
        {
            var logs = _dataService.GetLogEntries();
            return Ok(new
            {
                status = "success",
                data = logs.Select(l => new
                {
                    timestamp = l.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    level = l.Level,
                    @event = l.Event,
                    details = l.Details
                }).ToArray()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener logs");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpPost("step")]
    public IActionResult StepSimulation([FromBody] SimulationStepRequestDto request)
    {
        try
        {
            _logger.LogInformation($"Avanzando simulación {request.Ticks} ticks");
            var response = _tickProcessor.AvanzarSimulacion(request.Ticks);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al avanzar simulación");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpPost("start")]
    public IActionResult StartSimulation()
    {
        try
        {
            _logger.LogInformation("Iniciando simulación");
            _store.IsSimulationRunning = true;
            return Ok(new { status = "success", message = "Simulación iniciada", currentTick = _store.CurrentTick });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar simulación");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpPost("pause")]
    public IActionResult PauseSimulation()
    {
        try
        {
            _logger.LogInformation("Pausando simulación");
            _store.IsSimulationRunning = false;
            return Ok(new { status = "success", message = "Simulación pausada" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al pausar simulación");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpPost("reset")]
    public IActionResult ResetSimulation()
    {
        try
        {
            _logger.LogInformation("Reiniciando simulación");
            _store.Clear();
            return Ok(new { status = "success", message = "Simulación reiniciada", currentTick = 0 });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reiniciar simulación");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }
}

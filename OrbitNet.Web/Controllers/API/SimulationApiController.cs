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

    [HttpGet("satellites")]
    public IActionResult GetSatellites()
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

    [HttpGet("satellites/{id}")]
    public IActionResult GetSatelliteDetails(string id)
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

    [HttpGet("logs")]
    public IActionResult GetLogs()
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

    [HttpPost("step")]
    public IActionResult StepSimulation([FromBody] SimulationStepRequestDto request)
    {
        _logger.LogInformation("Avanzando simulación {Ticks} ticks", request.Ticks);
        var response = _tickProcessor.AvanzarSimulacion(request.Ticks);
        return Ok(response);
    }

    [HttpPost("start")]
    public IActionResult StartSimulation()
    {
        _logger.LogInformation("Iniciando simulación");
        _store.IsSimulationRunning = true;
        return Ok(new { status = "success", message = "Simulación iniciada", currentTick = _store.CurrentTick });
    }

    [HttpPost("pause")]
    public IActionResult PauseSimulation()
    {
        _logger.LogInformation("Pausando simulación");
        _store.IsSimulationRunning = false;
        return Ok(new { status = "success", message = "Simulación pausada" });
    }

    [HttpPost("reset")]
    public IActionResult ResetSimulation()
    {
        _logger.LogInformation("Reiniciando simulación");
        _store.Clear();
        return Ok(new { status = "success", message = "Simulación reiniciada", currentTick = 0 });
    }
}

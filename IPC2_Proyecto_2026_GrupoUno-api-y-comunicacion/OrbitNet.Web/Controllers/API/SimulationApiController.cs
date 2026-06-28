using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;
using OrbitNet.Web.Models.DTOs;
using OrbitNet.Web.Services.SimulationEngine;

namespace OrbitNet.Web.Controllers.API;

[ApiController]
[Route("api/simulation")]
public class SimulationApiController : ControllerBase
{
    private readonly ILogger<SimulationApiController> _logger;

     // Aquí estoy agregando el motor real de simulación
        private readonly TickProcessor _tickProcessor;

    public SimulationApiController(
            ILogger<SimulationApiController> logger,
            TickProcessor tickProcessor)
    {
        _logger = logger;
        _tickProcessor = tickProcessor; // Aquí estamos asignando el motor real a la variable
    }

    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        try
        {
            var dashboard = MockDataService.GetDashboardViewModel();
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
            var satellites = MockDataService.GetSatelliteList();
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
            var satellite = MockDataService.GetSatelliteDetails(id);
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
            var logs = MockDataService.GetLogEntries();
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

    //Aquí estoy integrando el Step al motor de simulación real
    [HttpPost("step")]
    public IActionResult StepSimulation([FromBody] SimulationStepRequest request)
    {
        try
        {
            // Validación básica del body
            if (request == null)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "La solicitud no puede venir vacía."
                });
            }

            // Si mandan 0 o negativo, lo forzamos a 1 tick
            if (request.Ticks < 1)
            {
                request.Ticks = 1;
            }

            _logger.LogInformation("Avanzando simulación {Ticks} tick(s)", request.Ticks);

            // Aquí ya se llama al motor real
            SimulationStepResponse response = _tickProcessor.AvanzarSimulacion(request.Ticks);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al avanzar simulación");
            return StatusCode(500, new
            {
                status = "error",
                message = "Ocurrió un error interno al avanzar la simulación."
            });
        }
    }

    [HttpPost("start")]
    public IActionResult StartSimulation()
    {
        try
        {
            _logger.LogInformation("Iniciando simulación");
            return Ok(new { status = "success", message = "Simulación iniciada", currentTick = 0 });
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
            return Ok(new { status = "success", message = "Simulación reiniciada", currentTick = 0 });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reiniciar simulación");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }
}

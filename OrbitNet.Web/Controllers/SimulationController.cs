using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;
using OrbitNet.Web.Services.SimulationEngine;

namespace OrbitNet.Web.Controllers;

public class SimulationController : Controller
{
    private readonly OrbitNetDataService _dataService;
    private readonly OrbitNetStore _store;
    private readonly TickProcessor _tickProcessor;

    public SimulationController(OrbitNetDataService dataService, OrbitNetStore store, TickProcessor tickProcessor)
    {
        _dataService = dataService;
        _store = store;
        _tickProcessor = tickProcessor;
    }

    public IActionResult Dashboard()
    {
        var model = _dataService.GetSimulationViewModel();
        return View(model);
    }

    [HttpGet]
    [HttpPost]
    public IActionResult TickResult(int count = 1)
    {
        if (count > 0)
        {
            _tickProcessor.AvanzarSimulacion(count);
        }
        _store.IsSimulationRunning = true;
        var model = _dataService.GetSimulationViewModel();
        return View(model);
    }

    [HttpPost]
    public IActionResult ExecuteTicks(int count)
    {
        return RedirectToAction("TickResult", new { count });
    }

    [HttpPost]
    public IActionResult StopSimulation()
    {
        _store.IsSimulationRunning = false;
        return RedirectToAction("Dashboard");
    }
}

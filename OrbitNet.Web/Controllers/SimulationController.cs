using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers;

public class SimulationController : Controller
{
    private readonly OrbitNetDataService _dataService;
    private readonly OrbitNetStore _store;

    public SimulationController(OrbitNetDataService dataService, OrbitNetStore store)
    {
        _dataService = dataService;
        _store = store;
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
        var model = _dataService.GetSimulationViewModel(additionalTicks: count);
        _store.IsSimulationRunning = true;
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

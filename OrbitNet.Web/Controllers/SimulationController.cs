using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers;

public class SimulationController : Controller
{
    public IActionResult Dashboard()
    {
        var model = MockDataService.GetSimulationViewModel();
        return View(model);
    }

    [HttpGet]
    [HttpPost]
    public IActionResult TickResult(int count = 1, bool isRunning = true)
    {
        var model = MockDataService.GetSimulationViewModel(additionalTicks: count);
        if (!isRunning) model.IsSimulationRunning = false;
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
        return RedirectToAction("TickResult", new { isRunning = false });
    }
}

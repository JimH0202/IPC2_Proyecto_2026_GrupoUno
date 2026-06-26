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

    [HttpPost]
    public IActionResult TickResult(int count = 1)
    {
        var model = MockDataService.GetSimulationViewModel(additionalTicks: count);
        return View(model);
    }

    [HttpPost]
    public IActionResult ExecuteTicks(int count)
    {
        var model = MockDataService.GetSimulationViewModel(additionalTicks: count);
        return View("TickResult", model);
    }

    [HttpPost]
    public IActionResult StopSimulation()
    {
        var model = MockDataService.GetSimulationViewModel(isRunning: false);
        return View("TickResult", model);
    }
}

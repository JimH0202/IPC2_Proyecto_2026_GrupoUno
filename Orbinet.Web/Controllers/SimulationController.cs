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

    public IActionResult TickResult()
    {
        var model = MockDataService.GetSimulationViewModel();
        return View(model);
    }
}

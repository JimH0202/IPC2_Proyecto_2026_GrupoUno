using Microsoft.AspNetCore.Mvc;

namespace OrbitNet.Controllers;

public class SimulationController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }
}
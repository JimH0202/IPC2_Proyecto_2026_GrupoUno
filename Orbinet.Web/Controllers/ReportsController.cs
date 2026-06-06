using Microsoft.AspNetCore.Mvc;

namespace OrbitNet.Controllers;

public class ReportsController : Controller
{
    public IActionResult MemoryLayout()
    {
        return View();
    }

    public IActionResult Routing()
    {
        return View();
    }

    public IActionResult Buffers()
    {
        return View();
    }
}
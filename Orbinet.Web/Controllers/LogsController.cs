using Microsoft.AspNetCore.Mvc;

namespace OrbitNet.Controllers;

public class LogsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
using Microsoft.AspNetCore.Mvc;

namespace OrbitNet.Controllers;

public class UploadController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Result()
    {
        return View();
    }
}
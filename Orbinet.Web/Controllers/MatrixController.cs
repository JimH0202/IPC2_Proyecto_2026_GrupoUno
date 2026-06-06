using Microsoft.AspNetCore.Mvc;

namespace OrbitNet.Controllers;

public class MatrixController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
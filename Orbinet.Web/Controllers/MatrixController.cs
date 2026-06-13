using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers;

public class MatrixController : Controller
{
    public IActionResult Index()
    {
        var model = MockDataService.GetMatrixViewModel();
        return View(model);
    }
}

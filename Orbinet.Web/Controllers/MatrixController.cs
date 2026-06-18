using Microsoft.AspNetCore.Mvc;
using Orbinet.Web.Services;

namespace Orbinet.Web.Controllers;

public class MatrixController : Controller
{
    public IActionResult Index()
    {
        var model = MockDataService.GetMatrixViewModel();
        return View(model);
    }
}

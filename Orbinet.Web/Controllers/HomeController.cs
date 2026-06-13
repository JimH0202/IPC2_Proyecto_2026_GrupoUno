using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var model = MockDataService.GetDashboardViewModel();
        return View(model);
    }
}

using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers;

public class LogsController : Controller
{
    public IActionResult Index()
    {
        var model = MockDataService.GetLogEntries();
        return View(model);
    }
}
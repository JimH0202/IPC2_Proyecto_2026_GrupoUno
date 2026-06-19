using Microsoft.AspNetCore.Mvc;
using Orbinet.Web.Services;

namespace Orbinet.Web.Controllers;

public class LogsController : Controller
{
    public IActionResult Index()
    {
        var model = MockDataService.GetLogEntries();
        return View(model);
    }
}
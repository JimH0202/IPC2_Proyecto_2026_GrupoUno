using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers;

public class LogsController : Controller
{
    private readonly OrbitNetDataService _dataService;

    public LogsController(OrbitNetDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult Index()
    {
        var model = _dataService.GetLogEntries();
        return View(model);
    }
}
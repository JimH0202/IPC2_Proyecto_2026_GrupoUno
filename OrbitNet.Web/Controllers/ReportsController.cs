using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers;

public class ReportsController : Controller
{
    private readonly OrbitNetDataService _dataService;

    public ReportsController(OrbitNetDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult MemoryLayout()
    {
        var model = _dataService.GetReportViewModel("MemoryLayout");
        return View(model);
    }

    public IActionResult Routing()
    {
        var model = _dataService.GetReportViewModel("Routing");
        return View(model);
    }

    public IActionResult Buffers()
    {
        var model = _dataService.GetReportViewModel("Buffers");
        return View(model);
    }
}

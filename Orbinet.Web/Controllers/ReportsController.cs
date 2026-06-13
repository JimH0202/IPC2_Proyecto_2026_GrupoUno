using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers;

public class ReportsController : Controller
{
    public IActionResult MemoryLayout()
    {
        var model = MockDataService.GetReportViewModel("MemoryLayout");
        return View(model);
    }

    public IActionResult Routing()
    {
        var model = MockDataService.GetReportViewModel("Routing");
        return View(model);
    }

    public IActionResult Buffers()
    {
        var model = MockDataService.GetReportViewModel("Buffers");
        return View(model);
    }
}

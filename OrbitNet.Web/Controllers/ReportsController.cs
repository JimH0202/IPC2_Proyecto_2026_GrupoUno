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

    public IActionResult ExportSvg(string report)
    {
        string svg = report switch
        {
            "MemoryLayout" => _dataService.GetReportViewModel("MemoryLayout").SvgContent,
            "Routing" => _dataService.GetReportViewModel("Routing").SvgContent,
            "Buffers" => _dataService.GetReportViewModel("Buffers").SvgContent,
            "Matrix" => _dataService.GetMatrixViewModel().SvgContent,
            _ => ""
        };

        if (string.IsNullOrWhiteSpace(svg))
            return NotFound();

        return File(
            System.Text.Encoding.UTF8.GetBytes(svg),
            "image/svg+xml",
            $"{report}.svg"
        );
    }
}

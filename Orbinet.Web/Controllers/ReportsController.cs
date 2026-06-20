using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;
using OrbitNet.Web.Services.Graphviz;
using System.Text;

namespace OrbitNet.Web.Controllers;

public class ReportsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult MemoryLayout()
    {
        var model = MockDataService.GetReportViewModel("MemoryLayout");
        if (model != null && !string.IsNullOrWhiteSpace(model.SvgContent))
        {
            model.SvgContent = DotCompiler.SanitizeSvg(model.SvgContent);
        }

        return View(model);
    }

    public IActionResult Routing()
    {
        var model = MockDataService.GetReportViewModel("Routing");
        if (model != null && !string.IsNullOrWhiteSpace(model.SvgContent))
        {
            model.SvgContent = DotCompiler.SanitizeSvg(model.SvgContent);
        }

        return View(model);
    }

    public IActionResult Buffers()
    {
        var model = MockDataService.GetReportViewModel("Buffers");
        if (model != null && !string.IsNullOrWhiteSpace(model.SvgContent))
        {
            model.SvgContent = DotCompiler.SanitizeSvg(model.SvgContent);
        }

        return View(model);
    }

    public IActionResult ExportSvg(string report)
    {
        if (string.IsNullOrWhiteSpace(report)) return BadRequest();

        var model = MockDataService.GetReportViewModel(report);
        if (model == null || string.IsNullOrWhiteSpace(model.SvgContent)) return NotFound();

        var bytes = Encoding.UTF8.GetBytes(model.SvgContent);
        return File(bytes, "image/svg+xml", report + ".svg");
    }
}

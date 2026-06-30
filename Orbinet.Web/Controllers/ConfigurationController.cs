using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers;

public class ConfigurationController : Controller
{
    private readonly IWebHostEnvironment _environment;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public ConfigurationController(IWebHostEnvironment environment, IStringLocalizer<SharedResource> localizer)
    {
        _environment = environment;
        _localizer = localizer;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult DownloadUserManual()
    {
        return DownloadDocument("MANUAL_USUARIO.txt", "MANUAL_USUARIO.txt", "text/plain; charset=utf-8");
    }

    [HttpGet]
    public IActionResult DownloadTechnicalManual()
    {
        return DownloadDocument("MANUAL_TECNICO.md", "MANUAL_TECNICO.md", "text/markdown; charset=utf-8");
    }

    private IActionResult DownloadDocument(string fileName, string downloadName, string contentType)
    {
        var filePath = Path.Combine(_environment.ContentRootPath, "Documentacion", fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(_localizer["ErrorTitle"].Value);
        }

        var bytes = System.IO.File.ReadAllBytes(filePath);
        return File(bytes, contentType, downloadName);
    }
}
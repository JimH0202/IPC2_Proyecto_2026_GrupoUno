using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;
using System.Text.Json;

namespace OrbitNet.Web.Controllers;

public class UploadController : Controller
{
    private readonly XmlIngestService _xmlIngestService;
    private readonly OrbitNetDataService _dataService;

    public UploadController(XmlIngestService xmlIngestService, OrbitNetDataService dataService)
    {
        _xmlIngestService = xmlIngestService;
        _dataService = dataService;
    }

    public IActionResult Index()
    {
        return View(new OrbitNet.Web.Models.ViewModels.UploadViewModel());
    }

    [HttpGet]
    public IActionResult Result()
    {
        if (TempData.Peek("UploadResultModel") is string savedModelJson)
        {
            var model = JsonSerializer.Deserialize<OrbitNet.Web.Models.ViewModels.UploadViewModel>(savedModelJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (model != null)
            {
                return View(model);
            }
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Result(Microsoft.AspNetCore.Http.IFormFile xmlFile)
    {
        if (xmlFile == null || xmlFile.Length == 0)
        {
            var errorModel = _dataService.GetUploadResultViewModel("", false, "No se selecciono ningun archivo.", 0, 0, 0, new List<string> { "Debe seleccionar un archivo XML." });
            TempData["UploadResultModel"] = JsonSerializer.Serialize(errorModel);
            return View(errorModel);
        }

        using var reader = new System.IO.StreamReader(xmlFile.OpenReadStream());
        string xmlContent = reader.ReadToEnd();

        var result = _xmlIngestService.ProcesarConfiguracion(xmlContent);

        var model = _dataService.GetUploadResultViewModel(
            xmlFile.FileName,
            result.Success,
            result.Success ? "Archivo procesado exitosamente." : $"Error: {result.Details}",
            result.SatellitesLoaded,
            result.AntennasLoaded,
            result.PolarOrbitsLoaded,
            result.Success ? null : new List<string> { result.Details ?? "" }
        );

        TempData["UploadResultModel"] = JsonSerializer.Serialize(model);

        return View(model);
    }
}

using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;
using System.Text.Json;

namespace OrbitNet.Web.Controllers;

public class UploadController : Controller
{
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
        var model = MockDataService.GetUploadResultViewModel();
        if (xmlFile != null)
        {
            model.FileName = xmlFile.FileName;
            model.Message = "Archivo recibido y simulado correctamente.";
        }

        TempData["UploadResultModel"] = JsonSerializer.Serialize(model);

        return View(model);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OrbitNet.Web.Services;
using System.Text.Json;

namespace OrbitNet.Web.Controllers;

public class UploadController : Controller
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    public UploadController(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
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
                model.Message = model.Success
                    ? _localizer["UploadSuccessMessage"]
                    : _localizer["UploadNoFileSelectedMessage"];

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
            model.Success = true;
            model.Message = _localizer["UploadSuccessMessage"];
        }
        else
        {
            model.Success = false;
            model.FileName = string.Empty;
            model.Message = _localizer["UploadNoFileSelectedMessage"];
        }

        TempData["UploadResultModel"] = JsonSerializer.Serialize(model);

        return View(model);
    }
}

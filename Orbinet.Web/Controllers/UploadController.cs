using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OrbitNet.Web.Services;

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

        return View(model);
    }
}

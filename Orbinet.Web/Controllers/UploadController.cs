using Microsoft.AspNetCore.Mvc;
using Orbinet.Web.Services;

namespace Orbinet.Web.Controllers;

public class UploadController : Controller
{
    public IActionResult Index()
    {
        return View(new Orbinet.Web.Models.ViewModels.UploadViewModel());
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

        return View(model);
    }
}

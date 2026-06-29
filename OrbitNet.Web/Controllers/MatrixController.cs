using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers;

public class MatrixController : Controller
{
    private readonly OrbitNetDataService _dataService;

    public MatrixController(OrbitNetDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult Index()
    {
        var model = _dataService.GetMatrixViewModel();
        return View(model);
    }
}

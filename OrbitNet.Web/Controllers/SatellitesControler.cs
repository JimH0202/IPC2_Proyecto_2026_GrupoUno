using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers;

public class SatellitesController : Controller
{
    private readonly OrbitNetDataService _dataService;

    public SatellitesController(OrbitNetDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult Index()
    {
        var model = _dataService.GetSatelliteList();
        return View(model);
    }

    public IActionResult Details(string id)
    {
        var model = _dataService.GetSatelliteDetails(id);
        return View(model);
    }
}
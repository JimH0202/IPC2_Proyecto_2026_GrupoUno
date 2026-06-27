using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers;

public class SatellitesController : Controller
{
    public IActionResult Index()
    {
        var model = MockDataService.GetSatelliteList();
        return View(model);
    }

    public IActionResult Details(string id)
    {
        var model = MockDataService.GetSatelliteDetails(id);
        return View(model);
    }
}
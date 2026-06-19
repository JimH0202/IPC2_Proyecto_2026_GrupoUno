using Microsoft.AspNetCore.Mvc;
using Orbinet.Web.Services;

namespace Orbinet.Web.Controllers;

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
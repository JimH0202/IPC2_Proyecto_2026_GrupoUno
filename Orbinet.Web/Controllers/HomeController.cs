using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Orbinet.Web.Configuration;
using Orbinet.Web.Models;
using Orbinet.Web.Services;

namespace Orbinet.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppInstanceSettings _settings; 

    public HomeController(IOptions<AppInstanceSettings> settings) 
    {
        _settings = settings.Value;
    }

    public IActionResult Index()
    {
        var model = MockDataService.GetDashboardViewModel();
        model.Hemisphere = _settings.Hemisphere;
        model.Port = _settings.Port;
        model.RemoteHemisphereUrl = _settings.RemoteHemisphereUrl;
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrbitNet.Web.Configuration;
using OrbitNet.Web.Services;
using OrbitNet.Web.Models;

namespace OrbitNet.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppInstanceSettings _settings;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public HomeController(IOptions<AppInstanceSettings> settings, IStringLocalizer<SharedResource> localizer)
    {
        _settings = settings.Value;
        _localizer = localizer;
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

    [HttpGet]
    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        LocalizationService.SetLanguage(Response, culture);
        
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class SharedResource
{
}

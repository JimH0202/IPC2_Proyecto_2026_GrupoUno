using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OrbitNet.Web.Configuration;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers;

public class ConfigurationController : Controller
{
    private readonly AppInstanceSettings _settings;
    private readonly OrbitNetStore _store;
    private readonly ILogger<ConfigurationController> _logger;

    public ConfigurationController(
        IOptions<AppInstanceSettings> settings,
        OrbitNetStore store,
        ILogger<ConfigurationController> logger)
    {
        _settings = settings.Value;
        _store = store;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.Hemisphere = _settings.Hemisphere;
        ViewBag.Port = _settings.Port;
        ViewBag.RemoteHemisphereUrl = _settings.RemoteHemisphereUrl;
        ViewBag.ConfigLoaded = _store.ConfigLoaded;
        ViewBag.ActiveSatellites = _store.ActiveSatellites;
        ViewBag.TotalAntennas = _store.TotalAntennas;

        return View();
    }
}

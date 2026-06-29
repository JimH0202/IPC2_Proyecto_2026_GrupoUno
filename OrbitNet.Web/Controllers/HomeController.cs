using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Models;
using OrbitNet.Web.Services;

namespace OrbitNet.Web.Controllers;

public class HomeController : Controller
{
    private readonly OrbitNetDataService _dataService;

    public HomeController(OrbitNetDataService dataService) 
    {
        _dataService = dataService;
    }

    public IActionResult Index()
    {
        var model = _dataService.GetDashboardViewModel();
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    [HttpPost]
    [HttpHead]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        LocalizationService.SetLanguage(Response, culture);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            var lowerReturnUrl = returnUrl.ToLowerInvariant();
            if (lowerReturnUrl.StartsWith("/simulation/executeticks") || lowerReturnUrl.StartsWith("/simulation/stopsimulation"))
            {
                return RedirectToAction("TickResult", "Simulation");
            }

            return Redirect(returnUrl);
        }

        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
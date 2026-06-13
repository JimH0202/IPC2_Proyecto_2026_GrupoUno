using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Models.ViewModels;

namespace OrbitNet.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var model = new DashboardViewModel
        {
            CurrentTick = 150,
            ActiveSatellites = 28,
            InactiveSatellites = 2,
            PendingMessages = 15,
            ProcessedMessages = 450,
            TotalAntennas = 6,
            Hemisphere = "Norte",
            IsSimulationRunning = true
        };

        return View(model);
    }
}
using System.Collections.Generic;

namespace OrbitNet.Web.Models.ViewModels;

public class DashboardViewModel
{
    public int CurrentTick { get; set; }

    public int ActiveSatellites { get; set; }

    public int InactiveSatellites { get; set; }

    public int PendingMessages { get; set; }

    public int ProcessedMessages { get; set; }

    public int TotalAntennas { get; set; }

    public string Hemisphere { get; set; } = string.Empty;

    public int Port { get; set; }

    public string RemoteHemisphereUrl { get; set; } = string.Empty;

    public bool IsSimulationRunning { get; set; }

    public List<SatelliteViewModel> Satellites { get; set; } = new();
}

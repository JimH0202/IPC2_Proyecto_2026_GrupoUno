using System.Collections.Generic;

namespace Orbinet.Web.Models.ViewModels;

public class UploadViewModel
{
    public string FileName { get; set; } = string.Empty;

    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public int SatellitesLoaded { get; set; }

    public int AntennasLoaded { get; set; }

    public int PolarOrbitsLoaded { get; set; }

    public List<string> Errors { get; set; } = new();
}

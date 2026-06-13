using System;

namespace OrbitNet.Web.Models.ViewModels;

public class ReportViewModel
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string SvgContent { get; set; } = string.Empty;

    public DateTime GeneratedAt { get; set; }
}

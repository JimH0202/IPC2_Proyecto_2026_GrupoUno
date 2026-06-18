namespace Orbinet.Web.Models.ViewModels;

public class MatrixViewModel
{
    public string SvgContent { get; set; } = string.Empty;

    public int Rows { get; set; }

    public int Columns { get; set; }

    public int OccupiedNodes { get; set; }
}

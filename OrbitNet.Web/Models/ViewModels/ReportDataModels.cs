namespace OrbitNet.Web.Models.ViewModels;

public class AvlReportNode
{
    public string Id { get; set; } = string.Empty;
    public int Value { get; set; }
    public int[] Children { get; set; } = Array.Empty<int>();
}

public class AvlReportData
{
    public string Title { get; set; } = string.Empty;
    public List<AvlReportNode> Nodes { get; set; } = new();
    public int TotalNodes { get; set; }
    public int Height { get; set; }
}

public class BufferReportItem
{
    public string Id { get; set; } = string.Empty;
    public string Satellite { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int Occupied { get; set; }
    public int OccupancyPercent { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class BufferReportData
{
    public List<BufferReportItem> Buffers { get; set; } = new();
    public int TotalBuffers { get; set; }
    public int AverageOccupancy { get; set; }
}

public class RouteReportItem
{
    public string Id { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public int Hops { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Packets { get; set; }
}

public class RouteReportData
{
    public List<RouteReportItem> Routes { get; set; } = new();
    public int TotalRoutes { get; set; }
    public int ActiveRoutes { get; set; }
}

public class SparseMatrixCell
{
    public int Row { get; set; }
    public int Column { get; set; }
    public string SatelliteId { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int Occupied { get; set; }
    public int OccupancyPercent { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Messages { get; set; } = string.Empty;
}

public class SparseMatrixReportData
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public List<SparseMatrixCell> Cells { get; set; } = new();
}

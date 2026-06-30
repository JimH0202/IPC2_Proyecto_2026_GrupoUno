namespace OrbitNet.Web.Models.ViewModels;

public class RelayDashboardViewModel
{
    public string Hemisphere { get; set; } = "North";
    public DateTime LastUpdated { get; set; } = DateTime.Now;
    public RelayStatusDto Status { get; set; } = new();
    public List<RouteDto> Routes { get; set; } = new();
    public List<BufferDto> Buffers { get; set; } = new();
    public List<EventDto> RecentEvents { get; set; } = new();
    public string? ActionMessage { get; set; }
    public string? ErrorMessage { get; set; }
}

public class RelayStatusDto
{
    public int ActiveRelays { get; set; }
    public int InactiveRelays { get; set; }
    public int TotalPacketsProcessed { get; set; }
    public double AvgQueueOccupancy { get; set; }
    public double NorthQueueOccupancy { get; set; }
    public double SouthQueueOccupancy { get; set; }
}

public class RouteDto
{
    public string FromSatellite { get; set; } = string.Empty;
    public string ToAntenna { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public int PacketCount { get; set; }
    public double QueueOccupancyPercentage { get; set; }
    public DateTime LastSeen { get; set; } = DateTime.Now;
    public string PacketData { get; set; } = string.Empty;
}

public class BufferDto
{
    public string BufferId { get; set; } = string.Empty;
    public string Type { get; set; } = "Priority";
    public int ItemsInQueue { get; set; }
    public double CapacityPercentage { get; set; }
}

public class EventDto
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Level { get; set; } = "Info";
    public string DisplayLevel { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

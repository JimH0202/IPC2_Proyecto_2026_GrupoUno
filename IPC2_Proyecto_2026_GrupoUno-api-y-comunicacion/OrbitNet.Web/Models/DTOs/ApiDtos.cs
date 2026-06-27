using System.Text;
using System.Text.Json.Serialization;
namespace OrbitNet.Web.Models.DTOs;

public class ConfigRequestDto
{
    [JsonPropertyName("xml_data")]
    public string XmlData { get; set; }  = string.Empty;
}

public class ConfigSuccessResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }  = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; }  = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }  = string.Empty;
}

public class ConfigErrorResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }  = string.Empty;

    [JsonPropertyName("error_code")]
    public string ErrorCode { get; set; }  = string.Empty;

    [JsonPropertyName("details")]
    public string Details { get; set; }  = string.Empty;
}

public class RelayRequestDto
{
    [JsonPropertyName("from_satellite")]
    public string FromSatellite { get; set; }

    [JsonPropertyName("to_antenna")]
    public string ToAntenna { get; set; }

    [JsonPropertyName("packet_data")]
    public string PacketData { get; set; }
}

public class RelaySuccessResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }  = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; }  = string.Empty;

    [JsonPropertyName("queue_occupancy_percentage")]
    public double QueueOccupancyPercentage { get; set; }
}

public class RelayErrorResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }  = string.Empty;

    [JsonPropertyName("details")]
    public string Details { get; set; }  = string.Empty;
}

public class SimulationStepRequestDto
{
    [JsonPropertyName("ticks")]
    public int Ticks { get; set; }
}

public class SimulationStepResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }  = string.Empty;

    [JsonPropertyName("current_tick")]
    public int CurrentTick { get; set; }

    [JsonPropertyName("events_processed")]
    public int EventsProcessed { get; set; }

    [JsonPropertyName("details")]
    public string Details { get; set; }  = string.Empty;
}

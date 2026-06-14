using System.Text;
using System.Text.Json.Serialization;

public class MessagePacket
{
    [JsonPropertyName("codigo_hex")]
    public string CodHex { get; set; }

    [JsonPropertyName("emisor_id")]
    public string SenderId { get; set; }

    [JsonPropertyName("destino_ip")]
    public string DestinationIp { get; set; }

    [JsonPropertyName("prioridad")]
    public int Priority { get; set; }

    [JsonPropertyName("contenido")]
    public string Content { get; set; }
}

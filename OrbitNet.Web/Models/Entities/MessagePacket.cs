using System.Text.Json.Serialization;
using OrbitNet.Web.Models.Enums;

namespace OrbitNet.Web.Models.Entities
{
    public class MessagePacket
    {
        [JsonPropertyName("codigo_hex")]
        public string CodHex { get; set; }

        [JsonPropertyName("emisor_id")]
        public string SenderId { get; set; }

        [JsonPropertyName("destino_ip")]
        public string DestinationIp { get; set; }

        [JsonPropertyName("prioridad")]
        public PriorityLevel Priority { get; set; }

        [JsonPropertyName("contenido")]
        public string Content { get; set; }

        public MessageStatus Status { get; set; }

        public int HopCount { get; set; }


        public MessagePacket()
        {
            CodHex = string.Empty;
            SenderId = string.Empty;
            DestinationIp = string.Empty;
            Content = string.Empty;
            Status = MessageStatus.EnEspera;
            HopCount = 0;
        }
    }
}

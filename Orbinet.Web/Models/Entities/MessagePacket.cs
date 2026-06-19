using System.Text;
using System.Text.Json.Serialization;
using Orbinet.Web.Models.Enums; // Para poder usar el Enum MessageStatus

namespace Orbinet.Web.Models.Entities
{
    /// Representa un paquete de datos que transita por la red espacial de OrbitNet.
    /// Mantiene la estructura de serialización JSON definida por el equipo.
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

        // Propiedad extra para el control del motor de simulación
        /// Estado actual del paquete dentro del ciclo de la simulación.
        /// No se serializa directamente desde el JSON de configuración inicial.
        public MessageStatus Estado { get; set; }

        /// Inicializa una nueva instancia de <see cref="MessagePacket"/> con un estado inicial.
        public MessagePacket()
        {
            Estado = MessageStatus.EnEspera; // Todo paquete inicia esperando ser transmitido
        }
    }
}

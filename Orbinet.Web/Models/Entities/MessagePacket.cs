using System.Text;
using System.Text.Json.Serialization;
using Orbinet.Web.Models.Enums; // Para poder usar el Enum MessageStatus

namespace Orbinet.Web.Models.Entities
{
    /// Representa un paquete de datos que transita por la red espacial de OrbitNet.
    /// Mantiene la estructura de serialización JSON definida por el equipo.
    public class MessagePacket
    {
        /// Código hexadecimal único que identifica al paquete.
        [JsonPropertyName("codigo_hex")]
        public string CodHex { get; set; }

        /// Identificador único del nodo emisor (origen).
        [JsonPropertyName("emisor_id")]
        public string SenderId { get; set; }

        /// Dirección IP del nodo destino final (satélite o antena terrestre).
        [JsonPropertyName("destino_ip")]
        public string DestinationIp { get; set; }

        /// Nivel de prioridad del paquete (Baja, Media, Alta) para el despacho en colas.
        [JsonPropertyName("prioridad")]
        public int Priority { get; set; }

        /// Cuerpo o contenido textual del mensaje transmitido.
        [JsonPropertyName("contenido")]
        public string Content { get; set; }

        // Propiedad extra para el control del motor de simulación
        /// Estado actual del paquete dentro del ciclo de la simulación.
        /// No se serializa directamente desde el JSON de configuración inicial.
        public MessageStatus Estado { get; set; }

        /// Cantidad de saltos o relevos que ha realizado el paquete entre nodos.
        /// Utilizado para el control del motor de simulación.
        public int HopCount { get; set; }

        /// Inicializa una nueva instancia de <see cref="MessagePacket"/> con un estado inicial.
        public MessagePacket()
        {
            Estado = MessageStatus.EnEspera; // Todo paquete inicia esperando ser transmitido
            HopCount = 0; // Inicialmente, el paquete no ha realizado ningún salto
        }
    }
}

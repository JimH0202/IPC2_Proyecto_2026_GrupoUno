using OrbitNet.Web.DataStructures.Interfaces;

namespace OrbitNet.Web.Models.Entities
{
    // Representa una estación o antena terrestre fija en la superficie de la Tierra.
    /// Actúa como origen y destino final de los paquetes de datos en la red OrbitNet.
    public class GroundAntenna{
        /// Identificador único de la antena terrestre (ej: "ANT-GT-01").
        public string Id { get; set; }
        /// Nombre descriptivo para mostrar en vista.
        public string Name { get; set; }
        /// Coordenadas geográficas formateadas en texto para representación visual.
        public string Coords { get; set; }
        /// Dirección IP única asignada a la antena para el direccionamiento y recepción de paquetes.
        public string Ip {get; set; }
       
        /// Ubicación angular fija de la antena sobre la superficie terrestre, medida en grados (0° a 360°).
        /// Utilizada matemáticamente por el motor de simulación para determinar la cobertura espacial.
        public double PosicionAngular { get; set; }

        /// Búfer estructurado que contiene los paquetes originados en tierra esperando transmisión espacial.
        public IMessageBuffer PaquetesEnEspera { get; set; }

        /// Búfer estructurado que almacena los paquetes que completaron su ciclo y fueron recibidos con éxito.
        public IMessageBuffer PaquetesRecibidos { get; set; }

    }
}

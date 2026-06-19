using Orbinet.Web.DataStructures.Interfaces;

namespace Orbinet.Web.Models.Entities
{
    /// Representa una estación o antena terrestre fija en la Tierra.
    /// Actúa como origen y destino final de los paquetes de datos.
    public class GroundAntenna{
        /// Atributos agregados por otro integrante
        public string Id { get; set; }
        public string Name { get; set; }
        public string Coords { get; set; }
         public string Ip {get; set; }
       
        // Atributo matemático para la simulación
        /// Ubicación angular fija de la antena sobre la superficie terrestre (0° a 360°).
        public double PosicionAngular { get; set; }

        /// Búfer estructurado que contiene los paquetes originados en tierra esperando transmisión espacial.
        public IMessageBuffer PaquetesEnEspera { get; set; }

        /// Búfer estructurado que almacena los paquetes que completaron su ciclo y fueron recibidos con éxito.
        public IMessageBuffer PaquetesRecibidos { get; set; }

    }
}

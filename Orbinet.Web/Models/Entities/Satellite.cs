using Orbinet.Web.DataStructures.Interfaces;

namespace Orbinet.Web.Models.Entities
{
  
    /// Representa un satélite individual dentro de la red de simulación OrbitNet.
    /// Almacena los datos de posición orbital y los paquetes de datos en tránsito.

    public class Satellite {
        /// Identificador único del satélite (ej: "Sat-Polar1-Node1").
        public string Id { get; set; }
        ///  Nombre elegible del satélite para propósitos de visualización
        public string Name { get; set; }
       /// Dirección IP única asignada al satélite para el enrutamiento de red.
        public string Ip { get; set; }
        
        /// Posición angular actual del satélite en su órbita, medida en grados (0° a 360°).
        public double AnomaliaOrbital { get; set; }
        
        // Búfer estructurado (Árbol ABB) que gestiona los mensajes almacenados a bordo.
        /// Su implementación interna dependerá de las estructuras del equipo.
        public IMessageBuffer PaquetesABordo { get; set; }
    }
}
using Orbinet.Web.DataStructures.Interfaces;

namespace Orbinet.Web.Models.Entities
{
    /// <Introducción>
    /// Representa un satélite individual dentro de la red de simulación OrbitNet.
    /// Almacena los datos de posición orbital y los paquetes de datos en tránsito.

    public class Satellite {
        /// Identificador único del satélite (ej: "Sat-Polar1-Node1").
        public string Id { get; set; }
        
        public string Name { get; set; }
       
        public string Ip { get; set; }
        
        /// Posición angular actual del satélite en su órbita, medida en grados (0° a 360°).
        public double AnomaliaOrbital { get; set; }
        
        // Búfer estructurado (Árbol ABB) que gestiona los mensajes almacenados a bordo.
        /// </summary>
        public IMessageBuffer PaquetesABordo { get; set; }
    }
}
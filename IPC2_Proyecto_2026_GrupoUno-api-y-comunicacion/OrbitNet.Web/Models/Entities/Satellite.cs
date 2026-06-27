using OrbitNet.Web.DataStructures.Buffer;
using OrbitNet.Web.DataStructures.Interfaces;
using OrbitNet.Web.Models.Enums;

namespace OrbitNet.Web.Models.Entities
{
  
    /// Representa un satélite individual dentro de la red de simulación OrbitNet.
    /// Almacena los datos de posición orbital y los paquetes de datos en tránsito.

    public class Satellite {
        //El constructor evita valores nulos, asegurando que que las propiedades tengan valores. 
        public Satellite()
        {
            Id = string.Empty;
            Name = string.Empty;
            Ip = string.Empty;
            OrbitalAngle = 0;
            OrbitType = OrbitType.Ecuatorial;
            PaquetesABordo = new BufferMensajes();
        }

        /// Identificador único del satélite (ej: "Sat-Polar1-Node1").
        public string Id { get; set; } = string.Empty; // El identificador se inicializa como cadena vacía para evitar nulls.
        ///  Nombre elegible del satélite para propósitos de visualización
        public string Name { get; set; } = string.Empty; // Evitamos nulls inicializando con cadena vacía.
       /// Dirección IP única asignada al satélite para el enrutamiento de red.
        public string Ip { get; set; } = string.Empty; // Evitamos nulls inicializando con cadena vacía.
        
        /// Posición angular actual del satélite en su órbita, medida en grados (0° a 360°).
        public double OrbitalAngle { get; set; }
        /// Tipo de órbita en la que se encuentra el satélite (Ecuatorial o Polar).
        /// Se utiliza para determinar la trayectoria y cobertura del satélite en la simulación.
        public OrbitType OrbitType { get; set; }
        
        // Búfer estructurado (Árbol ABB) que gestiona los mensajes almacenados a bordo.
        /// Su implementación interna dependerá de las estructuras del equipo.
        public IMessageBuffer PaquetesABordo { get; set; }
    }
}
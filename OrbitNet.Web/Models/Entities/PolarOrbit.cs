using OrbitNet.Web.Models; // Asegura la comunicación con la raíz de tus modelos

namespace OrbitNet.Web.Models.Entities
{
    /// Representa una órbita polar que agrupa y gestiona un conjunto de satélites.
    public class PolarOrbit{
        public string Id { get; set; }
        /// Arreglo de satélites que orbitan en esta trayectoria de simulación.
        public Satellite[] Satellites { get; set; }
    }
}
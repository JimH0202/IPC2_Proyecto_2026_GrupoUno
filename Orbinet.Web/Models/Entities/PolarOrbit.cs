using Orbinet.Web.Models; // Asegura la comunicación con la raíz de tus modelos
using Orbinet.Web.DataStructures.Matrix;


namespace Orbinet.Web.Models.Entities
{
    /// Representa una órbita polar que agrupa y gestiona un conjunto de satélites.
    /// Se mapea directamente con una fila indexada dentro de la matriz dispersa satelital.
    public class PolarOrbit{
        /// Identificador único de la órbita o número de fila en la matriz (ej: "1", "2").
        public string Id { get; set; }

        /// Nombre descriptivo o región de cobertura de la trayectoria orbital.
        public string Name { get; set; }

        /// Referencia al nodo de encabezado de fila (HeaderNode) en la matriz dispersa.
        /// Permite al motor de simulación acceder y recorrer horizontalmente los satélites de esta órbita.
        public HeaderNode EncabezadoMatriz { get; set; }
    }
}
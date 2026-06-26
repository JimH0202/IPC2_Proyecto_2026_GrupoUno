using Orbinet.Web.Models; // Asegura la comunicación con la raíz de tus modelos
using Orbinet.Web.DataStructures.Matrix;


namespace Orbinet.Web.Models.Entities
{
    /// Representa una órbita polar que agrupa y gestiona un conjunto de satélites.
    /// Se mapea directamente con una fila indexada dentro de la matriz dispersa satelital.
    public class PolarOrbit{
        //Inicialicé valores a cero para evitar errores de referencia. 
        public PolarOrbit()
        {
            Id = string.Empty;
            Name = string.Empty;
            MatrixRow = 0;
        }
        
        /// Identificador único de la órbita o número de fila en la matriz (ej: "1", "2").
        public string Id { get; set; }

        /// Nombre descriptivo o región de cobertura de la trayectoria orbital.
        public string Name { get; set; }

        /// Número de fila que ocupa esta órbita dentro de la matriz dispersa.
        /// El motor de simulación utilizará este índice para solicitar el
        /// HeaderNode correspondiente a RedSatelitalPlano.
        public int MatrixRow { get; set; }
    }
}
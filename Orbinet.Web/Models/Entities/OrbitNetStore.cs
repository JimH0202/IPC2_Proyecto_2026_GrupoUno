using Orbinet.Web.DataStructures.Matrix;
using Orbinet.Web.Services.SimulationEngine.State;
using Orbinet.Web.DataStructures.Antenas;
      
// NOTA: Si las antenas se guardan en el AVL, aquí se importa su namespace corporativo

namespace Orbinet.Web.Models.Entities
{
    /// Almacén central de estado y métricas globales para la simulación OrbitNet.
    /// Funciona como un Singleton en memoria para coordinar datos entre el motor y la interfaz visual.
    public class OrbitNetStore
    {
        // === Métricas de Rendimiento y Simulación ===
        
        /// Cantidad total de nodos (satélites/antenas) procesados durante la simulación.
        public int NodosProcesados { get; set; }
        /// Ciclo o Tick de tiempo actual en la ejecución del motor de simulación.
        public int CurrentTick { get; set; }
        /// Total de eventos de transmisión procesados con éxito.
        public int EventsProcessed { get; set; }
        /// Contador de saltos lógicos acumulados por todos los paquetes en la red.
        public int LogicalJumps { get; set; }
        /// Porcentaje promedio de ocupación de los buffers en la red espacial.
        public double QueueOccupancyPercentage { get; set; }
        
        // === Datos auxiliares de simulación ===

        /// Identificador del satélite receptor configurado para auditoría o pruebas.
        public string ReceptorSatelliteId { get; set; }
        /// Bandera de control que indica si el archivo de configuración XML fue cargado con éxito.
        public bool ConfigLoaded { get; set; }


        // === Estructuras de Datos Vivas del Sistema ===

        /// Referencia directa a la Matriz Dispersa que representa la red satelital física en el espacio.
        public RedSatelitalPlano RedSatellites { get; set; }
        /// Referencia a la estructura que almacena las antenas terrestres (se actualizará con el tipo AVL o Lista del Integrante 1).
        public ListaAntenas Antenas { get; set; }
        
        // Contador de satélites detectados en la última iteración de rotación orbital.
        public SatelliteStateIndex SatelliteStates { get; set; }

        public SatelliteRuntimeIndex SatelliteRuntime { get; set; }
        
        
        /// Inicializa una nueva instancia de <see cref="OrbitNetStore"/> con valores iniciales seguros.
        public OrbitNetStore()
        {
            NodosProcesados = 0;
            CurrentTick = 0;
            EventsProcessed = 0;
            LogicalJumps = 0;
            QueueOccupancyPercentage = 0.0;

            ReceptorSatelliteId = string.Empty;
            ConfigLoaded = false;

            // Instanciamos la matriz dispersa para que el motor no de errores de referencia nula al iniciar
            RedSatellites = new RedSatelitalPlano();
            Antenas = new ListaAntenas();
            SatelliteStates = new SatelliteStateIndex();
            SatelliteRuntime = new SatelliteRuntimeIndex();
        }
    }
}

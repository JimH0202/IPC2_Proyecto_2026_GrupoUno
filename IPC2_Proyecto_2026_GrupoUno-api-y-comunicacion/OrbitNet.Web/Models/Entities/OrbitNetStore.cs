using OrbitNet.Web.DataStructures.Matrix;
using OrbitNet.Web.Services.SimulationEngine.State;
using OrbitNet.Web.DataStructures.Antenas;
using OrbitNet.Web.DataStructures.Listas; // Estructura de Gustavo
using OrbitNet.Web.Models.Entities;

namespace OrbitNet.Web.Models.Entities
{
    /// <summary>
    /// Almacén central de estado y métricas globales para la simulación OrbitNet.
    /// Funciona como un Singleton en memoria para coordinar datos entre el motor y la interfaz visual.
    /// </summary>
    public class OrbitNetStore
    {
        // === Constantes del Sistema ===
        public const int CapacidadMaximaCola = 10;

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
        
        /// Cola de paquetes de mensajes administrada por la estructura de Gustavo.
        public ListaEnlazada<MessagePacket> PaquetesEnCola { get; } = new();

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="OrbitNetStore"/> con valores iniciales seguros.
        /// </summary>
        public OrbitNetStore()
        {
            NodosProcesados = 0;
            CurrentTick = 0;
            EventsProcessed = 0;
            LogicalJumps = 0;
            QueueOccupancyPercentage = 0.0;

            // Se adopta el ID por defecto sugerido por Gustavo
            ReceptorSatelliteId = "SAT-POL-1001";
            ConfigLoaded = false;

            // Instanciamos la matriz dispersa para que el motor no de errores de referencia nula al iniciar
            RedSatellites = new RedSatelitalPlano();
            Antenas = new ListaAntenas();
            SatelliteStates = new SatelliteStateIndex();
            SatelliteRuntime = new SatelliteRuntimeIndex();
        }

        // === Métodos de Gestión de Cola (Gustavo) ===

        public void EncolarPaquete(MessagePacket paquete)
        {
            PaquetesEnCola.Add(paquete);
            QueueOccupancyPercentage = CalcularOcupacionCola();
        }

        public double CalcularOcupacionCola()
        {
            if (PaquetesEnCola.Count == 0) return 0.0;
            return Math.Round((PaquetesEnCola.Count / (double)CapacidadMaximaCola) * 100.0, 1);
        }
    }
}

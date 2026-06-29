using OrbitNet.Web.DataStructures.Antenas;
using OrbitNet.Web.DataStructures.Listas;
using OrbitNet.Web.DataStructures.Logs;
using OrbitNet.Web.DataStructures.Matrix;
using OrbitNet.Web.Models.Entities;
using OrbitNet.Web.Models.ViewModels;
using OrbitNet.Web.Services.SimulationEngine.State;

public class OrbitNetStore
{
    public const int CapacidadMaximaCola = 10;

    public int NodosProcesados { get; set; }
    public int CurrentTick { get; set; }
    public int EventsProcessed { get; set; }
    public int LogicalJumps { get; set; }
    public double QueueOccupancyPercentage { get; set; }
    public string ReceptorSatelliteId { get; set; }
    public bool ConfigLoaded { get; set; }
    public bool IsSimulationRunning { get; set; }
    public ListaEnlazada<MessagePacket> PaquetesEnCola { get; } = new();

    public ListaEnlazada<RouteDto> Routes { get; set; } = new();
    public ListaEnlazada<BufferDto> Buffers { get; set; } = new();

    public ListaEnlazada<Satellite> Satellites { get; } = new();
    public ListaEnlazada<PolarOrbit> PolarOrbits { get; } = new();
    public ListaAntenas Antenas { get; set; }
    public RedSatelitalPlano RedSatellites { get; set; }
    public LogAuditoria LogAuditoria { get; } = new();

    public SatelliteStateIndex SatelliteStates { get; set; }
    public SatelliteRuntimeIndex SatelliteRuntime { get; set; }
    public OrbitNetStore()
    {
        QueueOccupancyPercentage = 40.0;
        ReceptorSatelliteId = "SAT-POL-1001";
        Antenas = new ListaAntenas();
        RedSatellites = new RedSatelitalPlano();
        SatelliteStates = new SatelliteStateIndex();
        SatelliteRuntime = new SatelliteRuntimeIndex();
    }

    public void EncolarPaquete(MessagePacket paquete)
    {
        PaquetesEnCola.Add(paquete);
        QueueOccupancyPercentage = CalcularOcupacionCola();
    }

    public double CalcularOcupacionCola()
    {
        return Math.Round((PaquetesEnCola.Count / (double)CapacidadMaximaCola) * 100.0, 1);
    }

    public void Clear()
    {
        PaquetesEnCola.Clear();
        Satellites.Clear();
        PolarOrbits.Clear();
        Antenas.Clear();
        RedSatellites.Clear();
        LogAuditoria.Clear();
        SatelliteStates.Clear();
        SatelliteRuntime.Clear();
        Routes.Clear();
        Buffers.Clear();
        NodosProcesados = 0;
        CurrentTick = 0;
        EventsProcessed = 0;
        LogicalJumps = 0;
        QueueOccupancyPercentage = 0;
        ConfigLoaded = false;
        IsSimulationRunning = false;
    }

    public int ActiveSatellites => Satellites.Count;
    public int TotalAntennas => Antenas.Count;
    public int PendingMessages => PaquetesEnCola.Count;
}

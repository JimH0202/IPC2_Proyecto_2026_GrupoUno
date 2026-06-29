using OrbitNet.Web.DataStructures.Antenas;
using OrbitNet.Web.DataStructures.Listas;
using OrbitNet.Web.DataStructures.Logs;
using OrbitNet.Web.DataStructures.Matrix;
using OrbitNet.Web.Models.Entities;

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

    public List<Satellite> Satellites { get; } = new();
    public List<PolarOrbit> PolarOrbits { get; } = new();
    public ListaAntenas Antennas { get; } = new();
    public RedSatelitalPlano Matrix { get; } = new();
    public LogAuditoria LogAuditoria { get; } = new();

    public OrbitNetStore()
    {
        ReceptorSatelliteId = "SAT-POL-1001";
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
        Antennas.Clear();
        Matrix.Clear();
        LogAuditoria.Clear();
        NodosProcesados = 0;
        CurrentTick = 0;
        EventsProcessed = 0;
        LogicalJumps = 0;
        QueueOccupancyPercentage = 0;
        ConfigLoaded = false;
        IsSimulationRunning = false;
    }

    public int ActiveSatellites => Satellites.Count;
    public int TotalAntennas => Antennas.Count;
    public int PendingMessages => PaquetesEnCola.Count;
}

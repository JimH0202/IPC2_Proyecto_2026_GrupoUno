using Orbinet.Web.DataStructures.Listas;
using Orbinet.Web.Models.Entities;

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
    public ListaEnlazada<MessagePacket> PaquetesEnCola { get; } = new();

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
}

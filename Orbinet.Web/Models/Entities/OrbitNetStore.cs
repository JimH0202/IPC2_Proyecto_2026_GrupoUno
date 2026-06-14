public class OrbitNetStore
{
    public int NodosProcesados { get; set; }
    public int CurrentTick { get; set; }
    public int EventsProcessed { get; set; }
    public int LogicalJumps { get; set; }
    public double QueueOccupancyPercentage { get; set; }
    public string ReceptorSatelliteId { get; set; }
    public bool ConfigLoaded { get; set; }

    public OrbitNetStore()
    {
        QueueOccupancyPercentage = 40.0;
        ReceptorSatelliteId = "SAT-POL-1001";
    }
}

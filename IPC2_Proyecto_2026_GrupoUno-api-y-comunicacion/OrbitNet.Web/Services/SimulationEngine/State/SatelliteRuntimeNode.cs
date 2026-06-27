using OrbitNet.Web.Models.Entities;

namespace OrbitNet.Web.Services.SimulationEngine.State
{
    public class SatelliteRuntimeNode
    {
        public string SatelliteId { get; set; }
        public Satellite SatelliteRef { get; set; }
        public SatelliteRuntimeNode? Next { get; set; }

        public SatelliteRuntimeNode()
        {
            SatelliteId = string.Empty;
            SatelliteRef = new Satellite();
            Next = null;
        }
    }
}
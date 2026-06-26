using Orbinet.Web.Models.Enums;

namespace Orbinet.Web.Services.SimulationEngine.State
{
    public class SatelliteStateNode
    {
        public string SatelliteId { get; set; }
        public string IpAddress { get; set; }

        public OrbitType OrbitType { get; set; }
        public double OrbitalAngle { get; set; }

        public int CurrentRow { get; set; }
        public int CurrentColumn { get; set; }

        public SatelliteStateNode? Next { get; set; }

        public SatelliteStateNode()
        {
            SatelliteId = string.Empty;
            IpAddress = string.Empty;
            OrbitType = OrbitType.Ecuatorial;
            OrbitalAngle = 0;
            CurrentRow = 0;
            CurrentColumn = 0;
            Next = null;
        }
    }
}
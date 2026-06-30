namespace OrbitNet.Web.Services.SimulationEngine
{
    public class OrbitalRotationResult
    {
        public int SatellitesDetected { get; set; }
        public int RotatedSuccessfully { get; set; }
        public int SkippedByCollision { get; set; }
        public int UnchangedPositions { get; set; }
        public string Details { get; set; }

        public OrbitalRotationResult()
        {
            Details = string.Empty;
        }
    }
}

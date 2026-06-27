namespace OrbitNet.Web.Services.SimulationEngine
{
    public class SimulationCoordinatorResult
    {
        public int SatellitesDetected { get; set; }
        public int RotatedSuccessfully { get; set; }
        public int SkippedByCollision { get; set; }
        public int UnchangedPositions { get; set; }

        public int SatellitesVisited { get; set; }
        public int BuffersWithMessages { get; set; }
        public int MessagesDispatched { get; set; }
        public int LocalDeliveriesDetected { get; set; }
        public int CrossPortCandidates { get; set; }
        public int MissingSatelliteRuntime { get; set; }
        public int EmptyBuffers { get; set; }

        public int MessagesDeliveredToAntennas { get; set; }

        public string Details { get; set; }

        public SimulationCoordinatorResult()
        {
            Details = string.Empty;
        }
    }
}
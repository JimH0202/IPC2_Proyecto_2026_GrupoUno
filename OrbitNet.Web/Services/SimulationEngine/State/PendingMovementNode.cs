namespace OrbitNet.Web.Services.SimulationEngine.State
{
    public class PendingMovementNode
    {
        public string SatelliteId { get; set; }
        public string IpAddress { get; set; }

        public int SourceRow { get; set; }
        public int SourceColumn { get; set; }

        public int TargetRow { get; set; }
        public int TargetColumn { get; set; }

        public bool IsValid { get; set; }

        public double NextAngle { get; set; }

        public SatelliteStateNode? StateRef { get; set; }
        public PendingMovementNode? Next { get; set; }

        public PendingMovementNode()
        {
            SatelliteId = string.Empty;
            IpAddress = string.Empty;
            IsValid = true;
            NextAngle = 0;
            Next = null;
        }
    }
}

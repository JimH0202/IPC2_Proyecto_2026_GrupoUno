namespace Orbinet.Web.Services.SimulationEngine
{
    public class DispatchResult
    {
        public int SatellitesVisited { get; set; }
        public int BuffersWithMessages { get; set; }
        public int MessagesDispatched { get; set; }
        public int LocalDeliveries { get; set; }
        public int CrossPortCandidates { get; set; }
        public int MissingSatelliteRuntime { get; set; }
        public int EmptyBuffers { get; set; }
        public string Details { get; set; }

        public DispatchResult()
        {
            
            Details = string.Empty;
        }
    }
}
using OrbitNet.Web.Models.Enums;

namespace OrbitNet.Web.Services.SimulationEngine.State
{
    public class SatelliteStateIndex
    {
        private SatelliteStateNode? head;
        private int count;

        public int Count => count;
        public bool IsEmpty => count == 0;

        public void Clear()
        {
            head = null;
            count = 0;
        }

        public SatelliteStateNode? FindBySatelliteId(string satelliteId)
        {
            SatelliteStateNode? current = head;

            while (current != null)
            {
                if (current.SatelliteId == satelliteId)
                {
                    return current;
                }

                current = current.Next;
            }

            return null;
        }

        public SatelliteStateNode GetOrCreate(
            string satelliteId,
            string ipAddress,
            OrbitType orbitType,
            int currentRow,
            int currentColumn)
        {
            SatelliteStateNode? existing = FindBySatelliteId(satelliteId);

            if (existing != null)
            {
                return existing;
            }

            SatelliteStateNode newNode = new SatelliteStateNode
            {
                SatelliteId = satelliteId,
                IpAddress = ipAddress,
                OrbitType = orbitType,
                CurrentRow = currentRow,
                CurrentColumn = currentColumn,
                OrbitalAngle = 0
            };

            if (head == null)
            {
                head = newNode;
            }
            else
            {
                SatelliteStateNode current = head;
                while (current.Next != null)
                {
                    current = current.Next;
                }

                current.Next = newNode;
            }

            count++;
            return newNode;
        }
    }
}

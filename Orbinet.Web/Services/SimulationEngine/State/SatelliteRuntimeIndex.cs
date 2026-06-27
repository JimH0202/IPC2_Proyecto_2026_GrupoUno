using Orbinet.Web.Models.Entities;

namespace Orbinet.Web.Services.SimulationEngine.State
{
    public class SatelliteRuntimeIndex
    {
        private SatelliteRuntimeNode? head;
        private int count;

        public int Count => count;
        public bool IsEmpty => count == 0;

        public void Clear()
        {
            head = null;
            count = 0;
        }

        public bool Register(Satellite satellite)
        {
            if (satellite == null || string.IsNullOrWhiteSpace(satellite.Id))
            {
                return false;
            }

            SatelliteRuntimeNode? existing = FindNode(satellite.Id);
            if (existing != null)
            {
                existing.SatelliteRef = satellite;
                return true;
            }

            SatelliteRuntimeNode newNode = new SatelliteRuntimeNode
            {
                SatelliteId = satellite.Id,
                SatelliteRef = satellite
            };

            if (head == null)
            {
                head = newNode;
            }
            else
            {
                SatelliteRuntimeNode current = head;
                while (current.Next != null)
                {
                    current = current.Next;
                }

                current.Next = newNode;
            }

            count++;
            return true;
        }

        public Satellite? FindSatellite(string satelliteId)
        {
            SatelliteRuntimeNode? node = FindNode(satelliteId);
            return node?.SatelliteRef;
        }

        private SatelliteRuntimeNode? FindNode(string satelliteId)
        {
            SatelliteRuntimeNode? current = head;

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
    }
}
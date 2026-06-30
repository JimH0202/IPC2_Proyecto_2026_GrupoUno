using OrbitNet.Web.Models.Entities;
using OrbitNet.Web.Models.Enums;

namespace OrbitNet.Web.Services.SimulationEngine
{
    public class RoutingService
    {
        private readonly OrbitNetStore _store;

        public RoutingService(OrbitNetStore store)
        {
            _store = store;
        }

        public bool TryRouteMessage(MessagePacket packet)
        {
            if (packet == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(packet.DestinationIp))
            {
                packet.Status = MessageStatus.Fallido;
                return false;
            }

            GroundAntenna? antenna = _store.Antenas.SearchByIp(packet.DestinationIp);

            if (antenna != null)
            {
                packet.Status = MessageStatus.Entregado;

                antenna.PaquetesRecibidos.Agregar(packet);

                return true;
            }

            packet.Status = MessageStatus.Reenviado;
            return false;
        }
    }
}

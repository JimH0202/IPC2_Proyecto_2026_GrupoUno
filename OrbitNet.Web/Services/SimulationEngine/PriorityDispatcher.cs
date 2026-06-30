using OrbitNet.Web.DataStructures.Matrix;
using OrbitNet.Web.Models.Entities;
using OrbitNet.Web.Models.Enums;

namespace OrbitNet.Web.Services.SimulationEngine
{
    public class PriorityDispatcher
    {
        private readonly OrbitNetStore _store;

        private readonly RoutingService _routingService;

        public PriorityDispatcher(OrbitNetStore store, RoutingService routingService)
        {
            _store = store;
            _routingService = routingService;
        }

        public DispatchResult DispatchOneMessagePerSatelliteTick()
        {
            DispatchResult result = new DispatchResult();

            HeaderNode? rowHeader = _store.RedSatellites.GetFirstRowHeader();

            while (rowHeader != null)
            {
                MatrixNode? current = rowHeader.Access;

                while (current != null)
                {
                    result.SatellitesVisited++;

                    Satellite? satellite = _store.SatelliteRuntime.FindSatellite(current.SatelliteId);

                    if (satellite == null)
                    {
                        result.MissingSatelliteRuntime++;
                        current = current.Right;
                        continue;
                    }

                    var paquetesABordo = satellite.PaquetesABordo;

                    if (paquetesABordo is null || paquetesABordo.IsEmpty)
                    {
                        result.EmptyBuffers++;
                        current = current.Right;
                        continue;
                    }

                    result.BuffersWithMessages++;

                    MessagePacket? packet = satellite.PaquetesABordo.ObtenerSiguiente();

                    if (packet == null)
                    {
                        current = current.Right;
                        continue;
                    }

                    packet.Status = MessageStatus.EnTransito;
                    packet.HopCount++;

                    result.MessagesDispatched++;

                    bool fueEntregaLocal = _routingService.TryRouteMessage(packet);

                    if (fueEntregaLocal)
                    {
                        result.LocalDeliveries++;
                    }
                    else
                    {
                        result.CrossPortCandidates++;
                    }

                    current = current.Right;
                }

                rowHeader = rowHeader.Next;
            }

            result.Details =
                "Despacho prioritario completado. " +
                "Satélites visitados: " + result.SatellitesVisited +
                ", buffers con mensajes: " + result.BuffersWithMessages +
                ", mensajes despachados: " + result.MessagesDispatched +
                ", entregas locales: " + result.LocalDeliveries +
                ", candidatos cross-port: " + result.CrossPortCandidates +
                ", satélites sin runtime: " + result.MissingSatelliteRuntime +
                ", buffers vacíos: " + result.EmptyBuffers + ".";

            return result;
        }
    }
}

using Orbinet.Web.DataStructures.Matrix;
using Orbinet.Web.Models.Entities;
using Orbinet.Web.Models.Enums;

namespace Orbinet.Web.Services.SimulationEngine
{
    public class PriorityDispatcher
    {
        private readonly OrbitNetStore _store;

        public PriorityDispatcher(OrbitNetStore store)
        {
            _store = store;
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

                    if (satellite.PaquetesABordo == null || satellite.PaquetesABordo.IsEmpty)
                    {
                        result.EmptyBuffers++;
                        current = current.Right;
                        continue;
                    }

                    result.BuffersWithMessages++;

                    MessagePacket? packet = satellite.PaquetesABordo.ObtenerSiguiente();

                    if (packet != null)
                    {
                        packet.Status = MessageStatus.EnTransito;
                        packet.HopCount++;

                        result.MessagesDispatched++;

                        if (EsEntregaLocal(packet.DestinationIp))
                        {
                            packet.Status = MessageStatus.Entregado;
                            result.LocalDeliveries++;
                        }
                        else
                        {
                            result.CrossPortCandidates++;
                        }
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
                ", satélites sin runtime: " + result.MissingSatelliteRuntime + ".";

            return result;
        }

        private bool EsEntregaLocal(string destinationIp)
        {
            if (string.IsNullOrWhiteSpace(destinationIp))
            {
                return false;
            }

            GroundAntenna? antenna = _store.Antenas.SearchByIp(destinationIp);
            return antenna != null;
        }
    }
}
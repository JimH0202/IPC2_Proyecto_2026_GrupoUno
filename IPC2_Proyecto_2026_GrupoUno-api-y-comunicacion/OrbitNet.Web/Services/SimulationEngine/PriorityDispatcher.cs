using OrbitNet.Web.DataStructures.Matrix;
using OrbitNet.Web.Models.Entities;
using OrbitNet.Web.Models.Enums;

namespace OrbitNet.Web.Services.SimulationEngine
{
    public class PriorityDispatcher
    {
        // Almacén central del sistema en RAM.
        private readonly OrbitNetStore _store;

        // Servicio encargado de decidir si el paquete
        // se entrega localmente o queda marcado para reenvío.
        private readonly RoutingService _routingService;

        public PriorityDispatcher(OrbitNetStore store, RoutingService routingService)
        {
            _store = store;
            _routingService = routingService;
        }

        // Recorre todos los satélites de la matriz
        // y despacha un mensaje por satélite en este tick.
        public DispatchResult DispatchOneMessagePerSatelliteTick()
        {
            DispatchResult result = new DispatchResult();

            // Obtenemos el primer encabezado de fila
            // para recorrer la matriz horizontalmente.
            HeaderNode? rowHeader = _store.RedSatellites.GetFirstRowHeader();

            while (rowHeader != null)
            {
                MatrixNode? current = rowHeader.Access;

                while (current != null)
                {
                    // Contamos que este satélite fue visitado.
                    result.SatellitesVisited++;

                    // Buscamos el satélite real en el índice runtime.
                    Satellite? satellite = _store.SatelliteRuntime.FindSatellite(current.SatelliteId);

                    // Si no existe el satélite en runtime,
                    // no podemos acceder a su buffer real.
                    if (satellite == null)
                    {
                        result.MissingSatelliteRuntime++;
                        current = current.Right;
                        continue;
                    }

                    // Si el buffer está vacío, contamos el caso y seguimos.
                    if (satellite.PaquetesABordo.IsEmpty)
                    {
                        result.EmptyBuffers++;
                        current = current.Right;
                        continue;
                    }

                    // Este satélite sí tenía mensajes.
                    result.BuffersWithMessages++;

                    // Extraemos solo un mensaje por tick.
                    MessagePacket? packet = satellite.PaquetesABordo.ObtenerSiguiente();

                    // Si por alguna razón no salió paquete, seguimos.
                    if (packet == null)
                    {
                        current = current.Right;
                        continue;
                    }

                    // El paquete ya salió del buffer y entra a tránsito.
                    packet.Status = MessageStatus.EnTransito;
                    packet.HopCount++;

                    // Contabilizamos el despacho realizado.
                    result.MessagesDispatched++;

                    // Aquí es donde RoutingService entra al ciclo real.
                    bool fueEntregaLocal = _routingService.TryRouteMessage(packet);

                    if (fueEntregaLocal)
                    {
                        // El mensaje fue entregado localmente a una antena.
                        result.LocalDeliveries++;
                    }
                    else
                    {
                        // El mensaje no era local y queda como candidato
                        // para relay entre hemisferios.
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
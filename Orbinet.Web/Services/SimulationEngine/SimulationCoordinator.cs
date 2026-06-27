using Orbinet.Web.DataStructures.Matrix;
using Orbinet.Web.Models.Entities;

namespace Orbinet.Web.Services.SimulationEngine
{
    public class SimulationCoordinator
    {
        // Referencia al estado global en RAM.
        private readonly OrbitNetStore _store;

        // Servicio que rota satélites según su ángulo orbital.
        private readonly OrbitalRotator _orbitalRotator;

        // Servicio que saca un mensaje por satélite en cada tick.
        private readonly PriorityDispatcher _priorityDispatcher;

        // Servicio que decide si el mensaje se entrega localmente
        // o queda marcado para reenviarse.
        private readonly RoutingService _routingService;

        public SimulationCoordinator(
            OrbitNetStore store,
            OrbitalRotator orbitalRotator,
            PriorityDispatcher priorityDispatcher,
            RoutingService routingService)
        {
            _store = store;
            _orbitalRotator = orbitalRotator;
            _priorityDispatcher = priorityDispatcher;
            _routingService = routingService;
        }

        // Ejecuta un tick completo de simulación.
        // Devuelve un resumen con métricas útiles para el TickProcessor.
        public SimulationCoordinatorResult ExecuteSingleTick()
        {
            SimulationCoordinatorResult result = new SimulationCoordinatorResult();

            // 1. Rotar la red satelital.
            OrbitalRotationResult rotationResult = _orbitalRotator.RotateOneTick();

            result.SatellitesDetected = rotationResult.SatellitesDetected;
            result.RotatedSuccessfully = rotationResult.RotatedSuccessfully;
            result.SkippedByCollision = rotationResult.SkippedByCollision;
            result.UnchangedPositions = rotationResult.UnchangedPositions;

            // 2. Despachar un mensaje por satélite.
            DispatchResult dispatchResult = _priorityDispatcher.DispatchOneMessagePerSatelliteTick();

            result.SatellitesVisited = dispatchResult.SatellitesVisited;
            result.BuffersWithMessages = dispatchResult.BuffersWithMessages;
            result.MessagesDispatched = dispatchResult.MessagesDispatched;
            result.LocalDeliveriesDetected = dispatchResult.LocalDeliveries;
            result.CrossPortCandidates = dispatchResult.CrossPortCandidates;
            result.MissingSatelliteRuntime = dispatchResult.MissingSatelliteRuntime;
            result.EmptyBuffers = dispatchResult.EmptyBuffers;

            // 3. Recorrer antenas locales para contar mensajes recibidos en este estado actual.
            // Aquí no estamos "moviendo" nada extra; solo consolidamos una métrica.
            result.MessagesDeliveredToAntennas = CountDeliveredMessagesInAntennas();

            // 4. Actualizar métricas globales del store.
            _store.NodosProcesados += result.SatellitesVisited;
            _store.EventsProcessed += result.MessagesDispatched;
            _store.LogicalJumps += result.MessagesDispatched;

            // Si hubo satélites visitados, calculamos ocupación simple.
            if (result.SatellitesVisited > 0)
            {
                _store.QueueOccupancyPercentage =
                    (double)result.BuffersWithMessages / result.SatellitesVisited * 100.0;
            }
            else
            {
                _store.QueueOccupancyPercentage = 0.0;
            }

            result.Details =
                "Tick coordinado correctamente. " +
                "Rotados: " + result.RotatedSuccessfully +
                ", mensajes despachados: " + result.MessagesDispatched +
                ", entregas locales detectadas: " + result.LocalDeliveriesDetected +
                ", candidatos cross-port: " + result.CrossPortCandidates + ".";

            return result;
        }

        // Método auxiliar para contar mensajes actualmente entregados
        // dentro de las antenas locales.
        private int CountDeliveredMessagesInAntennas()
        {
            int total = 0;

            // Como ListaAntenas no expone recorrido público completo,
            // aquí por ahora no podemos iterarla directamente.
            // Dejamos el contador en cero hasta que exista un método de recorrido.
            // Esto evita acoplar mal el coordinador.

            return total;
        }
    }
}
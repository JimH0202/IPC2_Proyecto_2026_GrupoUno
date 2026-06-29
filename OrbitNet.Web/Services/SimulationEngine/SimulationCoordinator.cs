using OrbitNet.Web.DataStructures.Matrix;
using OrbitNet.Web.Models.Entities;

namespace OrbitNet.Web.Services.SimulationEngine
{
    public class SimulationCoordinator
    {
        private readonly OrbitNetStore _store;

        private readonly OrbitalRotator _orbitalRotator;

        private readonly PriorityDispatcher _priorityDispatcher;

        public SimulationCoordinator(
            OrbitNetStore store,
            OrbitalRotator orbitalRotator,
            PriorityDispatcher priorityDispatcher)
        {
            _store = store;
            _orbitalRotator = orbitalRotator;
            _priorityDispatcher = priorityDispatcher;
        }

        public SimulationCoordinatorResult ExecuteSingleTick()
        {
            SimulationCoordinatorResult result = new SimulationCoordinatorResult();

            OrbitalRotationResult rotationResult = _orbitalRotator.RotateOneTick();

            result.SatellitesDetected = rotationResult.SatellitesDetected;
            result.RotatedSuccessfully = rotationResult.RotatedSuccessfully;
            result.SkippedByCollision = rotationResult.SkippedByCollision;
            result.UnchangedPositions = rotationResult.UnchangedPositions;

            DispatchResult dispatchResult = _priorityDispatcher.DispatchOneMessagePerSatelliteTick();

            result.SatellitesVisited = dispatchResult.SatellitesVisited;
            result.BuffersWithMessages = dispatchResult.BuffersWithMessages;
            result.MessagesDispatched = dispatchResult.MessagesDispatched;
            result.LocalDeliveriesDetected = dispatchResult.LocalDeliveries;
            result.CrossPortCandidates = dispatchResult.CrossPortCandidates;
            result.MissingSatelliteRuntime = dispatchResult.MissingSatelliteRuntime;
            result.EmptyBuffers = dispatchResult.EmptyBuffers;

            result.MessagesDeliveredToAntennas = CountDeliveredMessagesInAntennas();

            _store.NodosProcesados += result.SatellitesVisited;

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

        private int CountDeliveredMessagesInAntennas()
        {
            int total = 0;

            for (int i = 0; i < _store.Antenas.Count; i++)
            {
                var antenna = _store.Antenas.GetAt(i);
                if (antenna?.PaquetesRecibidos != null)
                {
                    total += antenna.PaquetesRecibidos.Count;
                }
            }

            return total;
        }
    }
}

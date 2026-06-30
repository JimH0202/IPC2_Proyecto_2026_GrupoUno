using Microsoft.Extensions.Options;
using OrbitNet.Web.Configuration;
using OrbitNet.Web.Models.Entities;
using OrbitNet.Web.Models.Enums;
using OrbitNet.Web.Services.Communication;

namespace OrbitNet.Web.Services.SimulationEngine
{
    public class TickProcessor
    {
        private readonly OrbitNetStore _store;

        private readonly RelayHttpService _relayHttpService;

        private readonly AppInstanceSettings _settings;

        private readonly SimulationCoordinator _simulationCoordinator;

        public TickProcessor(
            OrbitNetStore store,
            RelayHttpService relayHttpService,
            IOptions<AppInstanceSettings> settings,
            SimulationCoordinator simulationCoordinator)
        {
            _store = store;
            _relayHttpService = relayHttpService;
            _settings = settings.Value;
            _simulationCoordinator = simulationCoordinator;
        }

        public SimulationStepResponse AvanzarSimulacion(int ticks)
        {
            if (ticks < 1)
            {
                ticks = 1;
            }

            int eventosProcesadosEnEstePaso = 0;
            int saltosLogicosEnEstePaso = 0;
            int candidatosCrossPortEnEstePaso = 0;

            for (int i = 0; i < ticks; i++)
            {
                _store.CurrentTick++;

                SimulationCoordinatorResult result = _simulationCoordinator.ExecuteSingleTick();

                eventosProcesadosEnEstePaso += result.MessagesDispatched;
                saltosLogicosEnEstePaso += result.MessagesDispatched;
                candidatosCrossPortEnEstePaso += result.CrossPortCandidates;
            }

            _store.EventsProcessed += eventosProcesadosEnEstePaso;
            _store.LogicalJumps += saltosLogicosEnEstePaso;

            return new SimulationStepResponse
            {
                Status = "Simulated",
                CurrentTick = _store.CurrentTick,
                EventsProcessed = _store.EventsProcessed,
                Details =
                    "Simulación completada correctamente. " +
                    "Ticks ejecutados: " + ticks +
                    ", eventos procesados en este paso: " + eventosProcesadosEnEstePaso +
                    ", candidatos cross-port detectados: " + candidatosCrossPortEnEstePaso + "."
            };
        }

        public async Task<bool> IntentarRelayCrossPortAsync(MessagePacket paquete)
        {
            if (paquete == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(paquete.DestinationIp))
            {
                paquete.Status = MessageStatus.Fallido;
                return false;
            }

            bool esDestinoSur = EsIpDelHemisferioSur(paquete.DestinationIp);
            bool esDestinoNorte = EsIpDelHemisferioNorte(paquete.DestinationIp);

            if (_settings.Hemisphere == "North" && esDestinoSur)
            {
                paquete.Status = MessageStatus.Reenviado;
                return await _relayHttpService.EnviarPaqueteAlHemisferioHermanoAsync(paquete);
            }

            if (_settings.Hemisphere == "South" && esDestinoNorte)
            {
                paquete.Status = MessageStatus.Reenviado;
                return await _relayHttpService.EnviarPaqueteAlHemisferioHermanoAsync(paquete);
            }

            return false;
        }

        public bool RequiereRelayCrossPort(MessagePacket paquete)
        {
            if (paquete == null || string.IsNullOrWhiteSpace(paquete.DestinationIp))
                return false;

            bool esDestinoSur = EsIpDelHemisferioSur(paquete.DestinationIp);
            bool esDestinoNorte = EsIpDelHemisferioNorte(paquete.DestinationIp);

            return (_settings.Hemisphere == "North" && esDestinoSur)
                || (_settings.Hemisphere == "South" && esDestinoNorte);
        }

        private bool EsIpDelHemisferioSur(string destinationIp)
        {
            return destinationIp == "10.0.0.90";
        }

        private bool EsIpDelHemisferioNorte(string destinationIp)
        {
            return destinationIp == "10.0.0.50";
        }
    }
}

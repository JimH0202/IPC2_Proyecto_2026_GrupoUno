using Microsoft.Extensions.Options;
using Orbinet.Web.Configuration;                  // Para encontrar AppInstanceSettings
using Orbinet.Web.Services.Communication;          // Para encontrar RelayHttpService
using Orbinet.Web.Models.Entities;                 // Para encontrar MessagePacket

namespace Orbinet.Web.Services.SimulationEngine
{
    public class TickProcessor
    {
        private readonly OrbitNetStore _store;
        private readonly RelayHttpService _relayHttpService;
        private readonly AppInstanceSettings _settings;

        public TickProcessor(
            OrbitNetStore store,
            RelayHttpService relayHttpService,
            IOptions<AppInstanceSettings> settings)
        {
            _store = store;
            _relayHttpService = relayHttpService;
            _settings = settings.Value;
        }

        public SimulationStepResponse AvanzarSimulacion(int ticks)
        {
            if (ticks < 1)
            {
                ticks = 1;
            }

            _store.CurrentTick += ticks;
            _store.EventsProcessed += ticks;
            _store.LogicalJumps += ticks / 2;

            return new SimulationStepResponse
            {
                Status = "Simulated",
                CurrentTick = _store.CurrentTick,
                EventsProcessed = _store.EventsProcessed,
                Details = "Orbitas rotadas exitosamente. Se ejecutaron " + _store.LogicalJumps + " saltos logicos."
            };
        }

        public bool RequiereRelayCrossPort(MessagePacket paquete)
        {
            bool esDestinoSur = paquete.DestinationIp == "10.0.0.90";
            bool esDestinoNorte = paquete.DestinationIp == "10.0.0.50";

            return (_settings.Hemisphere == "North" && esDestinoSur)
                || (_settings.Hemisphere == "South" && esDestinoNorte);
        }

        public async Task<bool> IntentarRelayCrossPortAsync(MessagePacket paquete)
        {
            bool esDestinoSur = paquete.DestinationIp == "10.0.0.90";
            bool esDestinoNorte = paquete.DestinationIp == "10.0.0.50";

            if (_settings.Hemisphere == "North" && esDestinoSur)
            {
                return await _relayHttpService.EnviarPaqueteAlHemisferioHermanoAsync(paquete);
            }

            if (_settings.Hemisphere == "South" && esDestinoNorte)
            {
                return await _relayHttpService.EnviarPaqueteAlHemisferioHermanoAsync(paquete);
            }

            return false;
        }
    }
}
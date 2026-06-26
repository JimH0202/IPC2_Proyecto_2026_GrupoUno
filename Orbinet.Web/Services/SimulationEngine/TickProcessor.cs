using Microsoft.Extensions.Options;
using Orbinet.Web.Configuration;            // Para encontrar AppInstanceSettings
using Orbinet.Web.Services.Communication;          // Para encontrar RelayHttpService
using Orbinet.Web.Models.Entities; // Para encontrar MessagePacket


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

        // Este método avanza la simulación en un número específico de ticks. ya no tiene el incremente artificial.
        public SimulationStepResponse AvanzarSimulacion(int ticks)
        {
            if (ticks < 1)
            {
                ticks = 1;
            }

            int eventosProcesadosEnEstePaso = 0;
            int saltosLogicosEnEstePaso = 0;

            for (int i = 0; i < ticks; i++)
            {
                _store.CurrentTick++;

                // Aquí después voy a a invocar:
                // 1. OrbitalRotator
                // 2. PriorityDispatcher
                // 3. RoutingService

                // Por ahora solo avanza el tick real.
                // Las demás métricas se actualizarán cuando exista la lógica real.
            }

            _store.EventsProcessed += eventosProcesadosEnEstePaso;
            _store.LogicalJumps += saltosLogicosEnEstePaso;

            return new SimulationStepResponse
            {
                Status = "Simulated",
                CurrentTick = _store.CurrentTick,
                EventsProcessed = _store.EventsProcessed,
                Details = $"Simulación avanzó {ticks} tick(s) correctamente."
            };
        }

        // Este método intenta reenviar un paquete a través del puerto cruzado si es necesario. 
        public async Task<bool> IntentarRelayCrossPortAsync(MessagePacket paquete)
        {
            if (paquete == null)
            {
                return false;
            }

            bool esDestinoSur = EsIpDelHemisferioSur(paquete.DestinationIp);
            bool esDestinoNorte = EsIpDelHemisferioNorte(paquete.DestinationIp);

            if (_settings.Hemisphere == "North" && esDestinoSur)
            {
                paquete.Status = Orbinet.Web.Models.Enums.MessageStatus.Reenviado;
                return await _relayHttpService.EnviarPaqueteAlHemisferioHermanoAsync(paquete);
            }

            if (_settings.Hemisphere == "South" && esDestinoNorte)
            {
                paquete.Status = Orbinet.Web.Models.Enums.MessageStatus.Reenviado;
                return await _relayHttpService.EnviarPaqueteAlHemisferioHermanoAsync(paquete);
            }

            return false;
        }
        // Este método verifica si una IP pertenece al hemisferio sur.
        private bool EsIpDelHemisferioSur(string destinationIp)
        {
            return destinationIp == "10.0.0.90";
        }

        // Este método verifica si una IP pertenece al hemisferio norte.
        private bool EsIpDelHemisferioNorte(string destinationIp)
        {
            return destinationIp == "10.0.0.50";
        }



    }
}

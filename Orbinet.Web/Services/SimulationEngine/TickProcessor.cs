using Microsoft.Extensions.Options;
using Orbinet.Web.Configuration;
using Orbinet.Web.Models.DTOs;
using Orbinet.Web.Models.Entities;
using Orbinet.Web.Models.Enums;
using Orbinet.Web.Services.Communication;

namespace Orbinet.Web.Services.SimulationEngine
{
    public class TickProcessor
    {
        // Store global del sistema en RAM.
        private readonly OrbitNetStore _store;

        // Servicio de comunicación entre hemisferios.
        private readonly RelayHttpService _relayHttpService;

        // Configuración de la instancia actual (North / South).
        private readonly AppInstanceSettings _settings;

        // Coordinador principal del tick.
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

        // Avanza la simulación una cantidad de ticks.
        // Aquí ya no usamos incrementos artificiales:
        // cada tick llama al coordinador real del motor.
        public SimulationStepResponse AvanzarSimulacion(int ticks)
        {
            // Validación mínima: no permitir 0 o negativos.
            if (ticks < 1)
            {
                ticks = 1;
            }

            int eventosProcesadosEnEstePaso = 0;
            int saltosLogicosEnEstePaso = 0;
            int candidatosCrossPortEnEstePaso = 0;

            // Ejecutamos tick por tick.
            for (int i = 0; i < ticks; i++)
            {
                // El tiempo lógico del sistema avanza.
                _store.CurrentTick++;

                // El coordinador ejecuta:
                // 1. rotación orbital
                // 2. despacho por prioridad
                // 3. actualización de métricas básicas
                SimulationCoordinatorResult result = _simulationCoordinator.ExecuteSingleTick();

                // Acumulamos métricas del paso actual.
                eventosProcesadosEnEstePaso += result.MessagesDispatched;
                saltosLogicosEnEstePaso += result.MessagesDispatched;
                candidatosCrossPortEnEstePaso += result.CrossPortCandidates;
            }

            // Actualizamos los contadores globales del store.
            _store.EventsProcessed += eventosProcesadosEnEstePaso;
            _store.LogicalJumps += saltosLogicosEnEstePaso;

            // Construimos la respuesta para API / controlador.
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

        // Este método intenta reenviar un paquete al hemisferio hermano
        // cuando la IP destino pertenece a la otra instancia.
        public async Task<bool> IntentarRelayCrossPortAsync(MessagePacket paquete)
        {
            // Validación defensiva.
            if (paquete == null)
            {
                return false;
            }

            // Si la IP está vacía, no podemos enrutar.
            if (string.IsNullOrWhiteSpace(paquete.DestinationIp))
            {
                paquete.Status = MessageStatus.Fallido;
                return false;
            }

            bool esDestinoSur = EsIpDelHemisferioSur(paquete.DestinationIp);
            bool esDestinoNorte = EsIpDelHemisferioNorte(paquete.DestinationIp);

            // Si esta instancia es North y el destino pertenece al Sur,
            // reenviamos por HTTP.
            if (_settings.Hemisphere == "North" && esDestinoSur)
            {
                paquete.Status = MessageStatus.Reenviado;
                return await _relayHttpService.EnviarPaqueteAlHemisferioHermanoAsync(paquete);
            }

            // Si esta instancia es South y el destino pertenece al Norte,
            // también reenviamos por HTTP.
            if (_settings.Hemisphere == "South" && esDestinoNorte)
            {
                paquete.Status = MessageStatus.Reenviado;
                return await _relayHttpService.EnviarPaqueteAlHemisferioHermanoAsync(paquete);
            }

            // Si no aplica cross-port, no hacemos relay.
            return false;
        }

        // Verifica si una IP pertenece al hemisferio Sur.
        // Por ahora sigue con IPs conocidas del enunciado.
        private bool EsIpDelHemisferioSur(string destinationIp)
        {
            return destinationIp == "10.0.0.90";
        }

        // Verifica si una IP pertenece al hemisferio Norte.
        private bool EsIpDelHemisferioNorte(string destinationIp)
        {
            return destinationIp == "10.0.0.50";
        }
    }
}
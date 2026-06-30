using Microsoft.AspNetCore.Mvc;

using OrbitNet.Web.Models.Entities;
using OrbitNet.Web.Services.SimulationEngine;

namespace OrbitNet.Web.Controllers
{
    [ApiController]
    [Route("api/v1/space")]
    public class SpaceController : ControllerBase
    {
        private readonly XmlIngestService _xmlIngestService;
        private readonly BasicAuthService _basicAuthService;
        private readonly TickProcessor _tickProcessor;
        private readonly OrbitNetStore _store;

        public SpaceController(
            XmlIngestService xmlIngestService,
            BasicAuthService basicAuthService,
            TickProcessor tickProcessor,
            OrbitNetStore store)
        {
            _xmlIngestService = xmlIngestService;
            _basicAuthService = basicAuthService;
            _tickProcessor = tickProcessor;
            _store = store;
        }

        [HttpPost("config")]
        public IActionResult CargarConfiguracion([FromBody] ConfigRequestDto request)
        {
            XmlIngestResult result = _xmlIngestService.ProcesarConfiguracion(request.XmlData);

            if (!result.Success)
            {
                return BadRequest(new ConfigErrorResponse
                {
                    Status = "Error",
                    ErrorCode = result.ErrorCode ?? "",
                    Details = result.Details ?? ""
                });
            }

            return Ok(new ConfigSuccessResponse
            {
                Status = "Success",
                Message = "Configuracion cargada exitosamente en RAM. Nodos procesados: " + result.NodosProcesados + ".",
                Timestamp = DateTime.UtcNow.ToString("o")
            });
        }

        [HttpPost("relay")]
        public async Task<IActionResult> RecibirRelay([FromBody] MessagePacket paquete)
        {
            string? authHeader = Request.Headers.Authorization.FirstOrDefault();

            if (!_basicAuthService.EsCabeceraValida(authHeader))
            {
                return Unauthorized(new RelayErrorResponse
                {
                    Status = "Unauthorized",
                    Details = "Acceso restringido. Cabecera HTTP Basic Auth invalida o ausente."
                });
            }

            if (!_store.ConfigLoaded)
            {
                return BadRequest(new ConfigErrorResponse
                {
                    Status = "Error",
                    ErrorCode = "CONFIG_NOT_LOADED",
                    Details = "Debe cargar la configuracion XML con POST /api/v1/space/config antes de enviar relay."
                });
            }

            if (_tickProcessor.RequiereRelayCrossPort(paquete))
            {
                bool enviado = await _tickProcessor.IntentarRelayCrossPortAsync(paquete);

                if (!enviado)
                {
                    return StatusCode(502, new ConfigErrorResponse
                    {
                        Status = "Error",
                        ErrorCode = "RELAY_FORWARD_FAILED",
                        Details = "No se pudo reenviar el paquete al hemisferio hermano. Verifique que la otra instancia este activa."
                    });
                }

                return StatusCode(201, new RelaySuccessResponse
                {
                    Status = "Forwarded",
                    Message = "Paquete reenviado al hemisferio hermano via HTTP (puerto " + paquete.DestinationIp + ").",
                    QueueOccupancyPercentage = _store.CalcularOcupacionCola()
                });
            }

            var receptor = _store.SatelliteRuntime.FindSatellite(_store.ReceptorSatelliteId);
            if (receptor?.PaquetesABordo != null)
            {
                receptor.PaquetesABordo.Agregar(paquete);
            }

            return StatusCode(201, new RelaySuccessResponse
            {
                Status = "Routed",
                Message = "Mensaje insertado con exito en el buffer de prioridad del satelite receptor " + _store.ReceptorSatelliteId + ".",
                QueueOccupancyPercentage = _store.QueueOccupancyPercentage
            });
        }

        [HttpPost("simulation/step")]
        public IActionResult AvanzarSimulacion([FromBody] SimulationStepRequestDto request)
        {
            if (!_store.ConfigLoaded)
            {
                return BadRequest(new ConfigErrorResponse
                {
                    Status = "Error",
                    ErrorCode = "CONFIG_NOT_LOADED",
                    Details = "Debe cargar la configuracion XML con POST /api/v1/space/config antes de avanzar la simulacion."
                });
            }

            SimulationStepResponse response = _tickProcessor.AvanzarSimulacion(request.Ticks);
            return Ok(response);
        }
    }
}

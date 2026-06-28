using Microsoft.AspNetCore.Mvc;
using OrbitNet.Web.Models.Entities;                // Para que encuentre MessagePacket
using OrbitNet.Web.Services.SimulationEngine;       // Para que encuentre TickProcessor
using OrbitNet.Web.Models.DTOs;

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
                    ErrorCode = result.ErrorCode,
                    Details = result.Details
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

            
            //Modifique el SpaeController.cs para poder acoplar el tickProcessor que actualicé
            //Ahora ya no tiene datos simulados, ahora si trabaja con datos reales. 
            // 1. Intentamos el reenvío cross-port inteligente del motor
            bool enviado = await _tickProcessor.IntentarRelayCrossPortAsync(paquete);

            // 2. Si se envió con éxito a la otra instancia, respondemos "Forwarded"
            if (enviado)
            {
                return StatusCode(201, new RelaySuccessResponse
                {
                    Status = "Forwarded",
                    Message = "Paquete reenviado al hemisferio hermano via HTTP.",
                    QueueOccupancyPercentage = _store.CalcularOcupacionCola()
                });
            }

            // 3. Cambio para Reciente DOmingo 13:38 .
            // Ahora sí lo insertamos en el buffer real del satélite receptor.
            Satellite? sateliteReceptor = _store.SatelliteRuntime.FindSatellite(_store.ReceptorSatelliteId);

            if (sateliteReceptor == null)
            {
                return StatusCode(500, new RelayErrorResponse
                {
                    Status = "Error",
                    Details = "No se encontro el satelite receptor configurado en memoria: " + _store.ReceptorSatelliteId
                });
            }

            sateliteReceptor.PaquetesABordo.Agregar(paquete);

            // Actualizamos un valor simple de ocupación global para respuesta visual.
            _store.QueueOccupancyPercentage = _store.CalcularOcupacionCola();

            return StatusCode(201, new RelaySuccessResponse
            {
                Status = "Routed",
                Message = "Mensaje insertado con exito en el buffer de prioridad del satelite receptor " + _store.ReceptorSatelliteId + ".",
                QueueOccupancyPercentage = _store.QueueOccupancyPercentage
            });
        }

        // Cambie SimulationStepRequestDto a esta clase SimulationStepRequest porque
        //Ahora ya existe su versión mejorada.
        [HttpPost("simulation/step")]
        public IActionResult AvanzarSimulacion([FromBody] SimulationStepRequest request)
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

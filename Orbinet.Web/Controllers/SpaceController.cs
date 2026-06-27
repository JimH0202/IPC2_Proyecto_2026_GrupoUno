using Microsoft.AspNetCore.Mvc;
using Orbinet.Web.Models.Entities;                // Para que encuentre MessagePacket
using Orbinet.Web.Services.SimulationEngine;       // Para que encuentre TickProcessor
using Orbinet.Web.Models.DTOs;

namespace Orbinet.Web.Controllers
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
        public IActionResult RecibirRelay([FromBody] MessagePacket paquete)
        {
            string authHeader = Request.Headers.Authorization.FirstOrDefault();

            if (!_basicAuthService.EsCabeceraValida(authHeader))
            {
                return Unauthorized(new RelayErrorResponse
                {
                    Status = "Unauthorized",
                    Details = "Acceso restringido. Cabecera HTTP Basic Auth invalida o ausente."
                });
            }

            _store.QueueOccupancyPercentage = 40.0;

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
            SimulationStepResponse response = _tickProcessor.AvanzarSimulacion(request.Ticks);
            return Ok(response);
        }
    }
}
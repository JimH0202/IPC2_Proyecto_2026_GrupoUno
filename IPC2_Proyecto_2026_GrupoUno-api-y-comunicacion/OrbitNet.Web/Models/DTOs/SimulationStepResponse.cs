namespace OrbitNet.Web.Models.DTOs
{
    /// DTO de salida para responder el resultado del avance de simulación.
    public class SimulationStepResponse
    {
        /// Estado textual de la operación.
        public string Status { get; set; }

        /// Tick actual alcanzado por la simulación.
        public int CurrentTick { get; set; }

        /// Eventos procesados acumulados.
        public int EventsProcessed { get; set; }

        /// Mensaje descriptivo del resultado.
        public string Details { get; set; }

        public SimulationStepResponse()
        {
            Status = string.Empty;
            Details = string.Empty;
        }
    }
}
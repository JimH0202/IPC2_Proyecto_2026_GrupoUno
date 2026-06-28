namespace OrbitNet.Web.Models.DTOs
{
    /// DTO de entrada para solicitar avance de simulación.
    public class SimulationStepRequest
    {
        /// Cantidad de ticks a ejecutar.
        public int Ticks { get; set; }

        public SimulationStepRequest()
        {
            Ticks = 1;
        }
    }
}
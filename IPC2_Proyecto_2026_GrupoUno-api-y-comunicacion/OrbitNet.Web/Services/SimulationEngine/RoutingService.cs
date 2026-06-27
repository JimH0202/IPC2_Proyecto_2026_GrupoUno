using Orbinet.Web.Models.Entities;
using Orbinet.Web.Models.Enums;

namespace Orbinet.Web.Services.SimulationEngine
{
    //Recibe el mensaje de MessagePacket, Busca si existe la IP en ListasAntenas
    //Si existe lo entrega y lo mete en PaquetesRecibidos
    //Si no existe lo deja como Reenviado y devuelve false.
    public class RoutingService
    {
        // Referencia al almacén central del sistema.
        // Desde aquí accedemos a la lista de antenas y demás estructuras en RAM.
        private readonly OrbitNetStore _store;

        // Constructor: recibe el store para poder consultar datos globales.
        public RoutingService(OrbitNetStore store)
        {
            _store = store;
        }

        // Este método intenta enrutar un paquete ya despachado.
        // Devuelve true si se logró entrega local.
        // Devuelve false si no pertenece a una antena local
        // y por lo tanto podría enviarse al otro hemisferio.
        public bool TryRouteMessage(MessagePacket packet)
        {
            // Validación defensiva:
            // si el paquete viene nulo, no se puede procesar.
            if (packet == null)
            {
                return false;
            }

            // Si la IP destino está vacía, el mensaje no puede ser entregado.
            if (string.IsNullOrWhiteSpace(packet.DestinationIp))
            {
                packet.Status = MessageStatus.Fallido;
                return false;
            }

            // Buscamos si la IP pertenece a una antena registrada localmente.
            GroundAntenna? antenna = _store.Antenas.SearchByIp(packet.DestinationIp);

            // Si encontramos antena local, el mensaje se entrega aquí mismo.
            if (antenna != null)
            {
                // El paquete cambia a estado entregado.
                packet.Status = MessageStatus.Entregado;

                // Lo guardamos en la cola de recibidos de la antena.
                antenna.PaquetesRecibidos.Agregar(packet);

                return true;
            }

            // Si no existe antena local con esa IP,
            // no se entrega aquí. Queda para posible relay cross-port.
            packet.Status = MessageStatus.Reenviado;
            return false;
        }
    }
}
namespace OrbitNet.Web.DataStructures.Interfaces;
using OrbitNet.Web.Models.Entities;

public interface IMessageBuffer : IAbstractCollection
{
    void Agregar(MessagePacket packet);
    MessagePacket? ObtenerSiguiente();
    MessagePacket? VerSiguiente();
    MessagePacket? BuscarPorCodigoHex(string hexCode);
    string RecorrerInOrden();
}
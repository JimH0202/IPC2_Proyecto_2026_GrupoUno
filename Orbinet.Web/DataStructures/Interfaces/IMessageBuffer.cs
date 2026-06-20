namespace Orbinet.Web.DataStructures.Interfaces;

using Orbinet.Web.Models.Entities;

public interface IMessageBuffer : IAbstractCollection
{
    void Enqueue(MessagePacket packet);

    MessagePacket? Dequeue();

    MessagePacket? Peek();

    MessagePacket? SearchByHexCode(string hexCode);

    string TraverseInOrder();
}
using Xunit;
using OrbitNet.Web.DataStructures.Buffer;
using OrbitNet.Web.Models.Entities;
using OrbitNet.Web.Models.Enums;

namespace OrbitNet.Tests.BufferTests;

public class BufferMensajesTests
{
    private MessagePacket CrearPaquete(string codigo, int prioridad)
    {
        return new MessagePacket
        {
            CodHex = codigo,
            SenderId = "SAT-ECU-0001",
            DestinationIp = "10.0.0.50",
            Priority = (PriorityLevel)prioridad,
            Content = "Mensaje de prueba"
        };
    }

    [Fact]
    public void Constructor()
    {
        BufferMensajes buffer = new BufferMensajes();

        Assert.True(buffer.IsEmpty);
        Assert.Equal(0, buffer.Count);
    }

    [Fact]
    public void Enqueue()
    {
        BufferMensajes buffer = new BufferMensajes();

        buffer.Enqueue(CrearPaquete("A19F", 3));

        Assert.False(buffer.IsEmpty);
        Assert.Equal(1, buffer.Count);
    }

    [Fact]
    public void Peek()
    {
        BufferMensajes buffer = new BufferMensajes();

        buffer.Enqueue(CrearPaquete("A111", 1));
        buffer.Enqueue(CrearPaquete("A555", 5));
        buffer.Enqueue(CrearPaquete("A333", 3));

        MessagePacket? paquete = buffer.Peek();

        Assert.NotNull(paquete);
        Assert.Equal("A555", paquete!.CodHex);
    }

    [Fact]
    public void Dequeue()
    {
        BufferMensajes buffer = new BufferMensajes();

        buffer.Enqueue(CrearPaquete("A111", 1));
        buffer.Enqueue(CrearPaquete("A555", 5));

        MessagePacket? paquete = buffer.Dequeue();

        Assert.NotNull(paquete);
        Assert.Equal("A555", paquete!.CodHex);
        Assert.Equal(1, buffer.Count);
    }

    [Fact]
    public void SearchByHexCode()
    {
        BufferMensajes buffer = new BufferMensajes();

        buffer.Enqueue(CrearPaquete("ABCD", 2));

        MessagePacket? resultado = buffer.SearchByHexCode("ABCD");

        Assert.NotNull(resultado);
        Assert.Equal("ABCD", resultado!.CodHex);
    }

    [Fact]
    public void Clear()
    {
        BufferMensajes buffer = new BufferMensajes();

        buffer.Enqueue(CrearPaquete("A111", 1));
        buffer.Enqueue(CrearPaquete("A222", 2));

        buffer.Clear();

        Assert.True(buffer.IsEmpty);
        Assert.Equal(0, buffer.Count);
    }
}
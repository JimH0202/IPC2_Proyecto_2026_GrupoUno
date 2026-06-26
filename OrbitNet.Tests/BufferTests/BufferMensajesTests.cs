using Xunit;
using Orbinet.Web.DataStructures.Buffer;
using Orbinet.Web.Models.Entities;
using Orbinet.Web.Models.Enums;

namespace OrbitNet.Tests.BufferTests;

public class BufferMensajesTests
{
    private MessagePacket CrearPaquete(string codigo, PriorityLevel prioridad)
    {
        return new MessagePacket
        {
            CodHex = codigo,
            SenderId = "SAT-ECU-0001",
            DestinationIp = "10.0.0.50",
            Priority = prioridad,
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

        buffer.Enqueue(CrearPaquete("A19F", PriorityLevel.Alta));

        Assert.False(buffer.IsEmpty);
        Assert.Equal(1, buffer.Count);
    }

    [Fact]
    public void Peek()
    {
        BufferMensajes buffer = new BufferMensajes();

        buffer.Enqueue(CrearPaquete("A111", PriorityLevel.Baja));
        buffer.Enqueue(CrearPaquete("A555", PriorityLevel.Alta));
        buffer.Enqueue(CrearPaquete("A333", PriorityLevel.Media));

        MessagePacket? paquete = buffer.Peek();

        Assert.NotNull(paquete);
        Assert.Equal("A555", paquete!.CodHex);
    }

    [Fact]
    public void Dequeue()
    {
        BufferMensajes buffer = new BufferMensajes();

        buffer.Enqueue(CrearPaquete("A111", PriorityLevel.Baja));
        buffer.Enqueue(CrearPaquete("A555", PriorityLevel.Alta));

        MessagePacket? paquete = buffer.Dequeue();

        Assert.NotNull(paquete);
        Assert.Equal("A555", paquete!.CodHex);
        Assert.Equal(1, buffer.Count);
    }

    [Fact]
    public void SearchByHexCode()
    {
        BufferMensajes buffer = new BufferMensajes();

        buffer.Enqueue(CrearPaquete("ABCD", PriorityLevel.Media));

        MessagePacket? resultado = buffer.SearchByHexCode("ABCD");

        Assert.NotNull(resultado);
        Assert.Equal("ABCD", resultado!.CodHex);
    }

    [Fact]
    public void Clear()
    {
        BufferMensajes buffer = new BufferMensajes();

        buffer.Enqueue(CrearPaquete("A111", PriorityLevel.Baja));
        buffer.Enqueue(CrearPaquete("A222", PriorityLevel.Media));

        buffer.Clear();

        Assert.True(buffer.IsEmpty);
        Assert.Equal(0, buffer.Count);
    }
}
namespace Orbinet.Web.OrbitNet.Tests.LogTests;
using System;
using Xunit;

public class LogRegistroTests
{
    [Fact]
    public void EscribirEvento_InsertaNodoCorrectamente()
    {
        // Arrange
        var log = new LogRegistro();

        // Act
        log.EscribirEvento("INFO", "Sistema iniciado");

        // Assert
        Assert.False(log.IsEmpty);
        Assert.Equal(1, log.Count);
    }

    [Fact]
    public void BuscarPorRegex_EncuentraCoincidencia()
    {
        // Arrange
        var log = new LogRegistro();
        log.EscribirEvento("ERROR", "Fallo en satelite");
        log.EscribirEvento("INFO", "Evento normal");

        log.BuscarPorRegex("satelite"); 

        Assert.Equal(2, log.Count);
    }
}

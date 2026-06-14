namespace OrbitNet.Tests.LogTests;
using Orbinet.Web.DataStructures.Logs;
using Xunit;

public class LogAuditoriaTests
{
    [Fact]
    public void WriteEvent()
    {
        var log = new LogAuditoria();

        log.WriteEvent("INFO", "Sistema iniciado");

        Assert.False(log.IsEmpty);
        Assert.Equal(1, log.Count);
    }

    [Fact]
    public void SearchLogRegex()
    {
        var log = new LogAuditoria();
        log.WriteEvent("ERROR", "Fallo en satelite");
        log.WriteEvent("INFO", "Evento normal");

        string resultado = log.SearchLogRegex("satelite");

        Assert.Contains("Fallo en satelite", resultado);
        Assert.Contains("ERROR", resultado);
        Assert.Equal(2, log.Count);
    }

    [Fact]
    public void Clear()
    {
        var log = new LogAuditoria();
        log.WriteEvent("INFO", "Sistema iniciado");

        log.Clear();

        Assert.True(log.IsEmpty);
        Assert.Equal(0, log.Count);
    }
}
using Xunit;
using Orbinet.Web.DataStructures.Antenas;
using Orbinet.Web.Models.Entities;

namespace OrbitNet.Tests.AntenasTests;

public class ListaAntenasTests
{
    private GroundAntenna CrearAntena(string id, string name, string ip)
    {
        return new GroundAntenna
        {
            Id = id,
            Name = name,
            Coords = "10,20",
            Ip = ip
        };
    }

    [Fact]
    public void Constructor()
    {
        ListaAntenas lista = new ListaAntenas();

        Assert.True(lista.IsEmpty);
        Assert.Equal(0, lista.Count);
    }

    [Fact]
    public void Add()
    {
        ListaAntenas lista = new ListaAntenas();

        lista.Add(CrearAntena("ANT-001", "Antena Norte", "192.168.1.10"));

        Assert.False(lista.IsEmpty);
        Assert.Equal(1, lista.Count);
    }

    [Fact]
    public void SearchById_Encontrar()
    {
        ListaAntenas lista = new ListaAntenas();

        lista.Add(CrearAntena("ANT-001", "Antena Norte", "192.168.1.10"));
        lista.Add(CrearAntena("ANT-002", "Antena Sur", "192.168.1.20"));

        GroundAntenna? encontrada = lista.SearchById("ANT-002");

        Assert.NotNull(encontrada);
        Assert.Equal("ANT-002", encontrada!.Id);
        Assert.Equal("Antena Sur", encontrada.Name);
        Assert.Equal("192.168.1.20", encontrada.Ip);
    }

    [Fact]
    public void SearchById_Retornar()
    {
        ListaAntenas lista = new ListaAntenas();

        lista.Add(CrearAntena("ANT-001", "Antena Norte", "192.168.1.10"));

        GroundAntenna? encontrada = lista.SearchById("ANT-999");

        Assert.Null(encontrada);
    }

    [Fact]
    public void Clear()
    {
        ListaAntenas lista = new ListaAntenas();

        lista.Add(CrearAntena("ANT-001", "Antena Norte", "192.168.1.10"));
        lista.Add(CrearAntena("ANT-002", "Antena Sur", "192.168.1.20"));

        lista.Clear();

        Assert.True(lista.IsEmpty);
        Assert.Equal(0, lista.Count);
        Assert.Null(lista.SearchById("ANT-001"));
    }
}
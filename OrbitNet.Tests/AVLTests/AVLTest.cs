using Xunit;
using Orbinet.Web.DataStructures.AVL;
using Orbinet.Web.Models.Entities;

namespace OrbitNet.Tests.AVLTests;

public class AvlNodeTests
{
    private Satellite CrearSatelite(string id, string name, string ip)
    {
        return new Satellite
        {
            Id = id,
            Name = name,
            Ip = ip
        };
    }

    [Fact]
    public void Nodo()
    {
        Satellite satellite = CrearSatelite("SAT-ECU-0001", "Satelite Norte", "10.0.0.1");

        AvlNode node = new AvlNode(satellite);

        Assert.NotNull(node.Satellite);
        Assert.Equal("SAT-ECU-0001", node.Satellite.Id);
        Assert.Equal("Satelite Norte", node.Satellite.Name);
        Assert.Equal("10.0.0.1", node.Satellite.Ip);
    }

    [Fact]
    public void Altura()
    {
        Satellite satellite = CrearSatelite("SAT-POL-0002", "Satelite Polar", "10.0.0.2");

        AvlNode node = new AvlNode(satellite);

        Assert.Equal(1, node.Height);
    }

    [Fact]
    public void ConstructorHijos()
    {
        Satellite satellite = CrearSatelite("SAT-ECU-0003", "Satelite Sur", "10.0.0.3");

        AvlNode node = new AvlNode(satellite);

        Assert.Null(node.Left);
        Assert.Null(node.Right);
    }
}
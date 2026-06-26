using Xunit;
using OrbitNet.Web.DataStructures.AVL;
using OrbitNet.Web.Models.Entities;

namespace OrbitNet.Tests.AVLTests;

public class RegistroSatelitesTests
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
    public void Iniciar()
    {
        RegistroSatelites registro = new RegistroSatelites();

        Assert.True(registro.IsEmpty);
        Assert.Equal(0, registro.Count);
    }

    [Fact]
    public void Agregar()
    {
        RegistroSatelites registro = new RegistroSatelites();

        registro.Insert(CrearSatelite("SAT-002", "Satelite 2", "10.0.0.2"));

        Assert.False(registro.IsEmpty);
        Assert.Equal(1, registro.Count);
    }

    [Fact]
    public void Econtrar()
    {
        RegistroSatelites registro = new RegistroSatelites();

        registro.Insert(CrearSatelite("SAT-001", "Satelite 1", "10.0.0.1"));
        registro.Insert(CrearSatelite("SAT-002", "Satelite 2", "10.0.0.2"));

        Satellite? encontrado = registro.Search("SAT-002");

        Assert.NotNull(encontrado);
        Assert.Equal("SAT-002", encontrado!.Id);
        Assert.Equal("Satelite 2", encontrado.Name);
    }

    [Fact]
    public void RetornarNull()
    {
        RegistroSatelites registro = new RegistroSatelites();

        registro.Insert(CrearSatelite("SAT-001", "Satelite 1", "10.0.0.1"));

        Satellite? encontrado = registro.Search("SAT-999");

        Assert.Null(encontrado);
    }

    [Fact]
    public void NoDuplicados()
    {
        RegistroSatelites registro = new RegistroSatelites();

        registro.Insert(CrearSatelite("SAT-001", "Satelite 1", "10.0.0.1"));
        registro.Insert(CrearSatelite("SAT-001", "Satelite duplicado", "10.0.0.99"));

        Assert.Equal(1, registro.Count);
        Assert.Equal("Satelite 1", registro.Search("SAT-001")!.Name);
    }

    [Fact]
    public void Eliminar()
    {
        RegistroSatelites registro = new RegistroSatelites();

        registro.Insert(CrearSatelite("SAT-001", "Satelite 1", "10.0.0.1"));
        registro.Insert(CrearSatelite("SAT-002", "Satelite 2", "10.0.0.2"));

        bool eliminado = registro.Delete("SAT-001");

        Assert.True(eliminado);
        Assert.Null(registro.Search("SAT-001"));
        Assert.Equal(1, registro.Count);
    }

    [Fact]
    public void FalsoSiNoExite()
    {
        RegistroSatelites registro = new RegistroSatelites();

        registro.Insert(CrearSatelite("SAT-001", "Satelite 1", "10.0.0.1"));

        bool eliminado = registro.Delete("SAT-999");

        Assert.False(eliminado);
        Assert.Equal(1, registro.Count);
    }

    [Fact]
    public void Clear()
    {
        RegistroSatelites registro = new RegistroSatelites();

        registro.Insert(CrearSatelite("SAT-001", "Satelite 1", "10.0.0.1"));
        registro.Insert(CrearSatelite("SAT-002", "Satelite 2", "10.0.0.2"));

        registro.Clear();

        Assert.True(registro.IsEmpty);
        Assert.Equal(0, registro.Count);
        Assert.Null(registro.Search("SAT-001"));
    }

    [Fact]
    public void Ordenar()
    {
        RegistroSatelites registro = new RegistroSatelites();

        registro.Insert(CrearSatelite("SAT-003", "Satelite 3", "10.0.0.3"));
        registro.Insert(CrearSatelite("SAT-001", "Satelite 1", "10.0.0.1"));
        registro.Insert(CrearSatelite("SAT-002", "Satelite 2", "10.0.0.2"));

        string resultado = registro.TraverseInOrder();

        int pos1 = resultado.IndexOf("SAT-001");
        int pos2 = resultado.IndexOf("SAT-002");
        int pos3 = resultado.IndexOf("SAT-003");

        Assert.True(pos1 < pos2);
        Assert.True(pos2 < pos3);
    }

    [Fact]
    public void RotacionIzquierda()
    {
        RegistroSatelites registro = new RegistroSatelites();

        registro.Insert(CrearSatelite("SAT-001", "Satelite 1", "10.0.0.1"));
        registro.Insert(CrearSatelite("SAT-002", "Satelite 2", "10.0.0.2"));
        registro.Insert(CrearSatelite("SAT-003", "Satelite 3", "10.0.0.3"));

        AvlNode? root = registro.GetRoot();

        Assert.NotNull(root);
        Assert.Equal("SAT-002", root!.Satellite.Id);
    }

    [Fact]
    public void RotacionDerecha()
    {
        RegistroSatelites registro = new RegistroSatelites();

        registro.Insert(CrearSatelite("SAT-003", "Satelite 3", "10.0.0.3"));
        registro.Insert(CrearSatelite("SAT-002", "Satelite 2", "10.0.0.2"));
        registro.Insert(CrearSatelite("SAT-001", "Satelite 1", "10.0.0.1"));

        AvlNode? root = registro.GetRoot();

        Assert.NotNull(root);
        Assert.Equal("SAT-002", root!.Satellite.Id);
    }
}
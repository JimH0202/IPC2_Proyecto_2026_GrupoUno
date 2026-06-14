using Xunit;
using Orbinet.Web.DataStructures.Matrix;

namespace OrbitNet.Tests.MatrixTests;

public class RedSatelitalPlanoTests
{
    [Fact]
    public void Constructor()
    {
        RedSatelitalPlano matriz = new RedSatelitalPlano();
        Assert.True(matriz.IsEmpty); // Verificar que la matriz este vacia al crearla
        Assert.Equal(0, matriz.Count); // Verificar que el conteo de elementos sea 0 al crear la matriz
    }

    [Fact]
    public void InsertN()
    {
        RedSatelitalPlano matriz = new RedSatelitalPlano(); 
        bool insertado = matriz.Insert(1, 2, "SAT-ECU-0001", "10.0.0.1"); // Insertar un nodo en la matriz
        Assert.True(insertado); // Verificar que el nodo se inserto correctamente
        Assert.False(matriz.IsEmpty); // Verificar que la matriz no este vacia despues de insertar un nodo
        Assert.Equal(1, matriz.Count); // Verificar que el conteo de elementos sea 1 despues de insertar un nodo
    }

    [Fact]
    public void Search()
    {
        RedSatelitalPlano matriz = new RedSatelitalPlano();
        matriz.Insert(1, 2, "SAT-ECU-0001", "10.0.0.1"); // Insertar un nodo en la matriz
        MatrixNode? encontrado = matriz.Search(1, 2); // Buscar el nodo insertado en la matriz
        Assert.NotNull(encontrado); // Verificar que el nodo se encontro en la matriz
        Assert.Equal("SAT-ECU-0001", encontrado!.SatelliteId); // Verificar que el SatelliteId del nodo encontrado sea correcto
        Assert.Equal("10.0.0.1", encontrado.IpAddress); // Verificar que la IpAddress del nodo encontrado sea correcta
    }

    [Fact]
    public void Insert()
    {
        RedSatelitalPlano matriz = new RedSatelitalPlano();
        bool primero = matriz.Insert(1, 2, "SAT-ECU-0001", "10.0.0.1"); // Insertar un nodo en la matriz
        bool segundo = matriz.Insert(1, 2, "SAT-ECU-0002", "10.0.0.2");// Intentar insertar un nodo en la misma posición de la matriz
        Assert.True(primero);
        Assert.False(segundo);
        Assert.Equal(1, matriz.Count);
    }

    [Fact]
    public void Delete()
    {
        RedSatelitalPlano matriz = new RedSatelitalPlano();
        matriz.Insert(1, 2, "SAT-ECU-0001", "10.0.0.1"); // Insertar un nodo en la matriz
        bool eliminado = matriz.Delete(1, 2); // Eliminar el nodo insertado en la matriz
        MatrixNode? encontrado = matriz.Search(1, 2); // Intentar buscar el nodo eliminado en la matriz
        Assert.True(eliminado);
        Assert.Null(encontrado);
        Assert.True(matriz.IsEmpty);
        Assert.Equal(0, matriz.Count);
    }

    [Fact]
    public void Clear()
    {
        RedSatelitalPlano matriz = new RedSatelitalPlano();

        matriz.Insert(1, 2, "SAT-ECU-0001", "10.0.0.1"); 
        matriz.Insert(2, 3, "SAT-POL-0002", "10.0.0.2");
        
        matriz.Clear();

        Assert.True(matriz.IsEmpty); // Verificar que la matriz este vacia despues de llamar a Clear
        Assert.Equal(0, matriz.Count);  // Verificar que el conteo de elementos sea 0 despues de llamar a Clear
    }
}
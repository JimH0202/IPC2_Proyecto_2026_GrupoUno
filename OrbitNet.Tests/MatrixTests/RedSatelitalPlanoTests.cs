using Xunit;
using Orbinet.Web.DataStructures.Matrix;

namespace OrbitNet.Tests.MatrixTests;

public class RedSatelitalPlanoTests
{
    [Fact]
    public void Constructor_DebeIniciarMatrizVacia()
    {
        RedSatelitalPlano matriz = new RedSatelitalPlano();
        Assert.True(matriz.IsEmpty); // Verificar que la matriz este vacia al crearla
        Assert.Equal(0, matriz.Count); // Verificar que el conteo de elementos sea 0 al crear la matriz
    }

    [Fact]
    public void Clear_DebeVaciarMatriz()
    {
        RedSatelitalPlano matriz = new RedSatelitalPlano();

        matriz.Clear();

        Assert.True(matriz.IsEmpty); // Verificar que la matriz este vacia despues de llamar a Clear
        Assert.Equal(0, matriz.Count);  // Verificar que el conteo de elementos sea 0 despues de llamar a Clear
    }
}
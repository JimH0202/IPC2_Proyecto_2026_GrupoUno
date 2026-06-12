using Xunit;
using Orbinet.Web.DataStructures.Matrix;

namespace OrbitNet.Tests.MatrixTests;

public class MatrixNodeTests
{
    [Fact]
    public void Constructor_DebeCrearMatrixNodeCorrectamente() // Verificar que el constructor de MatrixNode inicializa correctamente sus propiedades
    {
        MatrixNode node = new MatrixNode(1, 2, "SAT-ECU-0001", "10.0.0.1");

        Assert.Equal(1, node.Row);
        Assert.Equal(2, node.Column);
        Assert.Equal("SAT-ECU-0001", node.SatelliteId);
        Assert.Equal("10.0.0.1", node.IpAddress);

        Assert.Null(node.Up);
        Assert.Null(node.Down);
        Assert.Null(node.Left);
        Assert.Null(node.Right);
    }
}
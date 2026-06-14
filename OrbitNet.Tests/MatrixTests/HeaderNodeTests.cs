using Xunit;
using Orbinet.Web.DataStructures.Matrix;

namespace OrbitNet.Tests.MatrixTests;

public class HeaderNodeTests
{
    [Fact]
    public void Constructor() // Verificar que el constructor de HeaderNode inicializa correctamente sus propiedades
    {
        HeaderNode header = new HeaderNode(5);

        Assert.Equal(5, header.Index);
        Assert.Null(header.Previous);
        Assert.Null(header.Next);
        Assert.Null(header.Access);
    }
}
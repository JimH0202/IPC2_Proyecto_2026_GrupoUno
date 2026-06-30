namespace OrbitNet.Tests.IntegrationTests;

public class BasicAuthTests
{
    private readonly BasicAuthService _service = new BasicAuthService();

    [Fact]
    public void CrearCabeceraAuthorization_FormatoCorrecto()
    {
        string header = _service.CrearCabeceraAuthorization();

        Assert.StartsWith("Basic ", header);
        Assert.True(_service.EsCabeceraValida(header));
    }

    [Fact]
    public void EsCabeceraValida_SinHeader_RetornaFalse()
    {
        Assert.False(_service.EsCabeceraValida(null));
        Assert.False(_service.EsCabeceraValida(""));
    }

    [Fact]
    public void EsCabeceraValida_CredencialesIncorrectas_RetornaFalse()
    {
        Assert.False(_service.EsCabeceraValida("Basic dXN1YXJpbzpwYXNzd29yZA=="));
    }
}

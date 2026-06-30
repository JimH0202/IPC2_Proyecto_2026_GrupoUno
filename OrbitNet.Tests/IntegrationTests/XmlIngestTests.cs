namespace OrbitNet.Tests.IntegrationTests;

public class XmlIngestTests
{
    private static string LeerXml(string nombreArchivo)
    {
        string ruta = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "OrbitNet.Web", "ArchivosPrueba", nombreArchivo);
        return File.ReadAllText(Path.GetFullPath(ruta));
    }

    [Fact]
    public void ProcesarConfiguracion_XmlValido_CargaEnRam()
    {
        var store = new OrbitNetStore();
        var service = new XmlIngestService(store);
        string xml = LeerXml("Cargaexitosa1_CNorte_5000.xml");

        XmlIngestResult result = service.ProcesarConfiguracion(xml);

        Assert.True(result.Success);
        Assert.True(store.ConfigLoaded);
        Assert.True(result.NodosProcesados > 0);
    }

    [Fact]
    public void ProcesarConfiguracion_RegExInvalido_NoCargaEnRam()
    {
        var store = new OrbitNetStore();
        var service = new XmlIngestService(store);
        string xml = LeerXml("Control_Errores_y_excepciones.xml");

        XmlIngestResult result = service.ProcesarConfiguracion(xml);

        Assert.False(result.Success);
        Assert.False(store.ConfigLoaded);
        Assert.Equal("XML_SCHEMA_VIOLATION", result.ErrorCode);
    }

    [Fact]
    public void ProcesarConfiguracion_XmlMalformado_RetornaError()
    {
        var store = new OrbitNetStore();
        var service = new XmlIngestService(store);

        XmlIngestResult result = service.ProcesarConfiguracion("<orbitnet><sin-cerrar>");

        Assert.False(result.Success);
        Assert.False(store.ConfigLoaded);
        Assert.Equal("XML_SCHEMA_VIOLATION", result.ErrorCode);
    }

    [Fact]
    public void ProcesarConfiguracion_XmlVacio_RetornaError()
    {
        var store = new OrbitNetStore();
        var service = new XmlIngestService(store);

        XmlIngestResult result = service.ProcesarConfiguracion("   ");

        Assert.False(result.Success);
        Assert.Equal("XML_SCHEMA_VIOLATION", result.ErrorCode);
    }
}

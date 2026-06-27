using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using OrbitNet.Web.Models.Enums;

namespace OrbitNet.Tests.IntegrationTests;

public class RelayTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RelayTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostConfig_XmlValido_Retorna200()
    {
        HttpClient client = _factory.CreateClient();
        string xml = File.ReadAllText(ObtenerRutaXml("Cargaexitosa1_CNorte_5000.xml"));
        var body = new { xml_data = xml };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/space/config", body);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"status\":\"Success\"", json.Replace(" ", ""));
    }

    [Fact]
    public async Task PostConfig_XmlInvalido_Retorna400()
    {
        HttpClient client = _factory.CreateClient();
        string xml = File.ReadAllText(ObtenerRutaXml("Control_Errores_y_excepciones.xml"));
        var body = new { xml_data = xml };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/space/config", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostRelay_SinBasicAuth_Retorna401()
    {
        HttpClient client = _factory.CreateClient();
        await CargarConfiguracionAsync(client);
        var paquete = CrearPaqueteRelay("10.0.0.50");

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/space/relay", paquete);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostRelay_SinConfig_Retorna400()
    {
        using WebApplicationFactory<Program> factoryLimpia = new WebApplicationFactory<Program>();
        HttpClient client = factoryLimpia.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", "b3JiaXRuZXRfYWRtaW46VVNBQ19FQ1lTXzIwMjY=");

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/space/relay", CrearPaqueteRelay("10.0.0.50"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        string json = await response.Content.ReadAsStringAsync();
        Assert.Contains("CONFIG_NOT_LOADED", json);
    }

    [Fact]
    public async Task PostRelay_ConBasicAuth_Retorna201()
    {
        HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", "b3JiaXRuZXRfYWRtaW46VVNBQ19FQ1lTXzIwMjY=");

        await CargarConfiguracionAsync(client);

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/space/relay", CrearPaqueteRelay("10.0.0.50"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        string json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"status\":\"Routed\"", json.Replace(" ", ""));
        Assert.Contains("queue_occupancy_percentage", json);
    }

    [Fact]
    public async Task PostSimulationStep_SinConfig_Retorna400()
    {
        using WebApplicationFactory<Program> factoryLimpia = new WebApplicationFactory<Program>();
        HttpClient client = factoryLimpia.CreateClient();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/space/simulation/step", new { ticks = 2 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        string json = await response.Content.ReadAsStringAsync();
        Assert.Contains("CONFIG_NOT_LOADED", json);
    }

    [Fact]
    public async Task PostSimulationStep_Retorna200()
    {
        HttpClient client = _factory.CreateClient();
        await CargarConfiguracionAsync(client);

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/space/simulation/step", new { ticks = 2 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string json = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(json);
        Assert.Equal("Simulated", doc.RootElement.GetProperty("status").GetString());
        Assert.True(doc.RootElement.GetProperty("current_tick").GetInt32() >= 2);
    }

    [Fact]
    public async Task RelayHttpService_EnviaBasicAuthAlPuertoHermano()
    {
        string? authRecibida = null;
        string? urlRecibida = null;

        var handler = new CapturingHandler((request) =>
        {
            authRecibida = request.Headers.Authorization?.ToString();
            urlRecibida = request.RequestUri?.ToString();
            return new HttpResponseMessage(HttpStatusCode.Created);
        });

        var httpClientFactory = new FakeHttpClientFactory(handler);
        var basicAuth = new BasicAuthService();
        var settings = Microsoft.Extensions.Options.Options.Create(new OrbitNet.Web.Configuration.AppInstanceSettings
        {
            Hemisphere = "North",
            Port = 5000,
            SiblingPort = 5001
        });

        var relayService = new OrbitNet.Web.Services.Communication.RelayHttpService(
            httpClientFactory, basicAuth, settings);

        var paquete = new OrbitNet.Web.Models.Entities.MessagePacket
        {
            CodHex = "A19F",
            SenderId = "SAT-ECU-0012",
            DestinationIp = "10.0.0.90",
            Priority = PriorityLevel.Alta,
            Content = "Simulacion 5000 a 5001"
        };

        bool enviado = await relayService.EnviarPaqueteAlHemisferioHermanoAsync(paquete);

        Assert.True(enviado);
        Assert.Equal("http://localhost:5001/api/v1/space/relay", urlRecibida);
        Assert.StartsWith("Basic ", authRecibida);
        Assert.True(basicAuth.EsCabeceraValida(authRecibida!));
    }

    private static object CrearPaqueteRelay(string destinoIp)
    {
        return new
        {
            codigo_hex = "A19F",
            emisor_id = "SAT-ECU-0012",
            destino_ip = destinoIp,
            prioridad = PriorityLevel.Alta,
            contenido = "Prueba relay API"
        };
    }

    private static async Task CargarConfiguracionAsync(HttpClient client)
    {
        string xml = File.ReadAllText(ObtenerRutaXml("Cargaexitosa1_CNorte_5000.xml"));
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/space/config", new { xml_data = xml });
        response.EnsureSuccessStatusCode();
    }

    private static string ObtenerRutaXml(string nombreArchivo)
    {
        return Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "OrbitNet.Web", "ArchivosPrueba", nombreArchivo));
    }

    private sealed class CapturingHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public CapturingHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            _responder = responder;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_responder(request));
        }
    }

    private sealed class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _client;

        public FakeHttpClientFactory(HttpMessageHandler handler)
        {
            _client = new HttpClient(handler);
        }

        public HttpClient CreateClient(string name) => _client;
    }
}

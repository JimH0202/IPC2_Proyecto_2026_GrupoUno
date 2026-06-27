using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OrbitNet.Web.Configuration;

public class RelayHttpService
{
    private readonly HttpClient _httpClient;
    private readonly BasicAuthService _basicAuthService;
    private readonly AppInstanceSettings _settings;
    private readonly ILogger<RelayHttpService> _logger;

    public RelayHttpService(HttpClient httpClient, BasicAuthService basicAuthService, IOptions<AppInstanceSettings> settings, ILogger<RelayHttpService> logger)
    {
        _httpClient = httpClient;
        _basicAuthService = basicAuthService;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<bool> EnviarPaqueteAlHemisferioHermanoAsync(MessagePacket paquete)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_settings.RemoteHemisphereUrl))
            {
                _logger.LogWarning("RemoteHemisphereUrl no configurada en AppInstanceSettings.");
                return false;
            }

            var target = _settings.RemoteHemisphereUrl.TrimEnd('/') + "/api/v1/space/relay";

            var json = JsonSerializer.Serialize(paquete);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Add Basic Auth header
            var auth = _basicAuthService.CrearCabeceraAuthorization();
            _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(auth);

            _logger.LogInformation("Enviando paquete relay a {Target}", target);

            var response = await _httpClient.PostAsync(target, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Relay exitoso. Código: {Status}", response.StatusCode);
                return true;
            }

            _logger.LogWarning("Relay falló. Código: {Status}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando paquete al hemisferio hermano");
            return false;
        }
    }
}
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using OrbitNet.Web.Configuration;

public class RelayHttpService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly BasicAuthService _basicAuthService;
    private readonly AppInstanceSettings _settings;

    public RelayHttpService(
        IHttpClientFactory httpClientFactory,
        BasicAuthService basicAuthService,
        IOptions<AppInstanceSettings> settings)
    {
        _httpClientFactory = httpClientFactory;
        _basicAuthService = basicAuthService;
        _settings = settings.Value;
    }

    public async Task<bool> EnviarPaqueteAlHemisferioHermanoAsync(MessagePacket paquete)
    {
        HttpClient client = _httpClientFactory.CreateClient();
        string url = "http://localhost:" + _settings.SiblingPort + "/api/v1/space/relay";

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", _basicAuthService.CrearCabeceraAuthorization());
        request.Content = JsonContent.Create(paquete);

        HttpResponseMessage response = await client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}

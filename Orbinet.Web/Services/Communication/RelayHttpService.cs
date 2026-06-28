using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using OrbitNet.Web.Configuration;

public class RelayHttpService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly BasicAuthService _basicAuthService;
    private readonly AppInstanceSettings _settings;
    private readonly ILogger<RelayHttpService> _logger;

    public RelayHttpService(
        IHttpClientFactory httpClientFactory,
        BasicAuthService basicAuthService,
        IOptions<AppInstanceSettings> settings,
        ILogger<RelayHttpService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _basicAuthService = basicAuthService;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<bool> EnviarPaqueteAlHemisferioHermanoAsync(MessagePacket paquete)
    {
        try
        {
            var url = string.IsNullOrWhiteSpace(_settings.RemoteHemisphereUrl)
                ? "http://localhost:" + _settings.SiblingPort + "/api/v1/space/relay"
                : _settings.RemoteHemisphereUrl.TrimEnd('/') + "/api/v1/space/relay";

            using var client = _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(paquete)
            };

            if (!_settings.EnableCrossHemisphereRelay)
            {
                _logger.LogInformation("Relay cross-hemisphere deshabilitado por configuración.");
                return false;
            }

            request.Headers.Add("Authorization", _basicAuthService.CrearCabeceraAuthorization());

            _logger.LogInformation("Enviando paquete relay a {Url}", url);
            using var response = await client.SendAsync(request);

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

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OrbitNet.Web.Configuration;
using OrbitNet.Web.Models.Entities;

namespace OrbitNet.Web.Services.Communication;

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
            if (string.IsNullOrWhiteSpace(_settings.RemoteHemisphereUrl))
            {
                _logger.LogWarning("RemoteHemisphereUrl no configurada en AppInstanceSettings.");
                return false;
            }

            var target = _settings.RemoteHemisphereUrl.TrimEnd('/') + "/api/v1/space/relay";

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, target);
            request.Headers.Add("Authorization", _basicAuthService.CrearCabeceraAuthorization());
            request.Content = JsonContent.Create(paquete);

            _logger.LogInformation("Enviando paquete relay a {Target}", target);

            var response = await client.SendAsync(request);

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
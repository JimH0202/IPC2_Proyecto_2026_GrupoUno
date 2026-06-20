using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Orbinet.Web.Configuration;
using Orbinet.Web.Models.Entities;

namespace Orbinet.Web.Services.Communication{
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
}

using BaseProject.DAO.IService;
using BaseProject.DAO.Models.Others;
using BaseProject.Util;
using Newtonsoft.Json;

namespace BaseProject.DAO.Service
{

    public class ServiceLocalidade : IServiceLocalidade
    {

        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _config;
        private readonly string _key;

        public ServiceLocalidade
        (
            IHttpClientFactory httpClient,
            IConfiguration config
        )
        {
            _httpClient = httpClient;
            _config = config;
            _key = config.GetValue<string>("CSCAPIKEY");
        }

        public async Task<Pais[]> ObterPaises()
        {
            using var httpClient = _httpClient.CreateClient();

            var url = $"https://api.countrystatecity.in/v1/countries";

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Headers = 
                {
                    { "X-CSCAPI-KEY", _key }
                }
            };

            var result = await httpClient.SendAsync(httpRequestMessage);

            var jsonString = await result.Content.ReadAsStringAsync();

            if (jsonString.Contains("error")) return default;

            var paises = JsonConvert.DeserializeObject<Pais[]>(jsonString);

            return paises;
        }

        public async Task<Estado[]> ObterEstados(string codPais)
        {
            using var httpClient = _httpClient.CreateClient();

            var url = $"https://api.countrystatecity.in/v1/countries/{codPais}/states";

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Headers =
                {
                    { "X-CSCAPI-KEY", _key }
                }
            };

            var result = await httpClient.SendAsync(httpRequestMessage);

            var jsonString = await result.Content.ReadAsStringAsync();

            if (jsonString.Contains("error")) return default;

            var estados = JsonConvert.DeserializeObject<Estado[]>(jsonString);

            return estados;
        }

        public async Task<Cidade[]> ObterCidades(string codPais, string codEstado)
        {
            using var httpClient = _httpClient.CreateClient();

            var url = $"https://api.countrystatecity.in/v1/countries/{codPais}/states/{codEstado}/cities";

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Headers =
                {
                    { "X-CSCAPI-KEY", _key }
                }
            };

            var result = await httpClient.SendAsync(httpRequestMessage);

            var jsonString = await result.Content.ReadAsStringAsync();

            if (jsonString.Contains("error")) return default;

            var cidades = JsonConvert.DeserializeObject<Cidade[]>(jsonString);

            return cidades;
        }

    }
}

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Location.IpGeolocation.Dtos;
using Microsoft.Extensions.Options;
using Weather.Abstractions;

namespace Location.IpGeolocation
{
    public class LocationGetter: ILocationGetter
    {
        private readonly HttpClient _client;
        private readonly IpGeolocationConfig _config;

        public LocationGetter(HttpClient client, IOptions<IpGeolocationConfig> config)
        {
            _client = client;
            _config = config.Value;
        }

        public async Task<LocationResult> GetLocation()
        {
            var response = await _client.GetAsync($"{_config.Endpoint}?apiKey={_config.ApiKey}");

            response.EnsureSuccessStatusCode();

            using (var responseAsStream = await response.Content.ReadAsStreamAsync())
            {
                var result = await JsonSerializer.DeserializeAsync<LocationResultDto>(responseAsStream);

                return new LocationResult
                {
                    Country = result.CountryName,
                    City = result.City,
                    Zipcode = result.Zipcode
                };
            }
        }
    }
}

using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Weather.Abstractions;
using Weather.Weatherstack.Dtos;

namespace Weather.Weatherstack
{
    public class WeatherGetter : IWeatherGetter
    {
        private readonly HttpClient _client;
        private readonly WeatherstackConfig _config;

        public WeatherGetter(HttpClient client, IOptions<WeatherstackConfig> config)
        {
            _client = client;
            _config = config.Value;
        }

        public async Task<WeatherResult> GetWeather(string locationQuery)
        {
            var response = await _client.GetAsync($"{_config.Endpoint}/current?access_key={_config.ApiKey}&query={locationQuery}");

            response.EnsureSuccessStatusCode();

            using (var responseAsStream = await response.Content.ReadAsStreamAsync())
            {
                var result = await JsonSerializer.DeserializeAsync<WeatherResultDto>(responseAsStream);

                return new WeatherResult
                {
                    TempCelsius = result?.Current?.Temperature,
                    Description = string.Join(", ", result?.Current?.WeatherDescriptions ?? new string[0]),
                    Humidity = result?.Current?.Humidity,
                    Pressure = result?.Current?.Pressure,
                    Location = $"{result?.Location?.Name}, {result?.Location?.Region}, {result?.Location?.Country}"
                };
            }
        }
    }
}

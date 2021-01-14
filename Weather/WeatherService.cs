using System.Threading.Tasks;
using Weather.Abstractions;

namespace Weather
{
    public class WeatherService : IWeatherService
    {
        private readonly ILocationGetter _locationGetter;
        private readonly IWeatherGetter _weatherGetter;

        public WeatherService(ILocationGetter locationGetter, IWeatherGetter weatherGetter)
        {
            _locationGetter = locationGetter;
            _weatherGetter = weatherGetter;
        }

        public async Task<WeatherResult> GetWeatherByLocation(string locationQuery = null)
        {
            if (string.IsNullOrEmpty(locationQuery))
            {
                var locationResult = await _locationGetter.GetLocation();
                locationQuery = $"{locationResult.City}, {locationResult.Country}, {locationResult.Zipcode}";
            }

            var result = await _weatherGetter.GetWeather(locationQuery);

            return result;
        }
    }
}
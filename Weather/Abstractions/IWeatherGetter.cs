using System.Threading.Tasks;

namespace Weather.Abstractions
{
    public interface IWeatherGetter
    {
        Task<WeatherResult> GetWeather(string locationQuery);
    }
}
using System.Threading.Tasks;

namespace Weather.Abstractions
{
    public interface IWeatherService
    {
        Task<WeatherResult> GetWeatherByLocation(string query = null);
    }
}
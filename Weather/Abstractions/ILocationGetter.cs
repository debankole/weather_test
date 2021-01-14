using System.Threading.Tasks;

namespace Weather.Abstractions
{
    public interface ILocationGetter
    {
        Task<LocationResult> GetLocation();
    }
}

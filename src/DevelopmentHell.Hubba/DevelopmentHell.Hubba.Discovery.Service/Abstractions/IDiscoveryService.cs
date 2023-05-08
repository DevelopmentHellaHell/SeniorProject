using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Discovery.Service.Abstractions
{
    public interface IDiscoveryService
    {
        Task<Result<Dictionary<string, List<Dictionary<string, object>>?>>> GetCurated(int offset);
        Task<Result<List<Dictionary<string, object>>>> GetSearch(string query, string category, string filter, int offset);
    }
}

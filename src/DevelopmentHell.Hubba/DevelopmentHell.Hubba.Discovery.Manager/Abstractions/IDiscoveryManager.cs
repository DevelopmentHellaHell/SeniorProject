using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Discovery.Manager.Abstractions
{
	public interface IDiscoveryManager
	{
		Task<Result<Dictionary<string, List<Dictionary<string, object>>?>>> GetCurated(int offset);
	}
}

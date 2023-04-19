using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
	public interface IListingsDataAccess
	{
		Task<Result<List<Dictionary<string, object>>>> CurateListings(int offset = 0);
		Task<Result<List<Dictionary<string, object>>>> SearchListings(string query, int offset = 0, double FTTWeight = 0.5, double RWeight = 0.25, double RCWeight = 0.25);
	}
}

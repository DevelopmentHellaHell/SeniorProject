using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public class ListingsDataAccess : IListingsDataAccess
	{
		private readonly ExecuteDataAccess _executeDataAccess;

		public ListingsDataAccess(string connectionString) {
			_executeDataAccess = new ExecuteDataAccess(connectionString);
		}

		public async Task<Result<List<Dictionary<string, object>>>> CurateListings(int offset = 0) {
			var result = await _executeDataAccess.Execute("CurateListings", new Dictionary<string, object>() {
				{ "Offset", offset },
			}).ConfigureAwait(false);

			return result;
		}

		public async Task<Result<List<Dictionary<string, object>>>> SearchListings(string query, int offset = 0, double FTTWeight = 0.5, double RWeight = 0.25, double RCWeight = 0.25)
		{
			var result = await _executeDataAccess.Execute("SearchListings", new Dictionary<string, object>()
			{
				{ "Query", query },
				{ "Offset", offset },
				{ "FTTableRankWeight", FTTWeight },
				{ "RatingsRankWeight", RWeight },
				{ "RatingsCountRankWeight", RCWeight },
			}).ConfigureAwait(false);

			return result;
		}
	}
}

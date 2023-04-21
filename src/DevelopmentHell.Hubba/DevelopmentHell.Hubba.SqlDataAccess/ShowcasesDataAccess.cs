using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public class ProjectShowcasesDataAccess : IProjectShowcasesDataAccess
	{
		private readonly ExecuteDataAccess _executeDataAccess;

		public ProjectShowcasesDataAccess(string connectionString)
		{
			_executeDataAccess = new ExecuteDataAccess(connectionString);
		}

		public async Task<Result<List<Dictionary<string, object>>>> Curate(int offset = 0)
		{
			var result = await _executeDataAccess.Execute("CurateShowcases", new Dictionary<string, object>() {
				{ "Offset", offset },
			}).ConfigureAwait(false);

			return result;
		}

		public async Task<Result<List<Dictionary<string, object>>>> Search(string query, int offset = 0, double FTTWeight = 0.5, double RWeight = 0.5)
		{
			var result = await _executeDataAccess.Execute("SearchShowcases", new Dictionary<string, object>()
			{
				{ "Query", query },
				{ "Offset", offset },
				{ "FTTableRankWeight", FTTWeight },
				{ "RatingsRankWeight", RWeight },
			}).ConfigureAwait(false);

			return result;
		}
	}
}

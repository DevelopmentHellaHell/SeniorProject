using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
	public interface IProjectShowcasesDataAccess
	{
		Task<Result<List<Dictionary<string, object>>>> Curate(int offset = 0);
		Task<Result<List<Dictionary<string, object>>>> Search(string query, int offset = 0, double FTTWeight = 0.5, double RWeight = 0.5);
	}
}

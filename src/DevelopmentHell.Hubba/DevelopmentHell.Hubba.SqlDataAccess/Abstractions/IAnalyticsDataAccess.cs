using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
	public interface IAnalyticsDataAccess
	{
		Task<Result<List<Dictionary<string, object>>>> GetDailyTotal(string item, DateTime fromTime);
	}
}

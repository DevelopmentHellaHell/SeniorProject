using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
	public interface ILoggerDataAccess
	{
		Task<Result> LogData(LogLevel logLevel, Category category, string userName, string message);
		Task<Result<List<Dictionary<string, object>>>> SelectLogs(List<string> columns, List<Comparator> filters);
	}
}

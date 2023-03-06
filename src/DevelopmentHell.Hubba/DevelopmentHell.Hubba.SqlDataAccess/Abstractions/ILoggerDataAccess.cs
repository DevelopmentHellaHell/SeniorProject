using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
	public interface ILoggerDataAccess
	{
		Task<Result> LogData(LogLevel logLevel, Category category, string message, string? userName = null);
		Task<Result<List<Dictionary<string, object>>>> SelectLogs(List<string> columns, List<Comparator> filters);
	}
}

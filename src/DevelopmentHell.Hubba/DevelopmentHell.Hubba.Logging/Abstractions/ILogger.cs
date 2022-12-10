using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Logging.Service.Abstractions
{
	public interface ILogger
	{
		Task<Result> Log(LogLevel level, string userName, string message);
	}
}

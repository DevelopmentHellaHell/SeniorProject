using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Logging.Service.Abstractions
{
	public interface ILoggerService
	{
		public Result Log(LogLevel level, Category category, string message, string userName = "");
	}
}

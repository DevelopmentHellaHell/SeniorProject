using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Logging.Service.Abstractions
{
    public interface ILoggerService
    {
        public Result Log(LogLevel level, Category category, string message, string? userName = null);
        public Result Error(Category category, string message, string? userName = null);
        public Result Warning(Category category, string message, string? userName = null);
        public Result Info(Category category, string message, string? userName = null);
        public Result Debug(Category category, string message, string? userName = null);
    }
}

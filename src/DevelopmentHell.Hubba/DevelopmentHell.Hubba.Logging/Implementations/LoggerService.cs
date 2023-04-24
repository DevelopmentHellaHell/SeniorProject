using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.Logging.Service.Implementations
{
    public class LoggerService : ILoggerService
    {
        private readonly ILoggerDataAccess _LoggerDataAccess;

        public LoggerService(ILoggerDataAccess loggerDataAccess)
        {
            _LoggerDataAccess = loggerDataAccess;
        }

        public Result Log(LogLevel logLevel, Category category, string message, string? userName = null)
        {
            Result result = new Result();

            if (message == null)
            {
                result.IsSuccessful = true;
                return result;
            }

            if (userName is not null && userName.Length > 256)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Logging user was over 256 characters.";
                return result;
            }

            try
            {
                var dataAccessResult = _LoggerDataAccess.LogData(logLevel, category, message, userName);
                result.IsSuccessful = true;
            }
            catch (Exception e)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = e.Message;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public Result Error(Category category, string message, string? userName = null) => Log(LogLevel.ERROR, category, message, userName);
        public Result Warning(Category category, string message, string? userName = null) => Log(LogLevel.WARNING, category, message, userName);
        public Result Info(Category category, string message, string? userName = null) => Log(LogLevel.INFO, category, message, userName);
        public Result Debug(Category category, string message, string? userName = null) => Log(LogLevel.DEBUG, category, message, userName);
    }
}
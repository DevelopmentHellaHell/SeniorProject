using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.Notification.Service.Implementations
{
    public class NotificationService : INotificationService
    {
        private INotificationDataAccess _notificationDataAccess;
        private ILoggerService _loggerService;
        public NotificationService(INotificationDataAccess notificationDataAccess, ILoggerService loggerService)
        {
            _notificationDataAccess = notificationDataAccess;
            _loggerService = loggerService;
        }
        public async Task<Result> AddNotification(int id, string message, NotificationType tag)
        {
            Result result = new Result();
            if (message.Length > 256)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Notification message must be less than 256 characters.";
                return result;
            }

            return await _notificationDataAccess.AddNotification(id, message, tag).ConfigureAwait(false);
        }
    }
}
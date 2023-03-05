using DevelopmentHell.Hubba.Emailing.Service;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Manager.Abstractions;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;

namespace DevelopmentHell.Hubba.Notification.Manager.Implementations
{
    public class NotificationManager : INotificationManager
    {
        private INotificationService _notificationService;
        private ILoggerService _loggerService;
        public NotificationManager(INotificationService notificationService, ILoggerService loggerService) 
        {
            _notificationService = notificationService;
            _loggerService = loggerService;
        }

        public async Task<Result> CreateNewNotification(int id, string message, NotificationType tag)
        {
            //EmailService.SendEmail()
            return await _notificationService.AddNotification(id, message, tag).ConfigureAwait(false);
        }
    }
}

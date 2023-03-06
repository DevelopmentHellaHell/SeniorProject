using DevelopmentHell.Hubba.CellPhoneProvider.Service.Abstractions;
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
        private ICellPhoneProviderService _cellPhoneProviderService;
        private ILoggerService _loggerService;

        public NotificationManager(INotificationService notificationService, ICellPhoneProviderService cellPhoneProviderService, ILoggerService loggerService) 
        {
            _notificationService = notificationService;
            _cellPhoneProviderService = cellPhoneProviderService;
            _loggerService = loggerService;
        }

        public async Task<Result> CreateNewNotification(int userId, string message, NotificationType tag)
        {
            Result result = new Result();

            Result<NotificationSettings> notificationSettingsResult = await _notificationService.SelectUserNotificationSettings(userId).ConfigureAwait(false);
            if (!notificationSettingsResult.IsSuccessful || notificationSettingsResult is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error fetching notification settings.";
                return result;
            }
            
            Result<UserAccount> userResult = await _notificationService.GetUser(userId).ConfigureAwait(false);
            if (!userResult.IsSuccessful || userResult is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error fetching user.";
                return result;
            }

            NotificationSettings notificationSettings = notificationSettingsResult.Payload!;
            UserAccount userAccount = userResult.Payload!;
            if ((bool)notificationSettings.EmailNotifications!
                && userAccount.Email is not null)
            {
                string userEmail = userAccount.Email;

                Result emailResult = EmailService.SendEmail(
                    userEmail,
                    $"Hubba Notification - {tag}",
                    message,
                    true
                );
                if (!emailResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = "Could not send email notification.";
                    return result;
                }
            }
            
            if ((bool)notificationSettings.TextNotifications!
                && userAccount.CellPhoneNumber is not null
                && userAccount.CellPhoneProvider is not null)
            {
                string providerEmail = userAccount.CellPhoneNumber + _cellPhoneProviderService.GetProviderEmail((CellPhoneProviders)userAccount.CellPhoneProvider);

                Result emailResult = EmailService.SendEmail(
                    providerEmail,
                    $"Hubba Notification - {tag}",
                    message,
                    true
                );
                if (!emailResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = "Could not send text notification.";
                    return result;
                }
            }

            return await _notificationService.AddNotification(userId, message, tag).ConfigureAwait(false);
        }
    }
    //TODO: task<result> UpdateNotificationSettings(NotificationSettings settings) -> NotificationSettingsDA
    //TODO: method to get notifications list

}

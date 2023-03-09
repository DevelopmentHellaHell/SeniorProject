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

        //Retrieves preferences from user
        public async Task<Result<NotificationSettings>> GetNotificationSettings(int userId)
        {
            return await _notificationService.SelectUserNotificationSettings(userId).ConfigureAwait(false);
        }

        public async Task<Result> CreateNewNotification(int userId, string message, NotificationType tag, bool supressTextNotification = false)
        {
            Result result = new Result();

            Result<NotificationSettings> notificationSettingsResult = await GetNotificationSettings(userId).ConfigureAwait(false);
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
                && userAccount.CellPhoneProvider is not null
                && !supressTextNotification)
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

        public async Task<Result<List<Dictionary<string, object>>>> GetNotificaions(int userId)
        {
            return await _notificationService.GetNotifications(userId).ConfigureAwait(false);
        }

        public async Task<Result> UpdateNotificationSettings(NotificationSettings settings)
        {
            Result result = new Result();

            Result updateNotificationResult = await _notificationService.UpdateNotificationSettings(settings).ConfigureAwait(false);
            if (!updateNotificationResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = updateNotificationResult.ErrorMessage;
                return result;
            }

            string message = "Your Notification Settings have been changed.";
            Result createNotificationResult = await CreateNewNotification(
                settings.UserId,
                message,
                NotificationType.OTHER,
                true
            ).ConfigureAwait(false);

            if (!createNotificationResult.IsSuccessful) {
                result.IsSuccessful = false;
                result.ErrorMessage = createNotificationResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> HideNotifications(int userId)
        {
            return await _notificationService.HideNotifications(userId).ConfigureAwait(false);
        }

        public async Task<Result> HideIndividualNotifications(List<int> selectedNotifications)
        {
            return await _notificationService.HideIndividualNotifications(selectedNotifications).ConfigureAwait(false);
        }

        public async Task<Result> DeleteNotificationSettings(int userId)
        {
            return await _notificationService.DeleteNotificationSettings(userId).ConfigureAwait(false);
        }

        public async Task<Result> DeleteAllNotifications(int userId)
        {
            return await _notificationService.DeleteAllNotifications(userId).ConfigureAwait(false);
        }
    }
    

    //TODO: method to get notifications list

}

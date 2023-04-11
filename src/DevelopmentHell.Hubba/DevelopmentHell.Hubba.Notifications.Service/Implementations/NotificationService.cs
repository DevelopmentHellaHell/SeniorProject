using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.Notification.Service.Implementations
{
    public class NotificationService : INotificationService
    {
        private INotificationDataAccess _notificationDataAccess;
        private INotificationSettingsDataAccess _notificationSettingsDataAccess;
        private IUserAccountDataAccess _userAccountDataAccess;
        private ILoggerService _loggerService;

        public NotificationService(INotificationDataAccess notificationDataAccess, INotificationSettingsDataAccess notificationSettingsDataAccess, IUserAccountDataAccess userAccountDataAccess, ILoggerService loggerService)
        {
            _notificationDataAccess = notificationDataAccess;
            _notificationSettingsDataAccess = notificationSettingsDataAccess;
            _userAccountDataAccess = userAccountDataAccess;
            _loggerService = loggerService;
        }
        public async Task<Result> AddNotification(int userId, string message, NotificationType tag)
        {
            Result result = new Result();
            if (message.Length > 256)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Notification message must be less than 256 characters.";
                return result;
            }

            return await _notificationDataAccess.AddNotification(userId, message, tag).ConfigureAwait(false);
        }

        public async Task<Result<List<Dictionary<string, object>>>> GetNotifications(int userId)
        {
            return await _notificationDataAccess.GetNotifications(userId).ConfigureAwait(false);
        }

        public async Task<Result> CreateNewNotificationSettings(int userId)
        {
            // default settings for new User Accounts
            NotificationSettings settings = new NotificationSettings()
            {
                UserId = userId,
                SiteNotifications = true,
                EmailNotifications = false,
                TextNotifications = false,
                TypeScheduling = true,
                TypeWorkspace = true,
                TypeProjectShowcase = true,
                TypeOther = true
            };

            return await _notificationSettingsDataAccess.CreateUserNotificationSettings(settings).ConfigureAwait(false);
        }

        public async Task<Result<NotificationSettings>> SelectUserNotificationSettings(int userId)
        {
            return await _notificationSettingsDataAccess.SelectUserNotificationSettings(userId).ConfigureAwait(false);
        }

        public async Task<Result> UpdateNotificationSettings(NotificationSettings settings)
        {
            return await _notificationSettingsDataAccess.UpdateUserNotificationSettings(settings).ConfigureAwait(false);
        }

        public async Task<Result> HideAllNotifications(int userId)
        {
            return await _notificationDataAccess.HideAllNotifications(userId).ConfigureAwait(false);
        }

        public async Task<Result> HideIndividualNotifications(List<int> selectedNotifications)
        {
            return await _notificationDataAccess.HideIndividualNotifications(selectedNotifications).ConfigureAwait(false);
        }

        public async Task<Result> DeleteNotificationSettings(int userId)
        {
            return await _notificationSettingsDataAccess.DeleteNotificationSettings(userId).ConfigureAwait(false);
        }

        public async Task<Result> DeleteAllNotifications(int userId)
        {
            return await _notificationDataAccess.DeleteAllNotifications(userId).ConfigureAwait(false);
        }

        public async Task<Result<UserAccount>> GetUser(int userId)
        {
            return await _userAccountDataAccess.GetUser(userId).ConfigureAwait(false);
        }

        public async Task<Result<int>> GetId(string email)
        {
            return await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
        }

        public async Task<Result> UpdatePhoneDetails(UserAccount user)
        {
            return await _userAccountDataAccess.Update(user).ConfigureAwait(false);
        }
    }
}
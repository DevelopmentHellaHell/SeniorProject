using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.CellPhoneProvider.Service.Abstractions;
using DevelopmentHell.Hubba.Email.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Manager.Abstractions;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.Notification.Manager.Implementations
{
    public class NotificationManager : INotificationManager
    {
        private INotificationService _notificationService;
        private ICellPhoneProviderService _cellPhoneProviderService;
        private IEmailService _emailService;
        private IAuthorizationService _authorizationService;
        private ILoggerService _loggerService;

        public NotificationManager(INotificationService notificationService, ICellPhoneProviderService cellPhoneProviderService, IEmailService emailService, IAuthorizationService authorizationService, ILoggerService loggerService) 
        {
            _notificationService = notificationService;
            _cellPhoneProviderService = cellPhoneProviderService;
            _emailService = emailService;
            _authorizationService = authorizationService;
            _loggerService = loggerService;
        }

        //Retrieves preferences from user
        public async Task<Result<NotificationSettings>> GetNotificationSettings()
        {
            Result<NotificationSettings> result = new Result<NotificationSettings>();

            if (!_authorizationService.Authorize(new string[] { "AdminUser", "VerifiedUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized.";
                return result;
            }

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            var accountId = int.Parse(stringAccountId);

            Result<NotificationSettings> getNotificationSettingsResult = await _notificationService.SelectUserNotificationSettings(accountId).ConfigureAwait(false);
            if (!getNotificationSettingsResult.IsSuccessful || getNotificationSettingsResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Could not fetch notification settings.";
                return result;
            }

            result.IsSuccessful = true;
            result.Payload = getNotificationSettingsResult.Payload;
            return result;
        }

        public async Task<Result> CreateNewNotification(int userId, string message, NotificationType tag, bool supressTextNotification = false) //cahnge supress to force email for account changes
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
            if((!(bool)notificationSettings.TypeProjectShowcase! && tag == NotificationType.PROJECT_SHOWCASE)
                || (!(bool)notificationSettings.TypeWorkspace! && tag == NotificationType.WORKSPACE)
                || (!(bool)notificationSettings.TypeScheduling! && tag == NotificationType.SCHEDULING)
                || (!(bool)notificationSettings.TypeOther! && tag == NotificationType.OTHER))
            {
                result.IsSuccessful = true;
                return result;
            } 

            UserAccount userAccount = userResult.Payload!;
            if ((bool)notificationSettings.EmailNotifications!
                && userAccount.Email is not null)
            {
                string userEmail = userAccount.Email;

                Result emailResult = _emailService.SendEmail(
                    userEmail,
                    $"Hubba Notification - {tag}",
                    message
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

                Result emailResult = _emailService.SendEmail(
                    providerEmail,
                    $"Hubba Notification - {tag}",
                    message
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

        public async Task<Result<List<Dictionary<string, object>>>> GetNotifications()
        {
            Result<List<Dictionary<string, object>>> result = new Result<List<Dictionary<string, object>>>();

            if (!_authorizationService.Authorize(new string[] { "AdminUser", "VerifiedUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized.";
                return result;
            }

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            var userId = int.Parse(stringAccountId);

            return await _notificationService.GetNotifications(userId).ConfigureAwait(false);
        }

        public async Task<Result> UpdateNotificationSettings(NotificationSettings settings)
        {
            Result result = new Result();
            if (!_authorizationService.Authorize(new string[] { "AdminUser", "VerifiedUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized.";
                return result;
            }

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            var accountId = int.Parse(stringAccountId);
            settings.UserId = accountId;

            Result updateNotificationResult = await _notificationService.UpdateNotificationSettings(settings).ConfigureAwait(false);
            if (!updateNotificationResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = updateNotificationResult.ErrorMessage;
                return result;
            }

            string message = "Your Notification Settings have been changed.";
            Result createNotificationResult = await CreateNewNotification(
                (int)settings.UserId!,
                message,
                NotificationType.OTHER
            ).ConfigureAwait(false);

            if (!createNotificationResult.IsSuccessful) {
                result.IsSuccessful = false;
                result.ErrorMessage = createNotificationResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> HideAllNotifications()
        {
            Result result = new Result();
            if (!_authorizationService.Authorize(new string[] { "AdminUser", "VerifiedUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized.";
                return result;
            }

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            var userId = int.Parse(stringAccountId);

            return await _notificationService.HideAllNotifications(userId).ConfigureAwait(false);
        }

        public async Task<Result> HideIndividualNotifications(List<int> selectedNotifications)
        {
            Result result = new Result();
            if (!_authorizationService.Authorize(new string[] { "AdminUser", "VerifiedUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized.";
                return result;
            }

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

        public async Task<Result<UserAccount>> GetPhoneDetails()
        {
            Result<UserAccount> result = new Result<UserAccount>();

            if (!_authorizationService.Authorize(new string[] { "AdminUser", "VerifiedUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized.";
                return result;
            }

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            var userId = int.Parse(stringAccountId);

            Result<UserAccount> getUserResult = await _notificationService.GetUser(userId).ConfigureAwait(false);
            if (!getUserResult.IsSuccessful || getUserResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error fetching user.";
                return result;
            }

            result.IsSuccessful = true;
            result.Payload = new UserAccount()
            {
                CellPhoneNumber = getUserResult.Payload.CellPhoneNumber,
                CellPhoneProvider = getUserResult.Payload.CellPhoneProvider
            };
            return result;
        }

        public async Task<Result> UpdatePhoneDetails(string? cellPhoneNumber, CellPhoneProviders? cellPhoneProvider)
        {
            Result result = new Result();

            if (!_authorizationService.Authorize(new string[] { "AdminUser", "VerifiedUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized.";
                return result;
            }

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            var userId = int.Parse(stringAccountId);

            return await _notificationService.UpdateUser(new UserAccount()
            {
                Id = userId,
                CellPhoneNumber= cellPhoneNumber,
                CellPhoneProvider = cellPhoneProvider
            }).ConfigureAwait(false);
        }
    }
}

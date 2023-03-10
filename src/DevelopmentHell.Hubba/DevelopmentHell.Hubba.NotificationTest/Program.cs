using DevelopmentHell.Hubba.CellPhoneProvider.Service.Implementations;
using DevelopmentHell.Hubba.Email.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Manager.Implementations;
using DevelopmentHell.Hubba.Notification.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using System.Configuration;

var loggerService = new LoggerService(
    new LoggerDataAccess(
        ConfigurationManager.AppSettings["LogsConnectionString"]!,
        ConfigurationManager.AppSettings["LogsTable"]!
    )
);
var notificationManager = new NotificationManager(
    new NotificationService(
        new NotificationDataAccess(
            ConfigurationManager.AppSettings["NotificationsConnectionString"]!,
            ConfigurationManager.AppSettings["UserNotificationsTable"]!
        ),
        new NotificationSettingsDataAccess(
            ConfigurationManager.AppSettings["NotificationsConnectionString"]!,
            ConfigurationManager.AppSettings["NotificationSettingsTable"]!
        ),
        new UserAccountDataAccess(
            ConfigurationManager.AppSettings["UsersConnectionString"]!,
            ConfigurationManager.AppSettings["UserAccountsTable"]!
        ),
        loggerService
    ),
    new CellPhoneProviderService(),
    new EmailService(
        ConfigurationManager.AppSettings["SENDGRID_USERNAME"]!,
        ConfigurationManager.AppSettings["SENDGRID_API_KEY"]!,
        ConfigurationManager.AppSettings["COMPANY_EMAIL"]!
    ),
    loggerService
);

Result result = await notificationManager.CreateNewNotification(9, "hi", NotificationType.OTHER).ConfigureAwait(false);

//Result<NotificationSettings> settingsResult = await notificationManager.GetNotificationSettings(8).ConfigureAwait(false);

//NotificationSettings settings = settingsResult.Payload!;
//settings.TypeProjectShowcase = false;
//settings.TextNotifications = false;
//settings.SiteNotifications = false;
//Result result = await notificationManager.UpdateNotificationSettings(settings);

//Result result = await notificationManager.HideNotifications(8);

Console.WriteLine(result.IsSuccessful);
Console.WriteLine(result.ErrorMessage);
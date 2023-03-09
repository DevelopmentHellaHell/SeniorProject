using DevelopmentHell.Hubba.SqlDataAccess;
using System.Configuration;
using System.Drawing.Text;

namespace DevelopmentHell.Hubba.Notification.Test
{
    [TestClass]
    public class NotificationUnitTests
    {
        private readonly NotificationDataAccess _notificationDataAccess;
        private readonly NotificationSettingsDataAccess _notificationSettingsDataAccess;

        private string _notificationConnectionString = ConfigurationManager.AppSettings["NotificationsConnectionString"]!;
        private string _notificationTable = ConfigurationManager.AppSettings["UserNotificationsTable"]!;
        private string _notificationSettingsTable = ConfigurationManager.AppSettings["NotificationSettingsTable"]!;
       public NotificationUnitTests()
        {
            _notificationDataAccess = new NotificationDataAccess(_notificationConnectionString, _notificationTable);
            _notificationSettingsDataAccess = new NotificationSettingsDataAccess(_notificationConnectionString, _notificationSettingsTable);
        }

        
        [TestMethod]
        public void CreateUserNotification()
        {
        }
    }
}
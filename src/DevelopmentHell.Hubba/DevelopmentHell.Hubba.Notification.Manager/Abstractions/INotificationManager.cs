using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Notification.Manager.Abstractions
{
    public interface INotificationManager
    {
        Task<Result<NotificationSettings>> GetNotificationSettings();
        Task<Result<List<Dictionary<string, object>>>> GetNotifications();
        Task<Result> CreateNewNotification(int userId, string message, NotificationType tag, bool supressTextNotification = false);
        Task<Result> UpdateNotificationSettings(NotificationSettings settings);
        Task<Result> HideAllNotifications();
        Task<Result> HideIndividualNotifications(List<int> selectedNotifications);
        Task<Result> DeleteNotificationSettings(int userId);
        Task<Result> DeleteAllNotifications(int userId);
    }
}

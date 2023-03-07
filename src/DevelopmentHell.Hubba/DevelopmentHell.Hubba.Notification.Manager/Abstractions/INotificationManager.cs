using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Notification.Manager.Abstractions
{
    public interface INotificationManager
    {
        Task<Result<NotificationSettings>> GetNotificationSettings(int id);
        Task<Result> CreateNewNotification(int id, string message, NotificationType tag);
        Task<Result> UpdateNotificationSettings(NotificationSettings settings);
        Task<Result> DeleteNotification(List<Dictionary<string, object>> selectedNotifications);
        Task<Result> ClearAllNotifications(int userId);
    }
}

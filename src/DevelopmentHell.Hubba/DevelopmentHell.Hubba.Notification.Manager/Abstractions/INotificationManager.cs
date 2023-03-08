using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Notification.Manager.Abstractions
{
    public interface INotificationManager
    {
        Task<Result<NotificationSettings>> GetNotificationSettings(int id);
        Task<Result> CreateNewNotification(int id, string message, NotificationType tag, bool supressTextNotification = false);
        Task<Result<List<Dictionary<string, object>>>> GetNotificaions(int userId);
        Task<Result> UpdateNotificationSettings(NotificationSettings settings);
        //Task<Result> DeleteNotification(List<Dictionary<string, object>> selectedNotifications);
        Task<Result> HideNotifications(int userId);
        Task<Result> DeleteNotificationSettings(int userId);
        Task<Result> DeleteAllNotifications(int userId);
    }
}

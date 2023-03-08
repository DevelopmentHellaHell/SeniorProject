using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Notification.Service.Abstractions
{
    public interface INotificationService
    {
        Task<Result> AddNotification(int userId, string message, NotificationType tag);
        Task<Result> CreateNewNotificationSettings(NotificationSettings settings);
        Task<Result<NotificationSettings>> SelectUserNotificationSettings(int userId);
        Task<Result> UpdateNotificationSettings(NotificationSettings settings);
        Task<Result> ClearNotifications(int userId);
        Task<Result<UserAccount>> GetUser(int userId);
        Task<Result<int>> GetId(string email);
    }
}

using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Notification.Service.Abstractions
{
    public interface INotificationService
    {
        Task<Result> AddNotification(int userId, string message, NotificationType tag);
        Task<Result<List<Dictionary<string, object>>>> GetNotifications(int userId);
        Task<Result> CreateNewNotificationSettings(NotificationSettings settings);
        Task<Result<NotificationSettings>> SelectUserNotificationSettings(int userId);
        Task<Result> UpdateNotificationSettings(NotificationSettings settings);
        Task<Result> HideAllNotifications(int userId);
        Task<Result> HideIndividualNotifications(List<int> selectedNotifications);
        Task<Result> DeleteNotificationSettings(int userId);
        Task<Result> DeleteAllNotifications(int userId);
        Task<Result<UserAccount>> GetUser(int userId);
        Task<Result<int>> GetId(string email);
    }
}

using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface INotificationSettingsDataAccess
    {
        Task<Result> CreateUserNotificationSettings(NotificationSettings settings);
        Task<Result> UpdateUserNotificationSettings(NotificationSettings settings);
        Task<Result<NotificationSettings>> SelectUserNotificationSettings(int userId);
        Task<Result> DeleteNotificationSettings(int userId);
    }
}

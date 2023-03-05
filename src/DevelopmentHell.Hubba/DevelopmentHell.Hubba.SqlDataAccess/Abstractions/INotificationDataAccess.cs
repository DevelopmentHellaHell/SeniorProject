using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface INotificationDataAccess
    {
        Task<Result> AddNotification(int id, string message, NotificationType tag);
    }
}

using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Notification.Manager.Abstractions
{
    public interface INotificationManager
    {
        Task<Result> CreateNewNotification(int id, string message, NotificationType tag);
    }
}

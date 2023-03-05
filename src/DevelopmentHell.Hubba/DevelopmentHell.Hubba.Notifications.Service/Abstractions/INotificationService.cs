using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Notification.Service.Abstractions
{
    public interface INotificationService
    {
        Task<Result> AddNotification(int id, string message, NotificationType tag);
    }
}

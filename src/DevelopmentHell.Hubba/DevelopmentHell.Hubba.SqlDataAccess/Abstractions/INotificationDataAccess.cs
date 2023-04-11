using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
	public interface INotificationDataAccess
	{
		Task<Result> AddNotification(int userId, string message, NotificationType tag);
		Task<Result<List<Dictionary<string, object>>>> GetNotifications(int userId);
		Task<Result> HideIndividualNotifications(List<int> selectedNotifications);
		Task<Result> HideAllNotifications(int userId);

		Task<Result> DeleteAllNotifications(int userId);
	}
}

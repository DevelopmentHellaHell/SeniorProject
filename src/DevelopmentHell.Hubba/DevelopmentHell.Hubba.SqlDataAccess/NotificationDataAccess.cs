using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class NotificationDataAccess : INotificationDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private string _tableName;

        public NotificationDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _tableName = tableName;
        }

        public async Task<Result> AddNotification(int id, string message, NotificationType tag)
        {
            Result insertResult = await _insertDataAccess.Insert(
                _tableName,
                new Dictionary<string, object>()
                {
                    { "UserId", id },
                    { "Tag", tag },
                    { "Message", message },
                    { "Hide", false },
                    { "DateCreated", DateTime.Now }
                }
            ).ConfigureAwait(false);

            return insertResult;
        }
    }
}

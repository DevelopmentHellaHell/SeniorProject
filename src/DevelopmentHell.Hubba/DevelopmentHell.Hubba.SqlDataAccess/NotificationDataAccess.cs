using Azure.Core;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using System.ComponentModel.DataAnnotations;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class NotificationDataAccess : INotificationDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private string _tableName;

        public NotificationDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(tableName);
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

        public async Task<Result<List<Dictionary<string, object>>>> GetNotifications(int id)
        {
            Result<List<Dictionary<string, object>>> result = new Result<List<Dictionary<string, object>>>();
            
            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "*" },
                new List<Comparator>
                {
                    new Comparator("UserId", "=", id),
                    new Comparator("Hide", "=", 0)
                }
            ).ConfigureAwait(false);

            if  (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;

            result.IsSuccessful = true;
            if (payload.Count > 0) result.Payload = payload;
            return result;
        }

        //TODO: should I still call it Delete Notification even though we don't delete it? DateTime datatype?
        public async Task<Result> DeleteNotification(List<Dictionary<string, object>> selectedNotifications)
        {

        }

        //Makes all notifications of user hidden
        public async Task<Result> ClearAllNotifications(int userId)
        {

        }
    }
}

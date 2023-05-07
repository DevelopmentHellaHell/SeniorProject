using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class NotificationDataAccess : INotificationDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private string _tableName;

        public NotificationDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _tableName = tableName;
        }

        public async Task<Result> AddNotification(int userId, string message, NotificationType tag)
        {
            Result insertResult = await _insertDataAccess.Insert(
                _tableName,
                new Dictionary<string, object>()
                {
                    { "UserId", userId },
                    { "Tag", tag },
                    { "Message", message },
                    { "Hide", false },
                    { "DateCreated", DateTime.Now }
                }
            ).ConfigureAwait(false);

            return insertResult;
        }

        public async Task<Result<List<Dictionary<string, object>>>> GetNotifications(int userId)
        {
            Result<List<Dictionary<string, object>>> result = new Result<List<Dictionary<string, object>>>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "*" },
                new List<Comparator>
                {
                    new Comparator("UserId", "=", userId),
                    new Comparator("Hide", "=", 0)
                },
                "",
                "",
                50
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;

            //whats going on here: need explanation
            result.IsSuccessful = true;
            if (payload.Count > 0) result.Payload = payload;
            return result;
        }

        public async Task<Result> HideIndividualNotifications(List<int> selectedNotifications)
        {
            Result result = new Result();

            foreach (int i in selectedNotifications)
            {
                Result updateResult = await _updateDataAccess.Update(
                    _tableName,
                    new List<Comparator>()
                    {
                        new Comparator("NotificationId", "=", i)
                    },
                    new Dictionary<string, object>()
                    {
                        {"Hide", true }
                    }
                ).ConfigureAwait(false);
                if (!updateResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = updateResult.ErrorMessage;
                    return updateResult;
                }
            }

            result.IsSuccessful = true;
            return result;

        }

        // Makes all notifications of user hidden
        public async Task<Result> HideAllNotifications(int userId) //change to HideAll
        {
            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("UserId", "=", userId),
                },
                new Dictionary<string, object>()
                {
                    { "Hide", true }
                }
            ).ConfigureAwait(false);

            return updateResult;
        }

        public async Task<Result> DeleteAllNotifications(int userId)
        {
            Result deleteResult = await _deleteDataAccess.Delete(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("UserId", "=", userId),
                }
            ).ConfigureAwait(false);

            return deleteResult;
        }
    }
}

using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class NotificationSettingsDataAccess : INotificationSettingsDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private string _tableName;

        public NotificationSettingsDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _tableName = tableName;
        }

        // used to create default Notification Setting values for a new account
        public async Task<Result> CreateUserNotificationSettings(NotificationSettings settings)
        {
            Result insertResult = await _insertDataAccess.Insert(
                _tableName,
                new Dictionary<string, object>()
                {
                    { "UserId", settings.UserId },
                    { "SiteNotifications",  settings.SiteNotifications! },
                    { "EmailNotifications", settings.EmailNotifications! },
                    { "TextNotifications", settings.TextNotifications! },
                    { "TypeWorkspace", settings.TypeWorkspace! },
                    { "TypeScheduling", settings.TypeScheduling! },
                    { "TypeProjectShowcase", settings.TypeProjectShowcase! },
                    { "TypeOther", settings.TypeOther! }
                }
                ).ConfigureAwait(false);

            return insertResult;
        }

        // Updates user's changed settings for Notifications
        public async Task<Result> UpdateUserNotificationSettings(NotificationSettings settings)
        {
            var values = new Dictionary<string, object>();
            foreach (var column in settings.GetType().GetProperties())
            {
                var value = column.GetValue(settings);
                // ensures proper values get assigned correctly
                if (value is null || column.Name == "UserId") continue;
                values[column.Name] = value;
            }

            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                // comparator helps create WHERE SQL statement
                new List<Comparator>()
                {
                    new Comparator("UserId", "=", settings.UserId),
                },
                values
            ).ConfigureAwait(false);

            return updateResult;
        }

        // used to retrieve user's saved Notification Settings
        public async Task<Result<NotificationSettings>> SelectUserNotificationSettings(int userId)
        {
            Result<NotificationSettings> result = new Result<NotificationSettings>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "*" },
                new List<Comparator>()
                {
                    new Comparator("UserId", "=", userId),
                }
            ).ConfigureAwait(false);
            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 1)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid number of Notification Settings selected.";
                return result;
            }

            result.IsSuccessful = true;
            if (payload.Count > 0) result.Payload = new NotificationSettings()
            {
                UserId = (int)payload.First()["UserId"],
                SiteNotifications = (bool)payload.First()["SiteNotifications"],
                EmailNotifications = (bool)payload.First()["EmailNotifications"],
                TextNotifications = (bool)payload.First()["TextNotifications"],
                TypeScheduling = (bool)payload.First()["TypeScheduling"],
                TypeWorkspace = (bool)payload.First()["TypeWorkspace"],
                TypeProjectShowcase = (bool)payload.First()["TypeProjectShowcase"],
                TypeOther = (bool)payload.First()["TypeOther"]
            };
            return result;
        }

        public async Task<Result> DeleteNotificationSettings(int userId)
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

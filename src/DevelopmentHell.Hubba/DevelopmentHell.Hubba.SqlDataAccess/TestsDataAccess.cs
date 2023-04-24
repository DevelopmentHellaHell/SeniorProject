using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.Tests;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System.Configuration;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class TestsDataAccess
    {
        private static readonly Dictionary<Databases, Tuple<string, string, Dictionary<Tables, string>>> _databaseStructure = new()
        {
            {
                Databases.LOGS,
                new (
                    "DevelopmentHell.Hubba.Logs",
                    ConfigurationManager.AppSettings["LogsConnectionString"]!,
                    new ()
                    {
                        { Tables.LOGS, ConfigurationManager.AppSettings["LogsTable"]! }
                    }
                )
            },
            {
                Databases.USERS,
                new (
                    "DevelopmentHell.Hubba.Users",
                    ConfigurationManager.AppSettings["UsersConnectionString"]!,
                    new ()
                    {
                        { Tables.RECOVERY_REQUESTS, ConfigurationManager.AppSettings["RecoveryRequestsTable"]! },
                        { Tables.USER_ACCOUNTS, ConfigurationManager.AppSettings["UserAccountsTable"]! },
                        { Tables.USER_LOGINS, ConfigurationManager.AppSettings["UserLoginsTable"]! },
                        { Tables.USER_OTPS, ConfigurationManager.AppSettings["UserOTPsTable"]! }
                    }
                )
            },
            {
                Databases.NOTIFICATIONS,
                new (
                    "DevelopmentHell.Hubba.Notifications",
                    ConfigurationManager.AppSettings["NotificationsConnectionString"]!,
                    new ()
                    {
                        { Tables.USER_NOTIFICATIONS, ConfigurationManager.AppSettings["UserNotificationsTable"]! },
                        { Tables.NOTIFICATION_SETTINGS, ConfigurationManager.AppSettings["NotificationSettingsTable"]! }
                    }
                )
            },
            {
                Databases.COLLABORATOR_PROFILES,
                new (
                    "DevelopmentHell.Hubba.CollaboratorProfiles",
                    ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                    new ()
                    {
                        { Tables.COLLABORATOR_FILE_JUNCTION, ConfigurationManager.AppSettings["CollaboratorFileJunctionTable"]! },
                        { Tables.COLLABORATORS, ConfigurationManager.AppSettings["CollaboratorsTable"]! },
                        { Tables.COLLABORATOR_FILES, ConfigurationManager.AppSettings["CollaboratorFilesTable"]! },
                        { Tables.USER_VOTES, ConfigurationManager.AppSettings["CollaboratorUserVotesTable"]! }
                    }
                )
            },
            {
                Databases.LISTING_PROFILES,
                new (
                    "DevelopmentHell.Hubba.ListingProfiles",
                    ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!,
                    new ()
                    {
                        { Tables.LISTING_RATINGS, ConfigurationManager.AppSettings["ListingRatingsTable"]! },
                        { Tables.LISTING_HISTORY, ConfigurationManager.AppSettings["ListingHistoryTable"]! },
                        { Tables.LISTINGS, ConfigurationManager.AppSettings["ListingsTable"]! },
                        { Tables.LISTING_AVAILABILITIES, ConfigurationManager.AppSettings["ListingAvailabilitiesTable"]! }
                    }
                )
            },
        };

        public TestsDataAccess() { }

        public async Task<Result> DeleteDatabaseRecords(Databases db)
        {
            Result result = new Result();
            var dbT = _databaseStructure[db];
            foreach (string tValue in dbT.Item3.Values)
            {
                DeleteDataAccess deleteDataAccess = new DeleteDataAccess(_databaseStructure[db].Item2);
                Result deleteResult = await deleteDataAccess.Delete(tValue, null).ConfigureAwait(false);
                if (!deleteResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = deleteResult.ErrorMessage;
                    return result;
                }
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> DeleteTableRecords(Databases db, Tables t)
        {
            var dbT = _databaseStructure[db];
            var tValue = dbT.Item3[t];
            DeleteDataAccess deleteDataAccess = new DeleteDataAccess(_databaseStructure[db].Item2);
            return await deleteDataAccess.Delete(tValue, null).ConfigureAwait(false);
        }

        public async Task<Result> DeleteAllRecords()
        {
            Result result = new Result();
            foreach (Databases db in Enum.GetValues(typeof(Databases)))
            {
                Result deleteResult = await DeleteDatabaseRecords(db).ConfigureAwait(false);
                if (!deleteResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = deleteResult.ErrorMessage;
                    return result;
                }
            }

            result.IsSuccessful = true;
            return result;
        }

        public Databases? GetDatabase(string dbStr)
        {
            foreach (var kvp in _databaseStructure)
            {
                if (Enum.GetName(kvp.Key)!.ToUpper() == dbStr.ToUpper()) return kvp.Key;
            }

            return null;
        }

        public Tables? GetTable(Databases db, string tStr)
        {
            foreach (var kvp in _databaseStructure[db].Item3)
            {
                if (Enum.GetName(kvp.Key)!.ToUpper() == tStr.ToUpper()) return kvp.Key;
            }

            return null;
        }
    }
}

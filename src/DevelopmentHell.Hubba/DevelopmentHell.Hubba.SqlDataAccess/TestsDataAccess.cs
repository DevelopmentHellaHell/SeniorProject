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
	}
}

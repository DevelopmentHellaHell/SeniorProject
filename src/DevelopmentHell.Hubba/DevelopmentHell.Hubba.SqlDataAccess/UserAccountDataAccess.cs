using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public class UserAccountDataAccess : IUserAccountDataAccess
	{
		private InsertDataAccess _insertDataAccess;
		private SelectDataAccess _selectDataAccess;
		private DeleteDataAccess _deleteDataAccess;
		private UpdateDataAccess _updateDataAccess;
		private string _tableName;
		public UserAccountDataAccess(string connectionString, string tableName)
		{
			_insertDataAccess = new InsertDataAccess(connectionString);
			_selectDataAccess = new SelectDataAccess(connectionString);
			_deleteDataAccess = new DeleteDataAccess(connectionString);
			_updateDataAccess = new UpdateDataAccess(connectionString);
			_tableName = tableName;
		}

		public async Task<Result> CreateUserAccount(string email, HashData password)
		{
			Result insertResult = await _insertDataAccess.Insert(
				_tableName,
				new Dictionary<string, object>()
				{
					{ "Email", email },
					{ "PasswordHash", Convert.ToBase64String(password.Hash!) },
					{ "PasswordSalt", password.Salt! },
					{ "LoginAttempts", 0 },
					{ "FailureTime", DBNull.Value },
					{ "Disabled", false },
					{ "Role", "VerifiedUser" }
				}
			).ConfigureAwait(false);

			return insertResult;
		}

		public async Task<Result<int>> GetId(string email)
		{
			Result<int> result = new Result<int>();

			Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
				_tableName,
				new List<string>() { "Id" },
				new List<Comparator>()
				{
					new Comparator("Email", "=", email),
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
				result.ErrorMessage = "Invalid number of UserAccounts selected.";
				return result;
			}

			result.IsSuccessful = true;
			if (payload.Count > 0) result.Payload = (int)payload[0]["Id"];
			return result;
		}

		public async Task<Result<int>> GetId(string email, HashData password)
		{
			Result<int> result = new Result<int>();

			Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
				_tableName,
				new List<string>() { "Id" },
				new List<Comparator>()
				{
					new Comparator("Email", "=", email),
					new Comparator("PasswordHash", "=", password.Hash!),
					new Comparator("PasswordSalt", "=", password.Salt!)
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
				result.ErrorMessage = "Invalid number of UserAccounts selected.";
				return result;
			}

			result.IsSuccessful = true;
			if (payload.Count > 0) result.Payload = (int)payload[0]["Id"];
			return result;
		}

		public async Task<Result<UserAccount>> GetUser(int id)
		{
			Result<UserAccount> result = new Result<UserAccount>();

			Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
				_tableName,
				new List<string>() { "*" },
				new List<Comparator>()
				{
					new Comparator("Id","=",id),
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
				result.ErrorMessage = "Invalid number of UserAccounts selected.";
				return result;
			}

			result.IsSuccessful = true;
			if (payload.Count > 0) result.Payload = new UserAccount()
			{
				Id = id,
				Email = (string)payload[0]["Email"],

				LoginAttempts = (int)payload[0]["LoginAttempts"],
				FailureTime = payload[0]["FailureTime"] == DBNull.Value ? null : payload[0]["FailureTime"],
				Role = (string)payload[0]["Role"]
			};
			return result;
		}

		public async Task<Result<UserAccount>> GetUser(string email)
		{
			return await GetUser((await GetId(email).ConfigureAwait(false)).Payload).ConfigureAwait(false);
		}

		public async Task<Result<UserAccount>> GetAttempt(int id)
		{
			Result<UserAccount> result = new Result<UserAccount>();

			Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
				_tableName,
				new List<string>() { "LoginAttempts", "FailureTime" },
				new List<Comparator>()
				{
					new Comparator("Id", "=", id),
				}
			).ConfigureAwait(false);
			if (!selectResult.IsSuccessful || selectResult.Payload is null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = selectResult.ErrorMessage;
				return result;
			}

			List<Dictionary<string, object>> payload = selectResult.Payload;
			if (payload.Count > 2)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid number of UserAccounts selected.";
				return result;
			}

			result.IsSuccessful = true;
			if (payload.Count > 0) result.Payload = new UserAccount()
			{
				LoginAttempts = (int)payload[0]["LoginAttempts"],
				FailureTime = payload[0]["FailureTime"] == DBNull.Value ? null : payload[0]["FailureTime"],
			};
			return result;
		}

		public async Task<Result<UserAccount>> GetHashData(string email)
		{
			Result<UserAccount> result = new Result<UserAccount>();

			Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
				_tableName,
				new List<string>() { "PasswordHash", "PasswordSalt" },
				new List<Comparator>()
				{
					new Comparator("Email", "=", email),
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
				result.ErrorMessage = "Invalid number of UserAccounts selected.";
				return result;
			}

			result.IsSuccessful = true;
			if (payload.Count > 0) result.Payload = new UserAccount()
			{
				PasswordHash = (string)payload.First()["PasswordHash"],
				PasswordSalt = (string)payload.First()["PasswordSalt"],
			};
			return result;
		}

		public async Task<Result<bool>> GetDisabled(int id)
		{
			Result<bool> result = new Result<bool>();

			Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
				_tableName,
				new List<string>() { "Disabled" },
				new List<Comparator>()
				{
					new Comparator("Id", "=", id),
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
				result.ErrorMessage = "Invalid number of UserAccounts selected.";
				return result;
			}

			result.IsSuccessful = true;
			if (payload.Count > 0) result.Payload = (bool)payload[0]["Disabled"];
			return result;
		}

		public async Task<Result> Update(UserAccount userAccount)
		{
			var values = new Dictionary<string, object>();
			foreach (var column in userAccount.GetType().GetProperties())
			{
				var value = column.GetValue(userAccount);
				if (value is null || column.Name == "Id") continue;
				values[column.Name] = value;
			}

			Result updateResult = await _updateDataAccess.Update(
				_tableName,
				new List<Comparator>()
				{
					new Comparator("Id", "=", userAccount.Id),
				},
				values
			).ConfigureAwait(false);

			return updateResult;
		}

		public async Task<Result> Delete(int id)
		{
			Result deleteResult = await _deleteDataAccess.Delete(
				_tableName,
				new List<Comparator>()
				{
					new Comparator("Id", "=", id),
				}
			).ConfigureAwait(false);

			return deleteResult;
		}

		public async Task<Result> Delete(string email)
		{
			Result deleteResult = await _deleteDataAccess.Delete(
				_tableName,
				new List<Comparator>()
				{
					new Comparator("Email", "=", email),
				}
			).ConfigureAwait(false);

			return deleteResult;
		}
        public async Task<Result<int>> CountAdmin()
        {
            Result<int> result = new Result<int>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "COUNT(*) as AdminCount" },
                new List<Comparator>()
                {
                    new Comparator("Role","=", "AdminUser"),
                }
            ).ConfigureAwait(false);
            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            result.IsSuccessful = true;
			result.Payload = (int)payload[0]["AdminCount"];

            return result;
        }
    }
}

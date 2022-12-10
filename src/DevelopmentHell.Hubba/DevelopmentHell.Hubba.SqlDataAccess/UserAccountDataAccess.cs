using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public class UserAccountDataAccess : IUserAccountDataAccess
	{
		private InsertDataAccess _insertDataAccess;
		private SelectDataAccess _selectDataAccess;
		private DeleteDataAccess _deleteDataAccess;
		private UpdateDataAccess _updateDataAccess;
		public UserAccountDataAccess(string connectionString)
		{
			_insertDataAccess = new InsertDataAccess(connectionString);
			_selectDataAccess = new SelectDataAccess(connectionString);
			_deleteDataAccess = new DeleteDataAccess(connectionString);
			_updateDataAccess = new UpdateDataAccess(connectionString);
		}

		public async Task<Result> CreateUserAccount(string email, HashData password)
		{
			Result insertResult = await _insertDataAccess.Insert(
				"UserAccounts",
				new Dictionary<string, object>()
				{
					{ "Email", email },
					{ "PasswordHash", password.Hash },
					{ "PasswordSalt", password.Salt },
					{ "LoginAttempts", 0 },
					{ "FailureTime", DBNull.Value },
					{ "Disabled", false },
				}
			).ConfigureAwait(false);

			return insertResult;
		}

		public async Task<Result<int>> GetId(string email)
		{
			Result<int> result = new Result<int>();

			Result<List<object>> selectResult = await _selectDataAccess.Select(
				"UserAccounts",
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

			List<object> payload = selectResult.Payload;
			if (payload.Count > 1)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid number of UserAccounts selected.";
				return result;
			}

			result.IsSuccessful = true;
			if (payload.Count > 0) result.Payload = (int)payload[0];
			return result;
		}

		public async Task<Result<int>> GetId(string email, HashData password)
		{
			Result<int> result = new Result<int>();

			Result<List<object>> selectResult = await _selectDataAccess.Select(
				"UserAccounts",
				new List<string>() { "Id" },
				new List<Comparator>()
				{
					new Comparator("Email", "=", email),
					new Comparator("PasswordHash", "=", password.Hash),
					new Comparator("PasswordSalt", "=", password.Salt)
				}
			).ConfigureAwait(false);

			if (!selectResult.IsSuccessful || selectResult.Payload is null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = selectResult.ErrorMessage;
				return result;
			}

			List<object> payload = selectResult.Payload;
			if (payload.Count > 1)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid number of UserAccounts selected.";
				return result;
			}

			result.IsSuccessful = true;
			if (payload.Count > 0) result.Payload = (int)payload[0];
			return result;
		}

		public async Task<Result<UserAccount>> GetAttempt(int id)
		{
			Result<UserAccount> result = new Result<UserAccount>();

			Result<List<object>> selectResult = await _selectDataAccess.Select(
				"UserAccounts",
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

			List<object> payload = selectResult.Payload;
			if (payload.Count > 2)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid number of UserAccounts selected.";
				return result;
			}

			result.IsSuccessful = true;
			if (payload.Count > 0) result.Payload = new UserAccount() {
				LoginAttempts = (int)payload[0],
				FailureTime = payload[1] == DBNull.Value ? null : payload[1],
			};
			return result;
		}

		public async Task<Result<bool>> GetDisabled(int id)
		{
			Result<bool> result = new Result<bool>();

			Result<List<object>> selectResult = await _selectDataAccess.Select(
				"UserAccounts",
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

			List<object> payload = selectResult.Payload;
			if (payload.Count > 1)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid number of UserAccounts selected.";
				return result;
			}

			result.IsSuccessful = true;
			if (payload.Count > 0) result.Payload = (bool)payload[0];
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
				"UserAccounts",
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
				"UserAccounts",
				new List<Comparator>()
				{
					new Comparator("Id", "=", id),
				}
			).ConfigureAwait(false);

			return deleteResult;
		}
	}
}

using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public class UserAccountDataAccess : IUserAccountDataAccess
	{
		private InsertDataAccess _insertDataAccess;
		private SelectDataAccess _selectDataAccess;
		private DeleteDataAccess _deleteDataAccess;
		public UserAccountDataAccess(string connectionString)
		{
			_insertDataAccess = new InsertDataAccess(connectionString);
			_selectDataAccess = new SelectDataAccess(connectionString);
			_deleteDataAccess = new DeleteDataAccess(connectionString);
		}

		public async Task<Result<int>> GetUserAccountIdByCredentials(string email, HashData password)
		{
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

			Result<int> result = new Result<int>();
			if (!selectResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = selectResult.ErrorMessage;
				return result;
			}

			List<object> payload = selectResult.Payload;
			if (payload.Count <= 0)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "No UserAccounts selected with email and password.";
				return result;
			}

			if (payload.Count > 1)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Mutliple UserAccounts selected with email and password.";
				return result;
			}

			result.IsSuccessful = true;
			result.Payload = Convert.ToInt32(payload[0]);

			return result;
		}

		public async Task<Result<int>> GetUserAccountIdByEmail(string email)
		{
			Result<List<object>> selectResult = await _selectDataAccess.Select(
				"UserAccounts",
				new List<string>() { "Id" },
				new List<Comparator>()
				{
					new Comparator("Email", "=", email),
				}
			).ConfigureAwait(false);

			Result<int> result = new Result<int>();
			if (!selectResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = selectResult.ErrorMessage;
				return result;
			}

			List<object> payload = selectResult.Payload;
			if (payload.Count <= 0)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "No UserAccounts selected with email.";
				return result;
			}

			if (payload.Count > 1)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Multiple UserAccounts selected with email.";
				return result;
			}

			result.IsSuccessful = true;
			result.Payload = Convert.ToInt32(payload[0]);

			return result;
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
				}
			).ConfigureAwait(false);

			return insertResult;
		}

		public async Task<Result> DeleteUserAccount(int id)
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

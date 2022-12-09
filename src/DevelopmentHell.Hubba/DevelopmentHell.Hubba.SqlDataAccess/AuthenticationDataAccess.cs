using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public class AuthenticationDataAccess : IAuthenticationDataAccess
	{
		private SelectDataAccess _selectDataAccess;
		private readonly string _tableName = "UserAccounts";

		public AuthenticationDataAccess (string connectionString)
		{
			_selectDataAccess = new SelectDataAccess(connectionString);
		}

 		public async Task<Result<UserAccount>> SelectUserAccount(string email, string passwordHash, string passwordSalt)
		{
			var response = await _selectDataAccess.Select(
				_tableName,
				new List<string>() { "Id" },
				new List<Comparator>()
				{
					new Comparator("Email", "=", email),
					new Comparator("PasswordHash", "=", passwordHash),
					new Comparator("PasswordSalt", "=", passwordSalt)
				}
			).ConfigureAwait(false);

			Result<UserAccount> result = new Result<UserAccount>();
			if (!response.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = response.ErrorMessage;
				return result;
			}

			var payload = response.Payload as List<object>;
			if (payload is null || payload.Count <= 0)
			{
				result.ErrorMessage = "No UserAccount selected.";
				result.IsSuccessful = false;
				return result;
			}

			result.Payload = new UserAccount()
			{
				Id = Convert.ToInt32(payload[0]),
			};

			return result;
		}
	}
}

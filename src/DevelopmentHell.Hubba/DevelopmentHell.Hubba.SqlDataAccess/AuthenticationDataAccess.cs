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

 		public async Task<Result> SelectUserAccount(string email, string passwordHash, string passwordSalt)
		{
			var selectResult = await _selectDataAccess.Select(
				_tableName,
				new List<string>() { "Id" },
				new List<Comparator>()
				{
					new Comparator("Email", "=", email),
					new Comparator("PasswordHash", "=", passwordHash),
					new Comparator("PasswordSalt", "=", passwordSalt)
				}
			).ConfigureAwait(false);

			return selectResult;
		}
	}
}

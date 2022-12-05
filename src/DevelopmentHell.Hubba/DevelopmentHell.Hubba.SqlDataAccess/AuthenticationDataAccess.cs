using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using System.Configuration;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public class AuthenticationDataAccess
	{
		private SelectDataAccess _selectDataAccess;
		private UpdateDataAccess _updateDataAccess;

		public AuthenticationDataAccess(string connectionString)
		{
			_selectDataAccess = new SelectDataAccess(connectionString);
			_updateDataAccess = new UpdateDataAccess(connectionString);
		}

		public async Task<Result> SelectUserAccount(string email, string password)
		{
			Result selectResult = await _selectDataAccess.Select("UserAccount", new List<string> { "Id" }, new()
			{
				new("Email", "=", email),
				new("Password", "=", password),
			}).ConfigureAwait(false);
			return selectResult;
		}

		public async Task<Result> CreateUserOTP(string email, string passphrase)
		{
			DateTime expirationDateTime = DateTime.UtcNow.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["OTPExpirationOffsetSeconds"]!));
			Result updateResult = await _updateDataAccess.Update("UserOTP", new Tuple<string, object>("Email", email), new Dictionary<string, object>
			{
				{ "ExpirationDateTime", expirationDateTime },
				{ "Passphrase", passphrase },
			}).ConfigureAwait(false);
			return updateResult;
		}

		public async Task<Result> SelectUserOTP(string otp, string email)
		{
			DateTime now = DateTime.UtcNow;
			Result selectResult = await _selectDataAccess.Select(
				TableManip.InnerJoinTables("UserOTP", "UserAccount", "Email", "Email"),
				new() { "*" },
				new()
				{
					new("passphrase", "=", otp),
					new("UserOTP.email", "=", email),
					new(now, "<", "UserOTP.ExpirationDateTime")
				});
			return selectResult;
		}
	}
}

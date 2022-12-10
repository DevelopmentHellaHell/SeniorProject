using DevelopmentHell.Hubba.Authentication.Implementation;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword;

namespace DevelopmentHell.Hubba.AuthenticationManager
{
	public class AuthenticationManager
	{
		private AuthenticationService _authenticationService;
		private readonly string _connectionString = "Server=.;Database=DevelopmentHell.Hubba.Users;Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.User;Password=password";

		public AuthenticationManager()
		{
			_authenticationService = new AuthenticationService(_connectionString);
		}

		public async Task<Result> Login(string email, string password)
		{
			var result = await _authenticationService.AuthenticateCredentials(email, password).ConfigureAwait(false);
			if (!result.IsSuccessful)
			{ 
				return result;
			}

			var accountRow = (List<object>)result.Payload!;
			if (accountRow!.Count <= 0)
			{
				return new Result(false, "No account rows returned");
			}
			var accountId = Convert.ToInt32(accountRow[0].ToString());
			var otpManager = new OTPService(_connectionString);
			var otp = otpManager.NewOTP(accountId).Result.Payload!.ToString()!;

			var sendOTPResult = otpManager.SendOTP(email, otp);
			if (!sendOTPResult)
			{
				return new Result(false, "Could not email OTP.");
			}

			return new Result(true);
		}
	}
}
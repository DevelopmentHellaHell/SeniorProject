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

			UserAccount account = result.Payload;
			var otpManager = new OTPService(_connectionString);
			var otp = otpManager.NewOTP(account.Id).Result.Payload!.ToString()!;

			var sendOTPResult = otpManager.SendOTP(email, otp);
			if (!sendOTPResult)
			{
				return new Result()
				{
					IsSuccessful = false,
					ErrorMessage = "Could not send otp.",
				};
			}

			return new Result()
			{
				IsSuccessful = true,
			};
		}
	}
}
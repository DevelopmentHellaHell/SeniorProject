using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Implementation;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service;

namespace DevelopmentHell.Hubba.Authentication.Manager
{
	public class AuthenticationManager
	{
		private IAuthenticatonService _authenticationService;
		private OTPService _otpService;
		private readonly string _connectionString = "Server=.;Database=DevelopmentHell.Hubba.Users;Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.User;Password=password";

		public AuthenticationManager()
		{
			_authenticationService = new AuthenticationService(_connectionString);
			_otpService = new OTPService(_connectionString);
		}

		public async Task<Result<int>> Login(string email, string password)
		{
			Result<int> result = new Result<int>();

			Result<int> authenticateResult = await _authenticationService.AuthenticateCredentials(email, password).ConfigureAwait(false);
			if (!authenticateResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = authenticateResult.ErrorMessage;
				return result;
			}

			int accountId = authenticateResult.Payload;
			Result<string> otpResult = await _otpService.NewOTP(accountId).ConfigureAwait(false);
			string otp = otpResult.Payload.ToString();

			Result sendOTPResult = _otpService.SendOTP(email, otp);
			if (!sendOTPResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = sendOTPResult.ErrorMessage;
				return result;
			}

			result.IsSuccessful = true;
			result.Payload = accountId;
			return result;
		}

		public async Task<Result> AuthenticateOTP(int accountId, string otp)
		{

			return await _otpService.CheckOTP(accountId, otp).ConfigureAwait(false);
		}
	}
}
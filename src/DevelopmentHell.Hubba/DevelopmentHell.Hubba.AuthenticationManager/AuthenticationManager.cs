using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Implementation;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service;

namespace DevelopmentHell.Hubba.Authentication.Manager
{
	public class AuthenticationManager
	{
		private IAuthenticatonService _authenticationService;
		private OTPService _otpService;
		private ILoggerService _loggerService;
		public AuthenticationManager(string connectionString, string authenticationTableName, string otpTableName, ILoggerService loggerService)
		{
			_authenticationService = new AuthenticationService(connectionString,authenticationTableName, loggerService);
			_otpService = new OTPService(connectionString,otpTableName);
			_loggerService = loggerService;
		}

		public async Task<Result<int>> Login(string email, string password, string ipAddress)
		{
			Result<int> result = new Result<int>();

			Result<int> authenticateResult = await _authenticationService.AuthenticateCredentials(email, password, ipAddress).ConfigureAwait(false);
			if (!authenticateResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = authenticateResult.ErrorMessage;
				return result;
			}

			int accountId = authenticateResult.Payload;
			Result<string> otpResult = await _otpService.NewOTP(accountId).ConfigureAwait(false);
			string otp = otpResult.Payload!.ToString();

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
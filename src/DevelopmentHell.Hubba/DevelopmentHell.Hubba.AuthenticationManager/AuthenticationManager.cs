using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;

namespace DevelopmentHell.Hubba.Authentication.Manager
{
    public class AuthenticationManager
	{
		private IAuthenticationService _authenticationService;
		private IOTPService _otpService;
		private ILoggerService _loggerService;
		public AuthenticationManager(IAuthenticationService authenticationService, IOTPService otpService, ILoggerService loggerService)
		{
			_authenticationService = authenticationService;
			_otpService = otpService;
			_loggerService = loggerService;
		}

		public async Task<Result<bool>> Login(string email, string password, string ipAddress)
		{
			Result<bool> result = new();

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
			return result;
		}

		public async Task<Result<AuthTicket>> AuthenticateOTP(int accountId, string otp)
		{
			Result<AuthTicket> result = new Result<AuthTicket>()
			{
				IsSuccessful = false,
			};

			Result resultCheck = await _otpService.CheckOTP(accountId, otp).ConfigureAwait(false);
			if (!resultCheck.IsSuccessful)
			{
				result.ErrorMessage = "Invalid OTP.";
				return result;
			}
			return await _authenticationService.CreateSession(accountId).ConfigureAwait(false);
		}

		public async Task<Result<bool>> ValidateSession(AuthTicket ticket)
		{
			return await _authenticationService.ValidateSession(ticket).ConfigureAwait(false);
		}
	}
}
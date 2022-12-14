using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using System.Security.Principal;

namespace DevelopmentHell.Hubba.Authentication.Manager
{
	public class AuthenticationManager
	{
		private IAuthenticationService _authenticationService;
		private IOTPService _otpService;
		private IAuthorizationService _authorizationService;
		private ILoggerService _loggerService;

		public AuthenticationManager(IAuthenticationService authenticationService, IOTPService otpService, IAuthorizationService authorizationService, ILoggerService loggerService)
		{
			_authenticationService = authenticationService;
			_otpService = otpService;
			_authorizationService = authorizationService;
			_loggerService = loggerService;
		}

		public async Task<Result<bool>> Login(string email, string password, string ipAddress, IPrincipal? principal = null, bool enabledSend = true)
		{
			Result<bool> result = new();

			if (_authorizationService.authorize(principal, new string[] { "VerifiedUser", "Admin" }).IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Error, user already logged in.";
				return result;
			}

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

			Result sendOTPResult = _otpService.SendOTP(email, otp, enabledSend);
			if (!sendOTPResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = sendOTPResult.ErrorMessage;
				return result;
			}

			result.IsSuccessful = true;
			return result;
		}

		public async Task<Result<GenericPrincipal>> AuthenticateOTP(int accountId, string otp, string ipAddress, IPrincipal? principal = null)
		{

			Result<GenericPrincipal> result = new Result<GenericPrincipal>()
			{
				IsSuccessful = false,
			};

			if (_authorizationService.authorize(principal, new string[] { "VerifiedUser", "Admin" }).IsSuccessful)
			{
				_loggerService.Log(LogLevel.INFO, Category.BUSINESS, "AuthenticationManager.AuthenticateOTP", $"{ipAddress} failed OTP authentication.");

				result.IsSuccessful = false;
				result.ErrorMessage = "Error, user already logged in.";
				return result;
			}

			Result resultCheck = await _otpService.CheckOTP(accountId, otp).ConfigureAwait(false);
			if (!resultCheck.IsSuccessful)
			{
				result.ErrorMessage = "Invalid or expired OTP, please try again.";
				return result;
			}

			return _authenticationService.CreateSession(accountId);
		}
	}
}
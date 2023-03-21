using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Email.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.OneTimePassword.Service.Implementations

{
	public class OTPService : IOTPService
	{
		private static readonly string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
		private IOTPDataAccess _otpDataAccess;
		private IEmailService _emailService;
		private ICryptographyService _cryptographyService;

		public OTPService(IOTPDataAccess otpDataAccess, IEmailService emailService, ICryptographyService cryptographyService)
		{
			_otpDataAccess = otpDataAccess;
			_emailService = emailService;
			_cryptographyService = cryptographyService;
		}

		public async Task<Result<string>> NewOTP(int accountId)
		{
			Random random = new((int)(DateTime.Now.Ticks << 4 >> 4));
			string otp = new(Enumerable.Repeat(validChars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
			byte[] eotp = _cryptographyService.Encrypt(otp);
			DateTime expiration = DateTime.Now.AddMinutes(2); // TODO: move to config

			Result result = await _otpDataAccess.NewOTP(accountId, eotp, expiration).ConfigureAwait(false);
			return new Result<string>()
			{
				IsSuccessful = result.IsSuccessful,
				ErrorMessage = result.ErrorMessage,
				Payload = otp,
			};
		}

		public async Task<Result> CheckOTP(int accountId, string otp)
		{
			Result result = new Result();

			Result<string> otpResult = await GetOTP(accountId).ConfigureAwait(false);
			if (!otpResult.IsSuccessful || otpResult.Payload is null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = otpResult.ErrorMessage;
				return result;
			}

			if (otp != otpResult.Payload)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid OTP.";
				return result;
			}

			result.IsSuccessful = true;
			return result;
		}

		public Result SendOTP(string email, string otp)
		{
			Result result = new Result()
			{
				IsSuccessful = false,
			};

			Result sendEmail = _emailService.SendEmail(email, "Hubba Authentication", $"Your one time password is: {otp}");
			if (!sendEmail.IsSuccessful)
			{
				result.ErrorMessage = "Serverside issue sending the OTP, please try again later.";
				return result;
			}

			result.IsSuccessful = true;
			return result;
		}

		public async Task<Result<string>> GetOTP(int accountId)
		{
			Result<string> result = new Result<string>();

			Result<byte[]> getResult = await _otpDataAccess.GetOTP(accountId);
			if (!getResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Error, please contact system administrator.";
				return result;
			}

			if (getResult.Payload is null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid OTP.";
				return result;
			}

			byte[] eotpDb = getResult.Payload;
			string otpDb = _cryptographyService.Decrypt(eotpDb);

			result.IsSuccessful = true;
			result.Payload = otpDb;
			return result;
		}
	}
}
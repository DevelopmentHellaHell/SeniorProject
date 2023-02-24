using DevelopmentHell.Hubba.Cryptography.Service;
using DevelopmentHell.Hubba.Emailing.Service;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.OneTimePassword.Service.Implementation

{
	public class OTPService : IOTPService
	{
		private static readonly string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
		private IOTPDataAccess _dao;
		public OTPService(IOTPDataAccess dao)
		{
			_dao = dao;
		}

		public async Task<Result<string>> NewOTP(int accountId)
		{
			Random random = new((int)(DateTime.Now.Ticks << 4 >> 4));
			string otp = new(Enumerable.Repeat(validChars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
			byte[] eotp = EncryptionService.Encrypt(otp);
			DateTime expiration = DateTime.Now.AddMinutes(2); // TODO: move to config

			Result result = await _dao.NewOTP(accountId, eotp, expiration).ConfigureAwait(false);
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

			Result<byte[]> getResult = await _dao.GetOTP(accountId);
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
			string otpDb = EncryptionService.Decrypt(eotpDb);

			if (otp != otpDb)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid OTP.";
				return result;
			}

			result.IsSuccessful = true;
			return result;
		}

		public Result SendOTP(string email, string otp, bool enabledSend)
		{
			Result result = new Result()
			{
				IsSuccessful = false,
			};

			Result sendEmail = EmailService.SendEmail(email, "Hubba Authentication", $"Your one time password is: {otp}", enabledSend);
			if (!sendEmail.IsSuccessful)
			{
				result.ErrorMessage = "Serverside issue sending the OTP, please try again later.";
				return result;
			}

			result.IsSuccessful = true;
			return result;
		}
	}
}
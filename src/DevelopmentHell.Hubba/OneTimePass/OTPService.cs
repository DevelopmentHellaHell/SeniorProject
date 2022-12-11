using DevelopmentHell.Hubba.Cryptography.Service;
using DevelopmentHell.Hubba.Emailing.Service;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;

namespace DevelopmentHell.Hubba.OneTimePassword.Service

{
	public class OTPService
	{
		private static readonly string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
		private OTPDataAccess _dataAccess;
		public OTPService(string connectionString, string tableName)
		{
			_dataAccess = new OTPDataAccess(connectionString, tableName);
		}

		public async Task<Result<string>> NewOTP(int accountId)
		{
			Random random = new((int)(DateTime.UtcNow.Ticks << 4 >> 4));
			string otp = new(Enumerable.Repeat(validChars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
			byte[] eotp = EncryptionService.Encrypt(otp);
			DateTime expiration = DateTime.UtcNow.AddMinutes(2); // TODO: move to config

			Result result = await _dataAccess.NewOTP(accountId, eotp, expiration).ConfigureAwait(false);
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

			Result<byte[]> getResult = await _dataAccess.GetOTP(accountId);
			if (!getResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Error, please contact system administrator.";
				return result;
			}

			if (getResult.Payload is null) {
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

		public Result SendOTP(string email, string otp)
		{
			return EmailService.SendEmail(email, "Hubba Authentication", $"Your one time password is: {otp}.");
		}
	}
}
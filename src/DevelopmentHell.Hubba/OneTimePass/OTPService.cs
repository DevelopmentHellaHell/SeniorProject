using System.Security.Cryptography;
using System.Text;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Emailing.Service;
using DevelopmentHell.Hubba.Cryptography.Service;

namespace DevelopmentHell.Hubba.OneTimePassword.Service

{
	public class OTPService
	{
		private static readonly string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
		private OTPDataAccess _dataAccess;
		public OTPService(string connectionString)
		{
			_dataAccess = new OTPDataAccess(connectionString);
		}

		// return value: payload of string containing the otp
		public async Task<Result<string>> NewOTP(int accountId)
		{
			byte[] aesKey = Encoding.ASCII.GetBytes("gVkYp2s5v8y/B?E(H+MbQeThWmZq4t6w"); // TODO: move to config
			Random random = new((int)(DateTime.UtcNow.Ticks << 4 >> 4));
			string otp = new(Enumerable.Repeat(validChars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
			byte[] eotp = EncryptionService.Encrypt(otp, aesKey);

			Result result = await _dataAccess.NewOTP(accountId, eotp).ConfigureAwait(false);
			return new Result<string>()
			{
				IsSuccessful = result.IsSuccessful,
				ErrorMessage = result.ErrorMessage,
				Payload = otp,
			};
		}

		public async Task<Result> CheckOTP(int accountId, string otp)
		{
			string userFriendlyErrorMessage = "The email or password provided is invalid.";
			Result result = new Result();
			byte[] aesKey = Encoding.ASCII.GetBytes("gVkYp2s5v8y/B?E(H+MbQeThWmZq4t6w"); // TODO: move to config

			Result<byte[]> getResult = await _dataAccess.GetOTP(accountId);
			if (!getResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = userFriendlyErrorMessage;
				return result;
			}
			byte[] eotpDb = getResult.Payload;
			string otpDb = EncryptionService.Decrypt(eotpDb, aesKey);

			// TEMP
			Console.WriteLine($"INPUT OTP: {otp}, DECRYPTED OTP FROM DB: {otpDb}");

			if (otp != otpDb)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = userFriendlyErrorMessage;
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
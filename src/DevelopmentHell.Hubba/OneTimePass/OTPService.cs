using System.Security.Cryptography;
using System.Text;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Mailing;

namespace DevelopmentHell.Hubba.OneTimePassword

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
			Random random = new( (int)((DateTime.UtcNow.Ticks << 4) >> 4 ) );
            string otp = new(Enumerable.Repeat(validChars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
            byte[] eotp;
			using (AesManaged aes = new AesManaged())
            {
				eotp = Cryptography.Encryption.Encrypt(aes, otp, aesKey, aes.IV);
            }
            
            var result = await _dataAccess.NewOTP(accountId, eotp).ConfigureAwait(false);
            return new Result<string>()
            {
                IsSuccessful = result.IsSuccessful,
                ErrorMessage = result.ErrorMessage,
                Payload = otp,
            };
        }

        //public async Task<Result> CheckOTP(int accountId, string otp)
        //{
        //    Cryptography.Encryption.Encrypt()
        //    _dataAccess.Check(accountId, )
        //}

        public bool SendOTP(string email, string otp)
        {
            return EmailService.SendEmail(email, "Hubba Authentication", $"Your one time password is: {otp}.");
        }
    }
}
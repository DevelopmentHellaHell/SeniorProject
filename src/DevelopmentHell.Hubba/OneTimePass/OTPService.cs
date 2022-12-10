using System.Security.Cryptography;
using System.Text;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Mailing;

namespace DevelopmentHell.Hubba.OneTimePassword

{
    public class OTPService
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private OTPDataAccess _dataAccess;
        //TODO: move to config file
        public OTPService(string connectionString)
        {
            _dataAccess = new OTPDataAccess(connectionString);
        }

        // return value: payload of string containing the otp
        public async Task<Result> NewOTP(int accountId)
        {
			Random random = new( (int)((DateTime.UtcNow.Ticks << 4) >> 4 ) );
            string otp = new(Enumerable.Repeat(validChars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
            byte[] eotp;

			eotp = Cryptography.Encryption.Encrypt(otp);

            var result = await _dataAccess.NewOTP(accountId, eotp).ConfigureAwait(false);
            Console.WriteLine(result.ErrorMessage);
			return new Result(result.IsSuccessful, result.ErrorMessage, otp);
        }

        public async Task<Result> CheckOTP(int accountId, string otp)
        {
            var checkResult = (await _dataAccess.Check(accountId)).Payload;
            if (checkResult is null)
            {
                return new Result(false, "Could not find an OTP associated with given account ID");
            }
            var uotp = Cryptography.Encryption.Decrypt(((List<byte[]>)(checkResult))[0]);
            if (otp != uotp)
            {
                return new Result(false, "OTP associated with given account ID does not match given otp");
            }
            return new Result(true);
        }

        public bool SendOTP(string email, string otp)
        {
            return EmailService.SendEmail(email, "Hubba Authentication", $"Your one time password is: {otp}.");
        }
    }
}
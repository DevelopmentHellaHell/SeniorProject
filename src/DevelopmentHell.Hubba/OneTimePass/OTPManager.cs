using System.Security.Cryptography;
using System.Text;
using DevelopmentHell.Hubba.SqlDataAccess;
using System.Configuration;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.OneTimePassword

{
    public class OTPManager
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private OTPDataAccess _dataAccess;
        public OTPManager(string connectionString)
        {
            _dataAccess = new OTPDataAccess(connectionString);
        }

        // return value: payload of string containing the otp
        public async Task<Result> NewOTP(int accountId)
        {
            byte[] aesKey = Encoding.ASCII.GetBytes("gVkYp2s5v8y/B?E(H+MbQeThWmZq4t6w");
			Random random = new( (int)((DateTime.UtcNow.Ticks << 4) >> 4 ) );
            string otp = new(Enumerable.Repeat(validChars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
            byte[] eotp;
            using (AesManaged aes = new AesManaged())
            {
                eotp = Encryption.Encryption.Encrypt(aes, otp, aesKey, aes.IV);
            }
            //TODO: write to db
            var result = await _dataAccess.NewOTP(accountId, eotp).ConfigureAwait(false);
            Console.WriteLine(result.ErrorMessage);
			return new Result(result.IsSuccessful, result.ErrorMessage, otp);
        }
    }
}
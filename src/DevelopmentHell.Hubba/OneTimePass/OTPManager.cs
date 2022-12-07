using System.Security.Cryptography;
using System.Text;
using DevelopmentHell.Hubba.Encryption;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using System.Configuration;
using System.Collections;
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
            byte[] aesKey = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["AESKey"]!);
            Random random = new( (int)((DateTime.UtcNow.Ticks << 4) >> 4 ) );
            string otp = new(Enumerable.Repeat(validChars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
            string eotp;
            using (AesManaged aes = new AesManaged())
            {
                eotp = Encoding.UTF8.GetString( Encryption.Encryption.Encrypt(aes, otp, aesKey, aes.IV));
            }
            //TODO: write to db
            return await _dataAccess.NewOTP(accountId, eotp).ConfigureAwait(false);
        }
    }
}
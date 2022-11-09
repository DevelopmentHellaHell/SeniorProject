using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace DevelopmentHell.Hubba.Models
{
    public class IHash
    {
        private string _plainText;
        private byte[] _salt;
        public string HashedText { get; set; }
        public IHash() { }
        public IHash(string text)
        {
            _plainText = text;
        }

        public Result ComputeHash()
        {
            var result = new Result();
            try
            {
                _salt = RandomNumberGenerator.GetBytes(128 / 8);
                HashedText = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                                    password: _plainText!,
                                                    salt: _salt,
                                                    prf: KeyDerivationPrf.HMACSHA256,
                                                    iterationCount: 100000,
                                                    numBytesRequested: 256 / 8));
                HashedText = _salt + HashedText;
                result.IsSuccessful = true;
                result.Payload = HashedText;
            } catch (Exception ex)
            {
                result.ErrorMessage = "Hashing error";
            }
            return result;
        }
    }
}
using System.Security.Cryptography;

namespace DevelopmentHell.Hubba.Models
{
    public class HashComputer
    {
        private? string _plainText;
        private byte[] _salt;
        public string HashedText { get; set; }
        public HashComputer() { }
        public HashComputer(string text)
        {
            _plainText = text;
        }

        public string ComputeHash()
        {
            _salt = RandomNumberGenerator.GetBytes(128 / 8);
            HashedText = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                                password: _plainTest!,
                                                salt: _salt,
                                                prf: KeyDerivationPrf.HMACSHA256,
                                                iterationCount: 100000,
                                                numBytesRequested: 256 / 8));
            return HashedText;
        }
    }
}
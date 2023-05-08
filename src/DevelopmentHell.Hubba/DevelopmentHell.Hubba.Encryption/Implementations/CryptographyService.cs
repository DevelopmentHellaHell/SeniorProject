using System.Security.Cryptography;
using System.Text;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Cryptography.Service.Implementations
{
    public class CryptographyService : ICryptographyService
    {
        private string _cryptographyKey;
        private readonly string _saltValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private static Aes _alg = Aes.Create(); // Must be the same across all services

        public CryptographyService(string cryptographyKey)
        {
            _cryptographyKey = cryptographyKey;
        }

        public byte[] Encrypt(string plainText)
        {

            byte[] encrypted;

            _alg.Key = Encoding.ASCII.GetBytes(_cryptographyKey);
            _alg.Padding = PaddingMode.Zeros;
            ICryptoTransform encryptor = _alg.CreateEncryptor();

            // Create MemoryStream
            using (MemoryStream ms = new MemoryStream())
            {
                // Create crypto stream using the CryptoStream class.
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    // Create StreamWriter and write data to a stream
                    using (StreamWriter sw = new StreamWriter(cs))
                        sw.Write(plainText);
                    encrypted = ms.ToArray();
                }
            }

            // Return encrypted data
            return encrypted;
        }

        public byte[] Encrypt(byte[] bytes)
        {
            byte[] encrypted;

            _alg.Key = Encoding.ASCII.GetBytes(_cryptographyKey);
            _alg.Padding = PaddingMode.Zeros;
            ICryptoTransform encryptor = _alg.CreateEncryptor();

            // Create MemoryStream
            using (MemoryStream ms = new MemoryStream())
            {
                // Create crypto stream using the CryptoStream class.
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(bytes, 0, bytes.Length);
                    cs.FlushFinalBlock();
                    encrypted = ms.ToArray();
                }
            }

            // Return encrypted data
            return encrypted;
        }
        public string Decrypt(byte[] encrypted)
        {
            string output;

            _alg.Key = Encoding.ASCII.GetBytes(_cryptographyKey);
            _alg.Padding = PaddingMode.Zeros;
            ICryptoTransform decryptor = _alg.CreateDecryptor();

            // Create MemoryStream
            using (MemoryStream ms = new MemoryStream(encrypted))
            {
                // Create crypto stream using the CryptoStream class.
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    // Create StreamWriter and write data to a stream
                    using (StreamReader sr = new StreamReader(cs))
                        output = sr.ReadToEnd();
                }
            }

            // Return decrypted data
            return output.Replace("\0", string.Empty);
        }
        public byte[] DecryptToBytes(byte[] encrypted)
        {
            byte[] output;

            _alg.Key = Encoding.ASCII.GetBytes(_cryptographyKey);
            _alg.Padding = PaddingMode.Zeros;
            ICryptoTransform decryptor = _alg.CreateDecryptor();

            // Create MemoryStream
            using (MemoryStream ms = new MemoryStream(encrypted))
            {
                // Create crypto stream using the CryptoStream class.
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    // Create StreamWriter and write data to a stream
                    cs.Write(encrypted, 0, encrypted.Length);
                    cs.FlushFinalBlock();
                    output = ms.ToArray();
                }
            }

            // Return encrypted data
            return output;
        }

        public Result<HashData> HashString(string text, string salt)
        {

            Result<HashData> result = new Result<HashData>();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(text);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1_000, HashAlgorithmName.SHA256);

            byte[] deriveBytes = rfc.GetBytes(64);

            result.IsSuccessful = true;
            result.Payload = new HashData()
            {
                Hash = deriveBytes,
                Salt = Encoding.UTF8.GetString(saltBytes),
            };

            return result;
        }

        public Result<HashData> HashString(string text)
        {

            return HashString(text, _cryptographyKey);
        }

        public string GetSaltValidChars()
        {
            return _saltValidChars;
        }
    }
}
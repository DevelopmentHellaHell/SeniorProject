using System.Security.Cryptography;
using System.Text;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Cryptography.Service.Implementations
{
    public class CryptographyService : ICryptographyService
    {
        private byte[] _cryptographyKey;
        private readonly string _saltValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private static Aes _alg = Aes.Create(); // Must be the same across all services

        public CryptographyService(string cryptographyKey)
        {
            _cryptographyKey = Encoding.ASCII.GetBytes(cryptographyKey);
        }

        public byte[] Encrypt(string plainText)
        {

            byte[] encrypted;

            _alg.Key = _cryptographyKey;
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

            _alg.Key = _cryptographyKey;
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

            _alg.Key = _cryptographyKey;
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

            _alg.Key = _cryptographyKey;
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

        public string GetSaltValidChars()
        {
            return _saltValidChars;
        }

        public string SerializeToJson(Dictionary<string, object> data)
        {
            var json = "{";
			foreach (var item in data)
            {
                json += $"\"{item.Key}\":\"{SerializeToJsonValue(item.Value)}\",";
            }
            json = json.Remove(json.Length - 1);
            json += "}";
            return json;
        }

        private string SerializeToJsonValue(object value)
        {
            if (value is string)
            {
                return (string)value;
			}
            else if (value is DateTime)
            {
                return string.Format("\"{0:yyyy-MM-ddTHH:mm:ssZ}\"", value);
            }
            else
            {
                return value.ToString()!;
            }
        }

        public string EncodeBase64(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

        public string SignToken(string unsignedToken, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            using (var hmac = new HMACSHA256(keyBytes))
            {
                var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(unsignedToken));
                return Convert.ToBase64String(signatureBytes)
					.Replace('+', '-')
				    .Replace('/', '_')
				    .Replace("=", "");
			}
        }
    }
}
using System.Security.Cryptography;
using System.Text;

namespace DevelopmentHell.Hubba.Cryptography.Service
{
	public class EncryptionService
	{
		//TODO: move to config file
		private static readonly byte[] _cryptographyKey = Encoding.ASCII.GetBytes("gVkYp2s5v8y/B?E(H+MbQeThWmZq4t6w");

		private static Aes _alg = Aes.Create();

		public static byte[] Encrypt(string plainText)
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

		public static byte[] Encrypt(byte[] bytes)
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
		public static string Decrypt(byte[] encrypted)
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
			return output.Replace("\0", String.Empty);
		}
		public static byte[] DecryptToBytes(byte[] encrypted)
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
	}
}
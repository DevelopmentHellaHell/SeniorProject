using System.Security.Cryptography;

namespace DevelopmentHell.Hubba.Cryptography.Service
{
	public class EncryptionService
	{
		public static byte[] Encrypt(string plainText, byte[] key)
		{
			byte[] encrypted;
			using (AesManaged aes = new AesManaged())
			{
				using (ICryptoTransform encryptor = aes.CreateEncryptor(key, aes.IV))
				{
					using (MemoryStream ms = new MemoryStream())
					{
						// Create crypto stream using the CryptoStream class. This class is the key to encryption
						// and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream
						// to encrypt
						using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
						{
							// Create StreamWriter and write data to a stream
							using (StreamWriter sw = new StreamWriter(cs))
								sw.Write(plainText);
							encrypted = ms.ToArray();
						}
					}
				}
			}

			return encrypted;
		}

		//public static string Decrypt(byte[] encryptedText, byte[] key)
		//{
		//	string decrypted;
		//	using (AesManaged aes = new AesManaged())
		//	{
		//		using (ICryptoTransform encryptor = aes.CreateDecryptor(key, aes.IV))
		//		{
		//			using (MemoryStream ms = new MemoryStream())
		//			{
		//				// Create crypto stream using the CryptoStream class. This class is the key to encryption
		//				// and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream
		//				// to encrypt
		//				using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
		//				{
		//					// Create StreamWriter and write data to a stream
		//					using (StreamWriter sw = new StreamWriter(cs))
		//						sw.Write(encryptedText);
		//					//decrypted = ms.; // TODO
		//				}
		//			}
		//		}
		//	}

		//	return decrypted;
		//}
	}
}
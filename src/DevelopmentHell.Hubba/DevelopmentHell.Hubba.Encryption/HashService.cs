using DevelopmentHell.Hubba.Models;
using System.Security.Cryptography;
using System.Text;

namespace DevelopmentHell.Hubba.Cryptography.Service
{
	public class HashService
	{
		public static readonly string saltValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
		public static Result<HashData> HashString(string text, string salt)
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
	}
}

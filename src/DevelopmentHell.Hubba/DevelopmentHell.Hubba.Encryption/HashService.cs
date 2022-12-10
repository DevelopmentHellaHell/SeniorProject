using DevelopmentHell.Hubba.Models;
using System.Security.Cryptography;

namespace DevelopmentHell.Hubba.Cryptography.Service
{
	public class HashService
	{
		public static Result<HashData> HashString(string text)
		{
			var result = new Result<HashData>();
			using (var hmac = new HMACSHA512(new byte[] { 1, 2, 3 })) // TODO: TEMP KEY
			{
				var salt = Convert.ToBase64String(hmac.Key);
				var hash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(text)));

				result.IsSuccessful = true;
				result.Payload = new HashData(hash, salt);
			}

			return result;

		}
	}
}

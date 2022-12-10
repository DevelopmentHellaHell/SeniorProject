using DevelopmentHell.Hubba.Models;
using System.Security.Cryptography;
using System.Text;

namespace DevelopmentHell.Hubba.Cryptography.Service
{
	public class HashService
	{
		private static readonly byte[] _cryptographyKey = Encoding.ASCII.GetBytes("gVkYp2s5v8y/B?E(H+MbQeThWmZq4t6w");
		public static Result<HashData> HashString(string text)
		{
			var result = new Result<HashData>();
			using (var hmac = new HMACSHA512(_cryptographyKey)) // TODO: TEMP KEY
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

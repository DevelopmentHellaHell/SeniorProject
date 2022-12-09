using DevelopmentHell.Hubba.Models;
using System.Security.Cryptography;

namespace DevelopmentHell.Hubba.Cryptography
{
	public class HashResult
	{
		public string Hash { get; set; }
		public string Salt { get; set; }
		public HashResult (string hash, string salt)
		{
			Hash = hash;
			Salt = salt;
		}
	}
	public class Hash
	{
		public static Result<HashResult> HashString(string text)
		{
			var result = new Result<HashResult>();
			using (var hmac = new HMACSHA512(new Byte[] { 1, 2, 3 })) // TODO: TEMP KEY
			{
				var salt = Convert.ToBase64String(hmac.Key);
				var hash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(text)));
				
				result.IsSuccessful = true;
				result.Payload = new HashResult(hash, salt);
			}

			return result;
			
		}
	}
}

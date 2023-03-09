using System.Security.Claims;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Development.Hubba.JWTHandler.Service.Implementations
{
    public class JWTHandlerService
    {
		public bool ValidateJwt(string jwt, string secretKey)
		{
			string[] parts = jwt.Split('.');

			string headerJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
			string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));

			var headerData = Deserialize(headerJson);

			string algorithm = (string)headerData["alg"];
			if (algorithm is not "HS256") return false;

			byte[] secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

			string unsignedJwt = string.Join(".", parts.Take(2));
			byte[] signature = ComputeSignature(unsignedJwt, secretKeyBytes);

			bool isValid = parts[2].Equals(Base64UrlEncode(signature));

			return isValid;
		}

		public ClaimsPrincipal GetPrincipal(string jwt)
		{
			string[] parts = jwt.Split('.');

			string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));

			var payloadData = Deserialize(payloadJson);
			var identity = new ClaimsIdentity();
			foreach (var kvp in payloadData)
			{
				identity.AddClaim(new Claim(kvp.Key, kvp.Value.ToString()!));
			}

			return new ClaimsPrincipal(identity);
		}

		public Dictionary<string, object> Deserialize(string json)
		{
			Dictionary<string, object> deserializedObject = new Dictionary<string, object>();
			var clean = json.Replace("{", "")
				.Replace("}", "");
			var rows = clean.Split(',');
			foreach (var kv in rows)
			{
				var items = kv.Trim().Replace("\"", "").Split(':');
				deserializedObject.Add(items[0], items[1]);
			}

			return deserializedObject;
		}

		public byte[] Base64UrlDecode(string input)
		{
			string base64 = input.Replace('-', '+').Replace('_', '/');
			while (base64.Length % 4 != 0)
			{
				base64 += '=';
			}
			return Convert.FromBase64String(base64);
		}

		public string Base64UrlEncode(byte[] input)
		{
			string base64 = Convert.ToBase64String(input).Replace('+', '-').Replace('/', '_').Replace("=", "");
			return base64;
		}

		private static byte[] ComputeSignature(string unsignedJwt, byte[] secretKey)
		{
			using (var hmac = new HMACSHA256(secretKey))
			{
				return hmac.ComputeHash(Encoding.UTF8.GetBytes(unsignedJwt));
			}
		}
	}
}
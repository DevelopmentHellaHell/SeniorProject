using Development.Hubba.JWTHandler.Service.Abstractions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Development.Hubba.JWTHandler.Service.Implementations
{
	public class JWTHandlerService : IJWTHandlerService
	{
		private readonly string _jwtKey;

		public JWTHandlerService(string jwtKey)
		{
			_jwtKey = jwtKey;
		}

		public string GenerateInvalidToken()
		{
			var header = new Dictionary<string, object>()
			{
				{ "alg", "HS256" },
				{ "typ", "JWT" }
			};
			var payload = new Dictionary<string, object>()
			{
				{ "exp", DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds() },
			};

			return GenerateToken(header, payload);
		}

		public string GenerateToken(Dictionary<string, object> header, Dictionary<string, object> payload)
		{
			string headerJson = SerializeToJson(header);
			string headerBase64 = EncodeBase64(headerJson);
			string payloadJson = SerializeToJson(payload);
			string payloadBase64 = EncodeBase64(payloadJson);

			string unsignedToken = string.Format("{0}.{1}", headerBase64, payloadBase64);
			string signature = SignToken(unsignedToken, _jwtKey);
			string jwtToken = string.Format("{0}.{1}", unsignedToken, signature);
			return jwtToken;
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

		public bool ValidateJwt(string jwt)
		{
			string[] parts = jwt.Split('.');

			string headerJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
			string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));

			ClaimsPrincipal principal = GetPrincipal(jwt);
			if (int.Parse(principal.FindFirstValue("exp")!) < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
			{
				return false;
			}

			var headerData = Deserialize(headerJson);

			string algorithm = (string)headerData["alg"];
			if (algorithm is not "HS256") return false;

			byte[] secretKeyBytes = Encoding.UTF8.GetBytes(_jwtKey);

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

		private byte[] ComputeSignature(string unsignedJwt, byte[] secretKey)
		{
			using (var hmac = new HMACSHA256(secretKey))
			{
				return hmac.ComputeHash(Encoding.UTF8.GetBytes(unsignedJwt));
			}
		}
	}
}
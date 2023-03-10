using DevelopmentHell.Hubba.Models;
using System.Security.Claims;

namespace Development.Hubba.JWTHandler.Service.Abstractions
{
	public interface IJWTHandlerService
	{
		string GenerateInvalidToken();
		string GenerateToken(Dictionary<string, object> header, Dictionary<string, object> payload);
		string SerializeToJson(Dictionary<string, object> data);
		string EncodeBase64(string value);
		string SignToken(string unsignedToken, string secretKey);
		bool ValidateJwt(string jwt);
		ClaimsPrincipal GetPrincipal(string jwt);
		Dictionary<string, object> Deserialize(string json);
		byte[] Base64UrlDecode(string input);
	}
}

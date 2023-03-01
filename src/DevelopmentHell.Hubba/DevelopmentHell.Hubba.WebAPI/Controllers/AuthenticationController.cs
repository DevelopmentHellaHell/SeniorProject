using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using DevelopmentHell.Hubba.WebAPI.DTO.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationManager _AuthenticationManager;

        public AuthenticationController(IAuthenticationManager AuthenticationManager)
        {
            _AuthenticationManager = AuthenticationManager;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(UserToLoginDTO userToLoginDTO)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress!.ToString();
            var result = await _AuthenticationManager.Login(userToLoginDTO.Email, userToLoginDTO.Password, ipAddress);
            if (!result.IsSuccessful || result.Payload is null)
            {
                return BadRequest(result.ErrorMessage);
            }

			HttpContext.Response.Cookies.Append("access_token", result.Payload, new CookieOptions { SameSite = SameSiteMode.None, Secure = true });
			return Ok();
        }

        [HttpPost]
        [Route("otp")]
        public async Task<IActionResult> AuthenticateOtp(UserToAuthenticateOtpDTO userToAuthenticateOtpDTO)
        {
            // TODO: replace AccountId from DTO with the Thread.CurrentPrincipal generated upon request
            var ipAddress = HttpContext.Connection.RemoteIpAddress!.ToString();
            var result = await _AuthenticationManager.AuthenticateOTP(userToAuthenticateOtpDTO.AccountId, userToAuthenticateOtpDTO.Otp, ipAddress);
            if (!result.IsSuccessful || result.Payload is null)
            {
                return BadRequest(result.ErrorMessage);
            }

            //https://stackoverflow.com/questions/61427818/store-validate-jwt-token-stored-in-httponly-cookie-in-net-core-api
            HttpContext.Response.Cookies.Append("access_token", result.Payload, new CookieOptions {  SameSite=SameSiteMode.None, Secure=true });//, new CookieOptions { HttpOnly = true });
			return Ok();
        }
    }
}

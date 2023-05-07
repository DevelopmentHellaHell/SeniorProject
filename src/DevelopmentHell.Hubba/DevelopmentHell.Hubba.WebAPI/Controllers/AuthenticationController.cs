using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using DevelopmentHell.Hubba.WebAPI.DTO.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;

        public AuthenticationController(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

#if DEBUG
        [HttpGet]
        [Route("health")]
        public Task<IActionResult> HeathCheck()
        {
            return Task.FromResult<IActionResult>(Ok("Healthy"));
        }
#endif

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(UserToLoginDTO userToLoginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress!.ToString();
            var result = await _authenticationManager.Login(userToLoginDTO.Email, userToLoginDTO.Password, ipAddress);
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
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }
            var ipAddress = HttpContext.Connection.RemoteIpAddress!.ToString();

            var result = await _authenticationManager.AuthenticateOTP(userToAuthenticateOtpDTO.Otp, ipAddress);
            if (!result.IsSuccessful || result.Payload is null)
            {
                return BadRequest(result.ErrorMessage);
            }

            // https://stackoverflow.com/questions/61427818/store-validate-jwt-token-stored-in-httponly-cookie-in-net-core-api
            // Enabling HttpOnly does not let client side scripts to see the cookie
            HttpContext.Response.Cookies.Append("access_token", result.Payload.Item1, new CookieOptions { SameSite = SameSiteMode.None, Secure = true });//, new CookieOptions { HttpOnly = true });
            HttpContext.Response.Cookies.Append("id_token", result.Payload.Item2, new CookieOptions { SameSite = SameSiteMode.None, Secure = true });
            return Ok();
        }

        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            var result = _authenticationManager.Logout();
            if (!result.IsSuccessful || result.Payload is null)
            {
                return BadRequest(result.ErrorMessage);
            }

            HttpContext.Response.Cookies.Append("access_token", result.Payload, new CookieOptions { SameSite = SameSiteMode.None, Secure = true });
            HttpContext.Response.Cookies.Append("id_token", result.Payload, new CookieOptions { SameSite = SameSiteMode.None, Secure = true });
            return Ok();
        }
    }
}

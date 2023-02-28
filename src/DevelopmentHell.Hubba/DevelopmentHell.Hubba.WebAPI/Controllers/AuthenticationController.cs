using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using DevelopmentHell.Hubba.WebAPI.DTO.Authentication;
using Microsoft.AspNetCore.Mvc;

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
            userToLoginDTO.IpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            var result = await _AuthenticationManager.Login(userToLoginDTO.Email, userToLoginDTO.Password, userToLoginDTO.IpAddress);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok();
        }

        [HttpPost]
        [Route("authenticateOtp")]
        public async Task<IActionResult> AuthenticateOtp(UserToAuthenticateOtpDTO userToAuthenticateOtpDTO)
        {
            userToAuthenticateOtpDTO.IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString();
            var result = await _AuthenticationManager.AuthenticateOTP(userToAuthenticateOtpDTO.AccountId, userToAuthenticateOtpDTO.Otp, userToAuthenticateOtpDTO.IpAddress);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.ErrorMessage);
            }

            HttpContext.Response.Cookies.Append("jwt", result.Payload!);

            return Ok();
        }
    }
}

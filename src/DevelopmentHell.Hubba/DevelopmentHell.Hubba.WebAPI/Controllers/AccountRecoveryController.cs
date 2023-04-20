using DevelopmentHell.Hubba.AccountRecovery.Manager.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.WebAPI.DTO.AccountRecovery;
using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountRecoveryController : Controller
    {
        private readonly IAccountRecoveryManager _accountRecoveryManager;

        public AccountRecoveryController(IAccountRecoveryManager accountRecoveryManager)
        {
            _accountRecoveryManager = accountRecoveryManager;
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
        [Route("emailVerification")]
        public async Task<IActionResult> EmailVerification(AccountToRecoverDTO accountToRecoverDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }

            var result = await _accountRecoveryManager.EmailVerification(accountToRecoverDTO.Email);
            if (!result.IsSuccessful || result.Payload is null)
            {
                return BadRequest(result.ErrorMessage);
            }

            HttpContext.Response.Cookies.Append("access_token", result.Payload, new CookieOptions { SameSite = SameSiteMode.None, Secure = true });
            return Ok();
        }

        [HttpPost]
        [Route("recoveryOtp")]
        public async Task<IActionResult> AuthenticateOtp(OtpToVerifyDTO otpToVerifyDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress!.ToString();
            var otpResult = await _accountRecoveryManager.AuthenticateOTP(otpToVerifyDTO.Otp, ipAddress);
            if (!otpResult.IsSuccessful || otpResult.Payload == false)
            {
                return BadRequest(otpResult.ErrorMessage);
            }

            var accessResult = await _accountRecoveryManager.AccountAccess(ipAddress);
            if (!accessResult.IsSuccessful)
            {
                return BadRequest(accessResult.ErrorMessage);
            }

            //manuary recovery
            if (accessResult.Payload is null)
            {
                return Ok();
            }

            //automated recovery
            //https://stackoverflow.com/questions/61427818/store-validate-jwt-token-stored-in-httponly-cookie-in-net-core-api
            HttpContext.Response.Cookies.Append("access_token", accessResult.Payload.Item1, new CookieOptions { SameSite = SameSiteMode.None, Secure = true });//, new CookieOptions { HttpOnly = true });
            HttpContext.Response.Cookies.Append("id_token", accessResult.Payload.Item2, new CookieOptions { SameSite = SameSiteMode.None, Secure = true });
            return Ok();
        }
    }
}

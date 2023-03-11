using DevelopmentHell.Hubba.AccountDeletion.Manager.Abstraction;
using DevelopmentHell.Hubba.WebAPI.DTO.AccountDeletion;
using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountDeletionController : Controller
    {
        private readonly IAccountDeletionManager _accountDeletionManager;

        public AccountDeletionController(IAccountDeletionManager accountDeletionManager)
        {
            _accountDeletionManager = accountDeletionManager;
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
        [Route("deleteaccount")]
        public async Task<IActionResult> DeleteAccount(AccountDeletionDTO accountDeletionDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }

            var result = await _accountDeletionManager.DeleteAccount(accountDeletionDTO.AccountId).ConfigureAwait(false);
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

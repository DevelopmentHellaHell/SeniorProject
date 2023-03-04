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

            var result = await _accountDeletionManager.DeleteAccount(accountDeletionDTO.AccountId);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }
    }
}

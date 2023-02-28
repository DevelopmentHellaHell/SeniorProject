using DevelopmentHell.Hubba.Registration.Manager.Abstractions;
using DevelopmentHell.Hubba.WebAPI.DTO.Registration;
using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    [ApiController]
	[Route("[controller]")]
	public class RegistrationController : Controller
	{
		private readonly IRegistrationManager _registrationManager;

		public RegistrationController(IRegistrationManager registrationManager)
		{
			_registrationManager = registrationManager;
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
		[Route("register")]
		public async Task<IActionResult> Register(UserToRegisterDTO userToRegisterDTO)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var result = await _registrationManager.Register(userToRegisterDTO.Email, userToRegisterDTO.Password).ConfigureAwait(false);
			if (!result.IsSuccessful)
			{
				return BadRequest(result.ErrorMessage);
			}

			return Ok();
		}
	}
}

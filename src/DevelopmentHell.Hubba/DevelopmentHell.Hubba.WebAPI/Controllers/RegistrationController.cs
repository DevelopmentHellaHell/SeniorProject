using DevelopmentHell.Hubba.Registration.Manager.Abstractions;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
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

		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register(UserToRegisterDTO userToRegisterDTO)
		{
			var result = await _registrationManager.Register(userToRegisterDTO.Email, userToRegisterDTO.Password);
			if (!result.IsSuccessful)
			{
				return BadRequest(result.ErrorMessage);
			}

			return Ok();
		}
	}
}

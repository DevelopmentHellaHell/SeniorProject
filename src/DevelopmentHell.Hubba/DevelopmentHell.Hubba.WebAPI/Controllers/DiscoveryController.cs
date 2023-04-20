using DevelopmentHell.Hubba.Discovery.Manager.Abstractions;
using DevelopmentHell.Hubba.WebAPI.DTO.Discovery;
using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DiscoveryController : HubbaController
	{
		private readonly IDiscoveryManager _discoveryManager;

		public DiscoveryController(IDiscoveryManager discoveryManager)
		{
			_discoveryManager = discoveryManager;
		}

		[HttpGet]
		[Route("getCurated")]
		public async Task<IActionResult> GetCurated(CuratedDTO curatedDTO)
		{
			return await GuardedWorkload(async () =>
			{
				if (!ModelState.IsValid)
				{
					return StatusCode(StatusCodes.Status400BadRequest);
				}

				var result = await _discoveryManager.GetCurated(curatedDTO.Offset).ConfigureAwait(false);
				if (!result.IsSuccessful)
				{
					return StatusCode(result.StatusCode, result.ErrorMessage);
				}

				return StatusCode((int)result.StatusCode!, result.Payload);
			}).ConfigureAwait(false);
		}
	}
}

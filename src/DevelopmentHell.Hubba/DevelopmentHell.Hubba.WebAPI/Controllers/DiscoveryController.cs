using DevelopmentHell.Hubba.Discovery.Manager.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
	public class DiscoveryController : ControllerBase
	{
		private readonly IDiscoveryManager _discoveryManager;

		public DiscoveryController(IDiscoveryManager discoveryManager)
		{
			_discoveryManager = discoveryManager;
		}

#if DEBUG
		[HttpGet]
		[Route("health")]
		public Task<IActionResult> HeathCheck()
		{
			return Task.FromResult<IActionResult>(Ok("Healthy"));
		}
#endif


	}
}

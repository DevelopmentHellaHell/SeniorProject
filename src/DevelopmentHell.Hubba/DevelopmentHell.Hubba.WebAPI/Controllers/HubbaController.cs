using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
	public abstract class HubbaController : ControllerBase
	{
#if DEBUG
		[HttpGet]
		[Route("health")]
		public Task<IActionResult> HeathCheck()
		{
			return Task.FromResult<IActionResult>(Ok("Healthy"));
		}
#endif

		public async Task<IActionResult> GuardedWorkload(Func<Task<IActionResult>> workload)
		{
			try
			{
				return await workload();
			}
			catch (Exception ex) 
			{
				// Handle the exception here
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}

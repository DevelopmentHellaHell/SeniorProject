using DevelopmentHell.Hubba.Analytics.Service.Abstractions;
using DevelopmentHell.Hubba.Analytics.Service.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AnalyticsController : ControllerBase
	{
		private readonly IAnalyticsService _analyticsService;

		public AnalyticsController(IAnalyticsService analyticsService)
		{
			//IServiceCollection services = new ServiceCollection();
			// add transient
			// var provider = services.buildserviceprovider();
			// var serviceProvider = provider.getrequiredservice<"service">();
			
			_analyticsService = analyticsService;
		}

		[HttpGet]
		[Route("data")]
		public async Task<IActionResult> Get()
		{
			var data = await _analyticsService.GetData(DateTime.Now.AddMonths(-3));
			if (!data.IsSuccessful)
			{
				return BadRequest();
			}
			return Ok(data.Payload);
		}
	}
}
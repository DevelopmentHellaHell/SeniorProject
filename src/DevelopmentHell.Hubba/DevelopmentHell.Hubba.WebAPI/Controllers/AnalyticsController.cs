using DevelopmentHell.Hubba.Analytics.Service.Abstractions;
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
            _analyticsService = analyticsService;
        }

#if DEBUG
        [HttpGet]
        [Route("health")]
        public Task<IActionResult> HeathCheck()
        {
            return Task.FromResult<IActionResult>(Ok("Healthy"));
        }
#endif

        [HttpGet]
        [Route("data")]
        public async Task<IActionResult> Get()
        {
            var fromTimeMonths = DateTime.Now.AddMonths(-3);
            var result = await _analyticsService.GetData(fromTimeMonths).ConfigureAwait(false);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Payload);
        }
    }
}
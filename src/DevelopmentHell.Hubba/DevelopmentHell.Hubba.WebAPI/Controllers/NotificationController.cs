using DevelopmentHell.Hubba.Notification.Manager.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    //TODO
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : Controller
    {
        private readonly INotificationManager _notificationManager;
        
        public NotificationController(INotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

        [HttpGet]
        [Route("health")]
        public Task<IActionResult> HealthCheck()
        {
            return Task.FromResult<IActionResult>(Ok("Healthy"));
        }

    }
}

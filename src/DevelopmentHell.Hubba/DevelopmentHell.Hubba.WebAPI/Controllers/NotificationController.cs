using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Manager.Abstractions;
using DevelopmentHell.Hubba.WebAPI.DTO.Notification;
using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : Controller
    {
        private readonly INotificationManager _notificationManager;

        public NotificationController(INotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

#if DEBUG
        [HttpGet]
        [Route("health")]
        public Task<IActionResult> HealthCheck()
        {
            return Task.FromResult<IActionResult>(Ok("Healthy"));
        }
#endif

        [HttpGet]
        [Route("getNotificationSettings")]
        public async Task<IActionResult> GetNotificationSettings()
        {
            var result = await _notificationManager.GetNotificationSettings().ConfigureAwait(false);
            if (!result.IsSuccessful || result.Payload is null)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Payload);
        }

        [HttpGet]
        [Route("getNotifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var result = await _notificationManager.GetNotifications().ConfigureAwait(false);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Payload);
        }

        [HttpPost]
        [Route("hideAllNotifications")]
        public async Task<IActionResult> hideAllNotifications()
        {
            var result = await _notificationManager.HideAllNotifications().ConfigureAwait(false);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok();
        }

        [HttpPost]
        [Route("updateNotificationSettings")]
        public async Task<IActionResult> UpdateNotificationSettings(UpdateNotificationSettingsDTO updateNotificationSettingsDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }

            var result = await _notificationManager.UpdateNotificationSettings(new NotificationSettings()
            {
                UserId = 0, // Temp value to be replaced in manager layer
                SiteNotifications = updateNotificationSettingsDTO.SiteNotifications,
                EmailNotifications = updateNotificationSettingsDTO.EmailNotifications,
                TextNotifications = updateNotificationSettingsDTO.TextNotifications,
                TypeScheduling = updateNotificationSettingsDTO.TypeScheduling,
                TypeWorkspace = updateNotificationSettingsDTO.TypeWorkspace,
                TypeProjectShowcase = updateNotificationSettingsDTO.TypeProjectShowcase,
                TypeOther = updateNotificationSettingsDTO?.TypeOther
            }
            ).ConfigureAwait(false);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok();
        }

        [HttpPost]
        [Route("hideIndividualNotifications")]
        public async Task<IActionResult> HideInidividualNotifications(HideNotificationsDTO hideNotificationsDTO)
        {
            //make dto
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            List<int> selectedNotifications = hideNotificationsDTO.hideNotifications;

            var result = await _notificationManager.HideIndividualNotifications(selectedNotifications);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        [HttpGet]
        [Route("getPhoneDetails")]
        public async Task<IActionResult> GetPhoneDetails()
        {
            var result = await _notificationManager.GetPhoneDetails().ConfigureAwait(false);
            if (!result.IsSuccessful || result.Payload is null)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Payload);
        }

        [HttpPost]
        [Route("updatePhoneDetails")]
        public async Task<IActionResult> UpdatePhoneDetails(UpdatePhoneDetailsDTO updatePhoneDetailsDTO)
        {
            var result = await _notificationManager.UpdatePhoneDetails(updatePhoneDetailsDTO.CellPhoneNumber, updatePhoneDetailsDTO.CellPhoneProvider).ConfigureAwait(false);
            if (!result.IsSuccessful)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok();
        }
    }
}

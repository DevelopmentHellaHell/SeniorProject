using Microsoft.AspNetCore.Mvc;
using DevelopmentHell.Hubba.UserManagement.Manager.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.WebAPI.DTO.UserManagement;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementManager _userManagementManager;
        private readonly ILoggerService _logger;
        public UserManagementController(IUserManagementManager userManagementManager, ILoggerService loggerService)
        {
            _userManagementManager = userManagementManager;
            _logger = loggerService;
        }

#if DEBUG
        [HttpGet]
        [Route("health")]
        public Task<IActionResult> HealthCheck()
        {
            return Task.FromResult<IActionResult>(Ok("Healthy"));
        }
#endif
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAccount(UserManagementUserDTO userData)
        {
            try
            {
                var result = await _userManagementManager.ElevatedCreateAccount(userData.Email!, userData.Password!, userData.Role!, userData.FirstName, userData.LastName, userData.UserName).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    _logger.Log(Models.LogLevel.WARNING, Category.VIEW, result.ErrorMessage!);
                    return BadRequest("Problem creating account. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Problem creating account. Please inspect querty and try again later.");
            }
            return Ok();
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> DeleteAccount(EmailDTO in_email)
        {
            var email = in_email.Email!;
            try
            {
                var result = await _userManagementManager.ElevatedDeleteAccount(email).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    _logger.Log(Models.LogLevel.WARNING, Category.VIEW, result.ErrorMessage!);
                    return BadRequest($"Failed to delete account {email}");
                }
            }
            catch (Exception ex)
            {
                _logger.Log(Models.LogLevel.WARNING, Category.VIEW, ex.Message!);
                return BadRequest($"Failed to delete account {email}");
            }
            return Ok();
        }

        [HttpPost]
        [Route("enable")]
        public async Task<IActionResult> EnableAccount(EmailDTO in_email)
        {
            var email = in_email.Email!;
            try
            {
                var result = await _userManagementManager.ElevatedEnableAccount(email);
                if (!result.IsSuccessful)
                {
                    _logger.Log(Models.LogLevel.WARNING, Category.VIEW, result.ErrorMessage!);
                    return BadRequest($"Failed to Enable account {email}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to Enable account {email}");
            }
            return Ok();
        }

        [HttpPost]
        [Route("disable")]
        public async Task<IActionResult> DisableAccount(EmailDTO in_email)
        {
            var email = in_email.Email!;
            try
            {
                var result = await _userManagementManager.ElevatedDisableAccount(email);
                if (!result.IsSuccessful)
                {
                    _logger.Log(Models.LogLevel.WARNING, Category.VIEW, result.ErrorMessage!);
                    return BadRequest($"Failed to Disable account {email}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to Disable account {email}");
            }
            return Ok();
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateAccount(UserManagementUserDTO user)
        {
            Dictionary<string, object> data = new();
            if (user.FirstName != null)
            {
                data["FirstName"] = user.FirstName;
            }
            if(user.LastName != null)
            {
                data["LastName"] = user.LastName;
            }
            if (user.UserName != null)
            {
                data["UserName"] = user.UserName;
            }
            if (user.Email != null)
            {
                data["Email"] = user.Email;
            }

            try
            {
                var result = await _userManagementManager.ElevatedUpdateAccount(user.Email!,data);
                if (!result.IsSuccessful)
                {
                    _logger.Log(Models.LogLevel.WARNING, Category.VIEW, result.ErrorMessage!);
                    return BadRequest($"Failed to Update account {user.Email}");
                }
            }
            catch (Exception ex)
            {
                _logger.Log(Models.LogLevel.WARNING, Category.VIEW, ex.Message!);
                return BadRequest($"Failed to Update account {user.Email}");
            }
            return Ok();
        }
    }
}

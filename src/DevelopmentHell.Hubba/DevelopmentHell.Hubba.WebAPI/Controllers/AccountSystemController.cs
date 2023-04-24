using DevelopmentHell.Hubba.AccountSystem.Manager.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountSystemController : HubbaController
    {
        private readonly IAccountSystemManager _accountSystemManager;
        public AccountSystemController(IAccountSystemManager accountSystemManager)
        {
            _accountSystemManager = accountSystemManager;
        }

        [HttpPost]
        [Route("verifyAccount")]
        public async Task<IActionResult> VerifyAccount()
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _accountSystemManager.VerifyAccount().ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("verifyNewEmail")]
        public async Task<IActionResult> VerifyNewEmail(string newEmail)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _accountSystemManager.VerifyNewEmail(newEmail).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("otpVerification")]
        public async Task<IActionResult> OTPValidation(string otpEntry)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _accountSystemManager.OTPVerification(otpEntry).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("updateEmailInformation")]
        public async Task<IActionResult> updateEmailInformation(string newEmail, string password)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _accountSystemManager.UpdateEmailInformation(newEmail, password).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("updatePassword")]
        public async Task<IActionResult> UpdatePassword(string oldPassword, string newPassword, string newPasswordDupe)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _accountSystemManager.UpdatePassword(oldPassword, newPassword, newPasswordDupe).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("updateUserName")]
        public async Task<IActionResult> UpdateUserName(string firstName, string lastName)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _accountSystemManager.UpdateUserName(firstName, lastName).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpGet]
        [Route("getAccountSettings")]
        public async Task<IActionResult> GetAccountSettings()
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _accountSystemManager.GetAccountSettings().ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode, result.Payload);
            }).ConfigureAwait(false);
        }
    }
}

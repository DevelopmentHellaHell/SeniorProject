using DevelopmentHell.Hubba.AccountSystem.Manager.Abstractions;
using DevelopmentHell.Hubba.WebAPI.DTO.AccountSystem;
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
        public async Task<IActionResult> VerifyNewEmail(VerifyEmailDTO verifyEmailDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                string newEmail = verifyEmailDTO.newEmail;

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
        public async Task<IActionResult> OTPValidation(otpDTO otpEntryDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                string otpEntry = otpEntryDTO.otp!;

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
        public async Task<IActionResult> UpdateEmailInformation(UpdateEmail emailInfo)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
                var newEmail = emailInfo.newEmail;
                var password = emailInfo.password;

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
        public async Task<IActionResult> UpdatePassword(UpdatePasswordDTO updatePasswordDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                string oldPassword = updatePasswordDTO.oldPassword!;
                string newPassword = updatePasswordDTO.newPassword!;
                string newPasswordDupe = updatePasswordDTO.newPasswordDupe!;

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
        public async Task<IActionResult> UpdateUserName(UpdateUserNameDTO updateUserNameDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                string firstName = updateUserNameDTO.firstName!;
                string lastName = updateUserNameDTO.lastName!;

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

        [HttpPost]
        [Route("cancelBooking")]
        public async Task<IActionResult> CancelBooking(CancelBookingDTO cancelBookingInfo)
        {
            return await GuardedWorkload(async() =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _accountSystemManager.CancelBooking(cancelBookingInfo.bookingId).ConfigureAwait(false);
                if(!result.IsSuccessful) 
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpGet]
        [Route("getBookingHistory")]
        public async Task<IActionResult> GetBookingHistory()
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _accountSystemManager.GetBookingHistory().ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode, result.Payload);
            }).ConfigureAwait(false);
        }
    }

}

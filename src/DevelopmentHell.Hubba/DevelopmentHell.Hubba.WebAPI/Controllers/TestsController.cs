using System.Security.Claims;
using DevelopmentHell.Hubba.Models.Tests;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.WebAPI.DTO.Tests;
using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    public struct CreateAdminDTO
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class TestsController : Controller
    {
        private readonly ITestingService _testingService;
        private readonly IOTPService _otpService;
        private readonly IRegistrationService _registrationService;

        public TestsController(ITestingService testingService, IOTPService otpService, IRegistrationService registrationService)
        {
            _testingService = testingService;
            _otpService = otpService;
            _registrationService = registrationService;
        }

#if DEBUG
        [HttpGet]
        [Route("health")]
        public Task<IActionResult> HeathCheck()
        {
            return Task.FromResult<IActionResult>(Ok("Healthy"));
        }

        [HttpPost]
        [Route("createAdmin")]
        public async Task<IActionResult> CreateAdmin(CreateAdminDTO createAdminDTO)
        {

            await _registrationService.RegisterAccount(createAdminDTO.Email!, createAdminDTO.Password!, "AdminUser").ConfigureAwait(false);
            return Ok();
        }

        [HttpPost]
        [Route("createUser")]
        public async Task<IActionResult> CreateUser(CreateAdminDTO createAdminDTO)
        {

            await _registrationService.RegisterAccount(createAdminDTO.Email!, createAdminDTO.Password!, "VerifiedUser").ConfigureAwait(false);
            return Ok();
        }

        [HttpPost]
        [Route("deleteDatabaseRecords")]
        public async Task<IActionResult> DeleteDatabaseRecords(DBRecordsToDeleteDTO dbRecordsToDeleteDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }

            var db = _testingService.GetDatabase(dbRecordsToDeleteDTO.Database);
            if (db is null)
            {
                return BadRequest($"Could not find database {dbRecordsToDeleteDTO.Database}");
            }

            var deleteResult = await _testingService.DeleteDatabaseRecords((Databases)db).ConfigureAwait(false);
            if (!deleteResult.IsSuccessful)
            {
                return BadRequest(deleteResult.ErrorMessage);
            }

            return Ok();
        }

        [HttpPost]
        [Route("deleteTableRecords")]
        public async Task<IActionResult> DeleteTableRecords(TRecordsToDeleteDTO tRecordsToDeleteDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }

            var db = _testingService.GetDatabase(tRecordsToDeleteDTO.Database);
            if (db is null)
            {
                return BadRequest($"Could not find database {tRecordsToDeleteDTO.Database}");
            }

            var t = _testingService.GetTable((Databases)db, tRecordsToDeleteDTO.Table);
            if (t is null)
            {
                return BadRequest($"Could not find table {tRecordsToDeleteDTO.Table} in {tRecordsToDeleteDTO.Database}");
            }

            var deleteResult = await _testingService.DeleteTableRecords((Databases)db, (Tables)t).ConfigureAwait(false);
            if (!deleteResult.IsSuccessful)
            {
                return BadRequest(deleteResult.ErrorMessage);
            }

            return Ok();
        }

        [HttpGet]
        [Route("getOtp")]
        public async Task<IActionResult> GetOTP()
        {
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                return BadRequest("Error, invalid access token format.");
            }

            var accountId = int.Parse(stringAccountId);
            var result = await _otpService.GetOTP(accountId).ConfigureAwait(false);
            if (!result.IsSuccessful || result.Payload is null)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Payload);
        }
#endif
    }
}

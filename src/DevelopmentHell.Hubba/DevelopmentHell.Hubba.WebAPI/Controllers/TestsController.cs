using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.WebAPI.DTO.Tests;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TestsController : Controller
	{
		private readonly TestsDataAccess _testsDataAccess;
		private readonly IOTPService _otpService;

		public TestsController(TestsDataAccess testsDataAccess, IOTPService otpService)
		{
			_testsDataAccess = testsDataAccess;
			_otpService = otpService;
		}

#if DEBUG
		[HttpGet]
		[Route("health")]
		public Task<IActionResult> HeathCheck()
		{
			return Task.FromResult<IActionResult>(Ok("Healthy"));
		}

		[HttpPost]
		[Route("deleteDatabaseRecords")]
		public async Task<IActionResult> DeleteDatabaseRecords(DBRecordsToDeleteDTO dbRecordsToDeleteDTO)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest("Invalid request.");
			}

			var deleteResult = await _testsDataAccess.DeleteDatabaseRecords(dbRecordsToDeleteDTO.Database).ConfigureAwait(false);
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
			var deleteResult = await _testsDataAccess.DeleteTableRecords(tRecordsToDeleteDTO.Database, tRecordsToDeleteDTO.Table).ConfigureAwait(false);
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
			var stringAccountId = claimsPrincipal?.FindFirstValue("accountId");
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

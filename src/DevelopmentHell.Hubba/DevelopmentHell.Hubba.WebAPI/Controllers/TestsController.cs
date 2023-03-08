using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.WebAPI.DTO.Tests;
using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TestsController : Controller
	{
		private readonly TestsDataAccess _testsDataAccess;

		public TestsController(TestsDataAccess testsDataAccess)
		{
			_testsDataAccess = testsDataAccess;
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
#endif

	}
}

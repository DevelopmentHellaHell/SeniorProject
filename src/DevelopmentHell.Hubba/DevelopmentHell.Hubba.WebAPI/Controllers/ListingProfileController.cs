using DevelopmentHell.Hubba.WebAPI.DTO.Discovery;
using DevelopmentHell.ListingProfile.Manager.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListingProfileController : HubbaController
    {
        private readonly IListingProfileManager _listingProfileManager;

        public ListingProfileController(IListingProfileManager listingProfileManager)
        {
            _listingProfileManager = listingProfileManager;
        }

        [HttpGet]
        [Route("createListing")]
        public async Task<IActionResult> CreateListing(string title)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _listingProfileManager.CreateListing(title).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpGet]
        [Route("viewListing")]
        public async Task<IActionResult> GetSearch(int listingId)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _listingProfileManager.ViewListing(listingId).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode, result.Payload);
            }).ConfigureAwait(false);
        }
    }
}

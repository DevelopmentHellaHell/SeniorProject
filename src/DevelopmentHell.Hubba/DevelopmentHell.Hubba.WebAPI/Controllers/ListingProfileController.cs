using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.WebAPI.DTO.Discovery;
using DevelopmentHell.Hubba.WebAPI.DTO.ListingProfile;
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

        [HttpPost]
        [Route("createListing")]
        public async Task<IActionResult> CreateListing(TitleToCreateListingDTO titleCreateListingDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _listingProfileManager.CreateListing(titleCreateListingDTO.Title).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("viewListing")]
        public async Task<IActionResult> ViewListing(ListingIdDTO listingIdDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _listingProfileManager.ViewListing(listingIdDTO.ListingId).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode, result.Payload);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("editListing")]
        public async Task<IActionResult> EditListing(ListingEditorDTO listingEditDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _listingProfileManager.EditListing(listingEditDTO).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("deleteListing")]
        public async Task<IActionResult> DeleteListing(ListingIdDTO listingIdDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _listingProfileManager.DeleteListing(listingIdDTO.ListingId).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("publishListing")]
        public async Task<IActionResult> PublishListing(ListingIdDTO listingIdDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _listingProfileManager.PublishListing(listingIdDTO.ListingId).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("addRating")]
        public async Task<IActionResult> AddRating(RatingToAddDTO addRatingDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _listingProfileManager.AddRating(addRatingDTO.ListingId, addRatingDTO.Rating, addRatingDTO.Comment, addRatingDTO.Anonymous).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("editRating")]
        public async Task<IActionResult> EditRating(ListingRatingEditorDTO listingRatingEditDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _listingProfileManager.EditRating(listingRatingEditDTO).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("deleteRating")]
        public async Task<IActionResult> DeleteRating(ListingIdDTO listingIdDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _listingProfileManager.DeleteRating(listingIdDTO.ListingId).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpGet]
        [Route("viewMyListings")]
        public async Task<IActionResult> ViewMyListings()
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _listingProfileManager.ViewUserListings().ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode, result.Payload);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("editListingFiles")]
        public async Task<IActionResult> EditListingFiles(FilesToEditDTO editFilesDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _listingProfileManager.EditListingFiles(editFilesDTO.ListingId, editFilesDTO.DeleteNames, editFilesDTO.AddFiles).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("editListingAvailabilities")]
        public async Task<IActionResult> EditListingAvailabilties(List<ListingAvailabilityDTO> editAvailabiltiiesDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _listingProfileManager.EditListingAvailabilities(editAvailabiltiiesDTO).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("getListingFiles")]
        public async Task<IActionResult> GetFiles(ListingIdDTO listingIdDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _listingProfileManager.GetListingFiles(listingIdDTO.ListingId).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode, result.Payload);
            }).ConfigureAwait(false);
        }
    }
}

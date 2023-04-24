

using DevelopmentHell.Hubba.ListingProfile.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using Microsoft.AspNetCore.Http.Features;
using System.Reflection;

namespace DevelopmentHell.Hubba.ListingProfile.Service.Implementations
{
    public class ListingProfileService : IListingProfileService
    {
        private IListingsDataAccess _listingDataAccess;
        private IListingAvailabilitiesDataAccess _listingAvailabilitiesDataAccess;
        private IListingHistoryDataAccess _listingHistoryDataAccess;
        private IRatingDataAccess _ratingDataAccess;
        private IUserAccountDataAccess _userAccountDataAccess;

        private ILoggerService _loggerService;

        public ListingProfileService(IListingsDataAccess listingDataAccess, IListingAvailabilitiesDataAccess listingAvailabilitiesDataAccess, IListingHistoryDataAccess listingHistoryDataAccess, IRatingDataAccess ratingDataAccess, IUserAccountDataAccess userAccountDataAccess, ILoggerService loggerService)
        {
            _listingDataAccess = listingDataAccess;
            _listingAvailabilitiesDataAccess = listingAvailabilitiesDataAccess;
            _listingHistoryDataAccess = listingHistoryDataAccess;
            _ratingDataAccess = ratingDataAccess;
            _userAccountDataAccess = userAccountDataAccess;
            _loggerService = loggerService;
        }

        public async Task<Result> CreateListing(int ownerId, string title)
        {
            Result result = new Result();

            Result createListingResult = await _listingDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            if (!createListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = createListingResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result<List<ListingViewDTO>>> GetUserListings(int ownerId)
        {
            Result<List<ListingViewDTO>> result = new Result<List<ListingViewDTO>>();

            Result<List<Listing>> getUserListingsResult = await _listingDataAccess.GetUserListings(ownerId).ConfigureAwait(false);
            if (!getUserListingsResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = getUserListingsResult.ErrorMessage;
                return result;
            }

            var getOwnerAverageRatings = await _ratingDataAccess.GetOwnerAverageRatings(Feature.Listing, ownerId).ConfigureAwait(false);
            if (!getOwnerAverageRatings.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = getOwnerAverageRatings.ErrorMessage;
                return result;
            }

            
            List<ListingViewDTO> listingDTOList = new();
            List<Listing> payload = getUserListingsResult.Payload!;
            foreach (Listing listing in payload)
            {
                ListingViewDTO temp = new ListingViewDTO()
                {
                    ListingId = (int)listing.ListingId!,
                    OwnerId = (int)listing.OwnerId!,
                    Title = (string)listing.Title!,
                    Description = listing.Description,
                    Price = listing.Price,
                    Location = listing.Location,
                    LastEdited = (DateTime)listing.LastEdited!,
                    Published = (bool)listing.Published!,
                };
                
                double? value = null;
                if (getOwnerAverageRatings.Payload is not null)
                {
                    if (getOwnerAverageRatings.Payload.TryGetValue(temp.ListingId, out double val)) {
                        value = val;
                    }  
                }
                temp.AverageRating = value;

                listingDTOList.Add(temp);
            }
            result.IsSuccessful = true;
            result.Payload = listingDTOList;
            return result;
        }

        public async Task<Result<ListingViewDTO>> GetListing(int listingId)
        {
            Result<ListingViewDTO> result = new Result<ListingViewDTO>();

            Result<Listing> getListingResult = await _listingDataAccess.GetListing(listingId).ConfigureAwait(false);
            if (!getListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = getListingResult.ErrorMessage;
                return result;
            }


            Listing payload = getListingResult.Payload!;
            ListingViewDTO temp = new ListingViewDTO()
            {
                OwnerId = payload.OwnerId!,
                ListingId = (int)payload.ListingId!,
                Title = payload.Title,
                Description = payload.Description,
                Location = payload.Location,
                Price = payload.Price,
                LastEdited = (DateTime)payload.LastEdited!,
                Published = (bool)payload.Published!
            };
            Result<string> getUsername = await GetUsername(temp.OwnerId).ConfigureAwait(false);
            if (!getUsername.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to retrieve username.";
                return result;
            }
            temp.OwnerUsername = getUsername.Payload!;

            Result<double?> getAverageRating = await _ratingDataAccess.GetAverageRating(Feature.Listing, temp.ListingId).ConfigureAwait(false);
            if (!getAverageRating.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to retrieve average rating.";
                return result;
            }
            temp.AverageRating = getAverageRating.Payload;

            result.IsSuccessful = true;
            result.Payload = temp;
            return result;
        }

        public async Task<Result<int>> GetListingOwnerId(int listingId)
        {
            Result<int> result = new Result<int>();

            Result<int> getListingOwnerIdResult = await _listingDataAccess.GetListingOwnerId(listingId).ConfigureAwait(false);
            if (!getListingOwnerIdResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = getListingOwnerIdResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            result.Payload = getListingOwnerIdResult.Payload;
            return result;
        }

        public async Task<Result<List<ListingAvailabilityViewDTO>>> GetListingAvailabilities(int listingId)
        {
            Result<List<ListingAvailabilityViewDTO>> result = new Result<List<ListingAvailabilityViewDTO>>();

            Result<List<ListingAvailability>> getListingAvailabilitiesResult = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);
            if (!getListingAvailabilitiesResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = getListingAvailabilitiesResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            List<ListingAvailabilityViewDTO> listingAvailabilitiesViewDTOList = new();
            List<ListingAvailability> payload = getListingAvailabilitiesResult.Payload;
            foreach (ListingAvailability listingAvailability in payload)
            {
                ListingAvailabilityViewDTO temp = new ListingAvailabilityViewDTO()
                {
                    ListingId = (int)listingAvailability.ListingId!,
                    AvailabilityId = (int)listingAvailability.AvailabilityId!,
                    StartTime = (DateTime)listingAvailability.StartTime!,
                    EndTime = (DateTime)listingAvailability.EndTime!
                };
                listingAvailabilitiesViewDTOList.Add(temp);
            }
            result.Payload = listingAvailabilitiesViewDTOList;


            return result;
        }

        public async Task<Result<bool>> CheckListingHistory(int listingId, int userId)
        {
            Result<bool> result = new Result<bool>();

            Result<int> countListingHistoryResult = await _listingHistoryDataAccess.CountListingHistory(listingId, userId).ConfigureAwait(false);
            if (!countListingHistoryResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = countListingHistoryResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            if (countListingHistoryResult.Payload != 1)
            {
                result.Payload = false;
                return result;
            }
            result.Payload = true;
            return result;
        }

        public async Task<Result> DeleteListing(int listingId)
        {
            Result result = new Result();

            Result deleteListingResult = await _listingDataAccess.DeleteListing(listingId).ConfigureAwait(false);
            if (!deleteListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = deleteListingResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> UpdateListing(ListingEditorDTO listing)
        {
            Result result = new Result();

            Result updateListingResult = await _listingDataAccess.UpdateListing(listing).ConfigureAwait(false);
            if (!updateListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = updateListingResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> PublishListing(int listingId)
        {
            Result result = new Result();

            Result publishListing = await _listingDataAccess.PublishListing(listingId).ConfigureAwait(false);
            if (!publishListing.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = publishListing.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> UnpublishListing(int listingId)
        {
            Result result = new Result();

            Result unpublishListing = await _listingDataAccess.UnpublishListing(listingId).ConfigureAwait(false);
            if (!unpublishListing.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = unpublishListing.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result<List<ListingRatingViewDTO>>> GetListingRatings(int listingId)
        {
            Result<List<ListingRatingViewDTO>> result = new Result<List<ListingRatingViewDTO>>();

            Result<List<ListingRating>> getListingRatingsResult = await _ratingDataAccess.GetListingRatings(listingId).ConfigureAwait(false);
            if (!getListingRatingsResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = getListingRatingsResult.ErrorMessage;
                return result;
            }


            List<ListingRatingViewDTO> listingRatingDTOList = new();
            List<ListingRating> payload = getListingRatingsResult.Payload;
            foreach (ListingRating listingRating in payload)
            {
                ListingRatingViewDTO temp = new ListingRatingViewDTO()
                {
                    ListingId = (int)listingRating.ListingId!,
                    UserId = (int)listingRating.UserId!,
                    LastEdited = (DateTime)listingRating.LastEdited!,
                    Rating = (int)listingRating.Rating,
                    Comment = listingRating.Comment
                };

                Result<string> getUsername = await GetUsername(temp.UserId).ConfigureAwait(false);
                if (!getUsername.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = "Unable to retrieve username.";
                    return result;
                }
                temp.Username = getUsername.Payload!;
                listingRatingDTOList.Add(temp);
            }
            result.Payload = listingRatingDTOList;
            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> AddRating(int listingId, int userId, int rating, string? comment, bool anonymous)
        {
            Result result = new Result();

            Result addRatingResult = await _ratingDataAccess.AddRating(Feature.Listing, listingId, userId, rating, comment, anonymous);
            if (!addRatingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = addRatingResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result<bool>> CheckListingRating(int listingId, int userId)
        {
            Result<bool> result = new Result<bool>();

            Result<int> countListingRatingResult = await _ratingDataAccess.CountRating(Feature.Listing, listingId, userId).ConfigureAwait(false);
            if (!countListingRatingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = countListingRatingResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            if (countListingRatingResult.Payload != 1)
            {
                result.Payload = false;
                return result;
            }
            result.Payload = true;
            return result;
        }

        public async Task<Result> DeleteRating(int listingId, int userId)
        {
            Result result = new Result();

            Result deleteListingResult = await _ratingDataAccess.DeleteRating(Feature.Listing, listingId, userId).ConfigureAwait(false);
            if (!deleteListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = deleteListingResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> UpdateRating(ListingRatingEditorDTO listingRating)
        {
            Result result = new Result();

            Result updateRatingResult = await _ratingDataAccess.UpdateListingRating(listingRating).ConfigureAwait(false);
            if (!updateRatingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = updateRatingResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> UpdateListingAvailabilities(List<ListingAvailabilityDTO> listingAvailabilities)
        {
            Result result = new Result();
            foreach (ListingAvailabilityDTO listingAvailability in listingAvailabilities)
            {
                Result updateListingAvailabilityResult = await _listingAvailabilitiesDataAccess.UpdateListingAvailability(listingAvailability).ConfigureAwait(false);
                if (!updateListingAvailabilityResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = updateListingAvailabilityResult.ErrorMessage;
                    return result;
                }
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> AddListingAvailabilities(List<ListingAvailabilityDTO> listingAvailabilities)
        {
            Result result = new Result();

            Result addListingResult = await _listingAvailabilitiesDataAccess.AddListingAvailabilities(listingAvailabilities).ConfigureAwait(false);
            if (!addListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = addListingResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> DeleteListingAvailabilities(List<ListingAvailabilityDTO> listingAvailabilities)
        {
            Result result = new Result();

            Result deleteListingResult = await _listingAvailabilitiesDataAccess.DeleteListingAvailabilities(listingAvailabilities).ConfigureAwait(false);
            if (!deleteListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = deleteListingResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        private async Task<Result<string>> GetUsername(int userId)
        {
            Result<string> result = new Result<string>();

            Result<string> getListingOwnerIdResult = await _userAccountDataAccess.GetEmail(userId).ConfigureAwait(false);
            if (!getListingOwnerIdResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = getListingOwnerIdResult.ErrorMessage;
                return result;
            }

            string username = getListingOwnerIdResult.Payload!.Split('@')[0];

            result.IsSuccessful = true;
            result.Payload = username;
            return result;
        }

        public async Task<Result<int>> GetListingId(string title, int userId)
        {
            return await _listingDataAccess.GetListingId(userId, title).ConfigureAwait(false);

        }
    }
}
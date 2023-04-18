using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.ListingProfile.Manager.Abstractions;
using System.Configuration;

using System.Security.Claims;

namespace DevelopmentHell.Hubba.ListingProfile.Manager.Implementations
{
    public class ListingProfileManager : IListingProfileManager
    {
        private IListingProfileService _listingsService;
        //private IFileService _fileService;
        private IRatingService _ratingService;
        private IAuthorizationService _authorizationService;
        private ILoggerService _loggerService;
        private IValidationService _validationService;
        private ICryptographyService _cryptographyService;


        public ListingProfileManager(IListingProfileService listingsService,
            //IFileService fileService, IRatingService ratingService,
            IAuthorizationService authorizationService, ILoggerService loggerService, IValidationService validationService, ICryptographyService cryptographyService)
        {
            _listingsService = listingsService;
            //_fileService = fileService;
            //_ratingService = ratingService;
            _authorizationService = authorizationService;
            _loggerService = loggerService;
            _validationService = validationService;
            _cryptographyService = cryptographyService;
        }

        public async Task<Result> CreateListing(string title)
        {
            Result result = new Result();

            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized user.";
                return result;
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            int ownerId = int.Parse(stringAccountId);

            //assign Username from token
            var stringAccountUsername = claimsPrincipal?.FindFirstValue("azp");
            if (stringAccountUsername is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            string ownerUsername = (string)stringAccountUsername;


            //validate title
            Result validationResult = _validationService.ValidateTitle(title);
            if (!validationResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = validationResult.ErrorMessage;
                return result;
            }


            Result createListingResult = await _listingsService.CreateListing(ownerId, title).ConfigureAwait(false);
            if (!createListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = createListingResult.ErrorMessage;
                return result;
            }

            //log 
            string userHashKey = ConfigurationManager.AppSettings["UserHashKey"]!;
            Result<HashData> userHashResult = _cryptographyService.HashString(ownerUsername, userHashKey);
            if (!userHashResult.IsSuccessful || userHashResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, unexpected error. Please contact system administrator.";
                return result;
            }
            string userHash = Convert.ToBase64String(userHashResult.Payload.Hash!);
            _loggerService.Log(LogLevel.INFO, Category.BUSINESS, $"Successful listing creation from: {ownerUsername}.", userHash);

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> ViewUserListings()
        {
            Result<List<ListingViewDTO>> result = new Result<List<ListingViewDTO>>();

            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized user.";
                return result;
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            int ownerId = int.Parse(stringAccountId);

            Result<List<ListingViewDTO>> getUserListingsResult = await _listingsService.GetUserListings(ownerId).ConfigureAwait(false);
            if (!getUserListingsResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to retrieve user listings.";
                return result;
            }
            result.Payload = getUserListingsResult.Payload;


            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> ViewListing(int listingId)
        {
            Result<List<Object>> result = new Result<List<Object>>();

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            int userId = int.Parse(stringAccountId);

            //get listing
            Result<ListingViewDTO> getListingResult = await _listingsService.GetListing(listingId).ConfigureAwait(false);
            if (!getListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = getListingResult.ErrorMessage;
                return result;
            }
            int ownerId = (int)getListingResult.Payload!.OwnerId;

            //check if published
            if (getListingResult.Payload!.Published == false)
            {
                //check userid to ownerId
                if (ownerId != userId)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = "Unauthorized user.";
                    return result;
                }
            }
            result.Payload![0] = getListingResult.Payload;

            //get availabilities
            Result<List<ListingAvailabilityDTO>> getListingAvailabilityResult = await _listingsService.GetListingAvailabilities(listingId).ConfigureAwait(false);
            if (!getListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = getListingResult.ErrorMessage;
                return result;
            }
            result.Payload[1] = getListingAvailabilityResult.Payload!;


            //get files
            //Result<List<Object>> getListingFilesResult = await _fileService.GetFiles(listingId).ConfigureAwait(false);
            //if (!getListingResult.IsSuccessful)
            //{
            //    result.IsSuccessful = false;
            //    result.ErrorMessage = getListingResult.ErrorMessage;
            //    return result;
            //}
            //result.Payload[2] = getListingFilesResult.Payload;

            //get rating comments
            Result<List<ListingRatingViewDTO>> getListingRatingsResult = await _listingsService.GetListingRatings(listingId).ConfigureAwait(false);
            if (!getListingRatingsResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = getListingRatingsResult.ErrorMessage;
                return result;
            }
            result.Payload[3] = getListingRatingsResult.Payload!;


            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> EditListing(ListingEditorDTO listing)
        {

            Result result = new Result();

            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized user.";
                return result;
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            int userId = int.Parse(stringAccountId);

            //validate user is owner
            if (userId != listing.OwnerId)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized user.";
                return result;
            }

            //validate changed field in a Listing
            var values = new Dictionary<string, object>();
            foreach (var column in listing.GetType().GetProperties())
            {
                var value = column.GetValue(listing);
                if (value is null || column.Name == "ListingId" || column.Name == "OwnerId") continue;

                //validate description
                if (column.Name == "Description")
                {
                    Result validationResult = _validationService.ValidateBodyText((string)value);
                    if (!validationResult.IsSuccessful)
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = validationResult.ErrorMessage;
                        return result;
                    }
                }

                //possible location or price checks
            }

            Result updateListingResult = await _listingsService.UpdateListing(listing).ConfigureAwait(false);
            if (!updateListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to update listing.";
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> EditListingAvailabilities(ListingAvailabilityDTO[] listingAvailabilities)
        {
            Result result = new Result();

            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized user.";
                return result;
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            int userId = int.Parse(stringAccountId);

            //validate user is owner
            if (userId != listingAvailabilities[0].OwnerId)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized user.";
                return result;
            }

            //validate starttime and endtime
            //split listingavailabilities into 3 groups
            List<ListingAvailabilityDTO> addListingAvailabilities = new();
            List<ListingAvailabilityDTO> updateListingAvailabilities = new();
            List<ListingAvailabilityDTO> deleteListingAvailabilities = new();

            foreach (ListingAvailabilityDTO listingAvailability in listingAvailabilities)
            {
                Result validationResult = _validationService.ValidateAvailability(listingAvailability);
                if (!validationResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = validationResult.ErrorMessage;
                    return result;
                }

                if (listingAvailability.Action == AvailabilityAction.Delete)
                {
                    deleteListingAvailabilities.Add(listingAvailability);
                }

                if (listingAvailability.Action == AvailabilityAction.Add)
                {
                    addListingAvailabilities.Add(listingAvailability);
                }

                if (listingAvailability.Action == AvailabilityAction.Update)
                {
                    updateListingAvailabilities.Add(listingAvailability);
                }
            }


            //TO DO
            //check for bookings in update and delete lists
            //check db to ensure there is no availability in between existing


            //add listingavailabilities
            Result addListingAvailabilitiesResult = await _listingsService.AddListingAvailabilities(addListingAvailabilities).ConfigureAwait(false);
            if (!addListingAvailabilitiesResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to add new listing availabilities.";
            }

            //update listingavailabilties
            Result updateListingAvailabilitiesResult = await _listingsService.UpdateListingAvailabilities(updateListingAvailabilities).ConfigureAwait(false);
            if (!updateListingAvailabilitiesResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to update preexisting listing availabilities.";
            }



            //delete listingavailabilities
            Result deleteListingAvailabilitiesResult = await _listingsService.DeleteListingAvailabilities(deleteListingAvailabilities).ConfigureAwait(false);
            if (!deleteListingAvailabilitiesResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to remove preexisting listing availabilities.";
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> EditListingFiles(HubbaFile[] listingFiles)
        {
            //authorize

            //for each file, validate model and validate file (name, type, size)

            throw new NotImplementedException();
        }

        public async Task<Result> DeleteListing(int listingId)
        {
            Result result = new Result();

            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized user.";
                return result;
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            int userId = int.Parse(stringAccountId);

            //check ownerId of listing = userId
            Result<int> getListingOwnerIdResult = await _listingsService.GetListingOwnerId(listingId).ConfigureAwait(false);
            if (!getListingOwnerIdResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = getListingOwnerIdResult.ErrorMessage;
                return result;
            }

            if (userId != getListingOwnerIdResult.Payload)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized user.";
                return result;
            }

            Result deleteListingResult = await _listingsService.DeleteListing(listingId).ConfigureAwait(false);
            if (!deleteListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to delete listing.";
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> PublishListing(int listingId)
        {
            Result result = new Result();

            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized user.";
                return result;
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            int userId = int.Parse(stringAccountId);

            //get listing
            Result<ListingViewDTO> getListingResult = await _listingsService.GetListing(listingId).ConfigureAwait(false);
            if (!getListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to retrieve specified listing.";
                return result;
            }
            int ownerId = (int)getListingResult.Payload!.OwnerId;

            //validate listing model
            Result validationListingResult = _validationService.ValidateModel(getListingResult.Payload);
            if (!validationListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = validationListingResult.ErrorMessage;
                return result;
            }

            //check if published
            if (getListingResult.Payload.Published == true)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Listing already published.";
                return result;
            }

            if (getListingResult.Payload.Published == false)
            {
                //check userid to ownerId
                if (ownerId != userId)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = "Unauthorized user.";
                    return result;
                }
            }

            //get availabilities
            Result<List<ListingAvailabilityDTO>> getListingAvailabilityResult = await _listingsService.GetListingAvailabilities(listingId).ConfigureAwait(false);
            if (!getListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = getListingResult.ErrorMessage;
                return result;
            }

            if (getListingAvailabilityResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Listing contains no availabilities.";
                return result;
            }

            //validate availabilities model
            foreach (ListingAvailabilityDTO listingAvailability in getListingAvailabilityResult.Payload)
            {
                Result validationAvailabilityResult = _validationService.ValidateModel(listingAvailability);
                if (!validationAvailabilityResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = validationAvailabilityResult.ErrorMessage;
                    return result;
                }
            }



            //get files
            //Result<List<Object>> getListingFilesResult = await _fileService.GetFiles(listingId).ConfigureAwait(false);
            //if (!getListingResult.IsSuccessful)
            //{
            //    result.IsSuccessful = false;
            //    result.ErrorMessage = getListingResult.ErrorMessage;
            //    return result;
            //}

            //if (getListingFilesResult.Payload is null)
            //{
            //    result.IsSuccessful = false;
            //    result.ErrorMessage = "Listing contains no availabilities.";
            //    return result;
            //}

            //validate files model
            //foreach (  in getListingFilesResult.Payload)
            //{
            //    Result validationFileResult = _validationService.ValidateModel();
            //    if (!validationFileResult.IsSuccessful)
            //    {
            //        result.IsSuccessful = false;
            //        result.ErrorMessage = validationFileResult.ErrorMessage;
            //        return result;
            //    }
            //}

            Result publishListingResult = await _listingsService.PublishListing(listingId).ConfigureAwait(false);
            if (!publishListingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to publish listing.";
                return result;
            }


            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> AddRating(int listingId, int rating, string? comment, bool anonymous)
        {
            Result result = new Result();

            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized user.";
                return result;
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            int userId = int.Parse(stringAccountId);

            //check listing history          
            Result<bool> checkListingHistoryResult = await _listingsService.CheckListingHistory(listingId, userId).ConfigureAwait(false);
            if (!checkListingHistoryResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to retrieve listing history.";
                return result;
            }
            if (checkListingHistoryResult.Payload == false)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized user.";
                return result;
            }

            //validate comment
            if (comment is not null)
            {
                Result validationCommentResult = _validationService.ValidateBodyText(comment);
                if (!validationCommentResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = validationCommentResult.ErrorMessage;
                    return result;
                }
            }

            //validate rating
            Result validationRatingResult = _validationService.ValidateRating(rating);
            if (!validationRatingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = validationRatingResult.ErrorMessage;
                return result;
            }

            Result addReviewResult = await _listingsService.AddRating(listingId, userId, rating, comment, anonymous).ConfigureAwait(false);
            if (!addReviewResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to add review.";
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> EditRating(ListingRatingEditorDTO listingRating)
        {
            Result result = new();

            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized user.";
                return result;
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            int userId = int.Parse(stringAccountId);

            Result<bool> checkListingRating = await _listingsService.CheckListingRating(listingRating.ListingId, userId).ConfigureAwait(false);
            if (!checkListingRating.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to retrieve count.";
                return result;
            }

            if (checkListingRating.Payload == false)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized user.";
                return result;
            }
            listingRating.UserId = userId;

            //validate comment
            if (listingRating.Comment is not null)
            {
                Result validationCommentResult = _validationService.ValidateBodyText(listingRating.Comment);
                if (!validationCommentResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = validationCommentResult.ErrorMessage;
                    return result;
                }
            }

            //validate rating
            Result validationRatingResult = _validationService.ValidateRating(listingRating.Rating);
            if (!validationRatingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = validationRatingResult.ErrorMessage;
                return result;
            }

            Result updateReviewResult = await _listingsService.UpdateRating(listingRating).ConfigureAwait(false);
            if (!updateReviewResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to edit review.";
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> DeleteRating(int listingId)
        {
            Result result = new();

            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized user.";
                return result;
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            int userId = int.Parse(stringAccountId);

            Result<bool> checkListingRating = await _listingsService.CheckListingRating(listingId, userId).ConfigureAwait(false);
            if (!checkListingRating.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to retrieve count.";
                return result;
            }

            if (checkListingRating.Payload == false)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized user.";
                return result;
            }

            Result deleteRatingResult = await _listingsService.DeleteRating(listingId, userId).ConfigureAwait(false);
            if (!deleteRatingResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to delete review.";
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }
    }
}
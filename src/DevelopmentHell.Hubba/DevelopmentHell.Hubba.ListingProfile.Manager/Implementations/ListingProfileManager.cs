using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.ListingProfile.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.ListingProfile.Manager.Abstractions;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

using System.Security.Claims;

namespace DevelopmentHell.Hubba.ListingProfile.Manager.Implementations
{
    public class ListingProfileManager : IListingProfileManager
    {
        private IListingProfileService _listingsService;
        private IFileService _fileService;
        private IAuthorizationService _authorizationService;
        private ILoggerService _loggerService;
        private IValidationService _validationService;
        private ICryptographyService _cryptographyService;


        public ListingProfileManager(IListingProfileService listingsService, IFileService fileService, IAuthorizationService authorizationService, ILoggerService loggerService, IValidationService validationService, ICryptographyService cryptographyService)
        {
            _listingsService = listingsService;
            _fileService = fileService;
            _authorizationService = authorizationService;
            _loggerService = loggerService;
            _validationService = validationService;
            _cryptographyService = cryptographyService;
        }

        public async Task<Result> CreateListing(string title)
        {
            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                return new(Result.Failure("Error, invalid access token format.", StatusCodes.Status400BadRequest));
            }
            int ownerId = int.Parse(stringAccountId);

            //assign Username from token
            var stringAccountUsername = claimsPrincipal?.FindFirstValue("azp");
            if (stringAccountUsername is null)
            {
                return new(Result.Failure("Error, invalid access token format.", StatusCodes.Status400BadRequest));
            }
            string ownerUsername = (string)stringAccountUsername;


            //validate title
            Result validationResult = _validationService.ValidateTitle(title);
            if (!validationResult.IsSuccessful)
            {
                return new(Result.Failure(validationResult.ErrorMessage!, StatusCodes.Status400BadRequest));
            }


            Result createListingResult = await _listingsService.CreateListing(ownerId, title).ConfigureAwait(false);
            if (!createListingResult.IsSuccessful)
            {
                return new(Result.Failure(createListingResult.ErrorMessage!, StatusCodes.Status400BadRequest));
            }

            Result<int> getListingId = await _listingsService.GetListingId(title, ownerId).ConfigureAwait(false);
            if (!getListingId.IsSuccessful)
            {
                return new(Result.Failure(getListingId.ErrorMessage!, StatusCodes.Status400BadRequest));
            }
            int listingId = getListingId.Payload;

            Result createFileDir = await _fileService.CreateDir("ListingProfiles/" + listingId).ConfigureAwait(false);
            if (!createFileDir.IsSuccessful)
            {
                return new(Result.Failure(createFileDir.ErrorMessage!, StatusCodes.Status400BadRequest));
            }

            //log 
            string userHashKey = ConfigurationManager.AppSettings["UserHashKey"]!;
            Result<HashData> userHashResult = _cryptographyService.HashString(ownerUsername, userHashKey);
            if (!userHashResult.IsSuccessful || userHashResult.Payload is null)
            {
                return new(Result.Failure("Error, unexpected error. Please contact system administrator.", StatusCodes.Status400BadRequest));
            }
            string userHash = Convert.ToBase64String(userHashResult.Payload.Hash!);
            _loggerService.Log(LogLevel.INFO, Category.BUSINESS, $"Successful listing creation from: {ownerUsername}.", userHash);

            return Result.Success();
        }

        public async Task<Result<List<ListingViewDTO>>> ViewUserListings()
        {
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                return new(Result.Failure("Error, invalid access token format.", StatusCodes.Status400BadRequest));
            }
            int ownerId = int.Parse(stringAccountId);

            Result<List<ListingViewDTO>> getUserListingsResult = await _listingsService.GetUserListings(ownerId).ConfigureAwait(false);
            if (!getUserListingsResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to retrieve user listings.", StatusCodes.Status400BadRequest));
            }
            
            var payload = getUserListingsResult.Payload;
            return Result<List<ListingViewDTO>>.Success(payload);
        }

        public async Task<Result<Dictionary<string, object>>> ViewListing(int listingId)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();

            //get listing
            Result<ListingViewDTO> getListingResult = await _listingsService.GetListing(listingId).ConfigureAwait(false);
            if (!getListingResult.IsSuccessful)
            {
                return new(Result.Failure(getListingResult.ErrorMessage, StatusCodes.Status400BadRequest));
            }
            int ownerId = (int)getListingResult.Payload!.OwnerId;

            //check if published
            if (getListingResult.Payload!.Published == false)
            {
                //assign Id from token
                var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
                var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
                if (stringAccountId is null)
                {
                    return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
                }

                int userId = int.Parse(stringAccountId);

                //check userid to ownerId
                if (ownerId != userId)
                {
                    return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
                }
            }
            
            payload.Add("listing", getListingResult.Payload);

            //get availabilities
            Result<List<ListingAvailabilityDTO>> getListingAvailabilityResult = await _listingsService.GetListingAvailabilities(listingId).ConfigureAwait(false);
            if (!getListingResult.IsSuccessful)
            {
                return new(Result.Failure(getListingResult.ErrorMessage, StatusCodes.Status400BadRequest));
            }
            payload.Add("availabilities", getListingAvailabilityResult.Payload);


            //get files
            Result<List<string>> getFiles = await _fileService.GetFilesInDir("Listing Profiles/" + listingId).ConfigureAwait(false);
            if (!getFiles.IsSuccessful)
            {
                return new(Result.Failure(getFiles.ErrorMessage, StatusCodes.Status400BadRequest));
            }
            payload.Add("files", getFiles.Payload);

            //get rating comments
            Result<List<ListingRatingViewDTO>> getListingRatingsResult = await _listingsService.GetListingRatings(listingId).ConfigureAwait(false);
            if (!getListingRatingsResult.IsSuccessful)
            {
                return new(Result.Failure(getListingRatingsResult.ErrorMessage, StatusCodes.Status400BadRequest));
            }
            payload.Add("ratings", getListingRatingsResult.Payload);

            return Result<Dictionary<string, object>>.Success(payload);
        }

        public async Task<Result> EditListing(ListingEditorDTO listing)
        {
            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                return new(Result.Failure("Error, invalid access token format.", StatusCodes.Status400BadRequest));
            }
            int userId = int.Parse(stringAccountId);

            //validate user is owner
            if (userId != listing.OwnerId)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
            }

            //validate changed field in a Listing
            var values = new Dictionary<string, object>();
            foreach (var column in listing.GetType().GetProperties())
            {
                var value = column.GetValue(listing);
                if (value is null || column.Name == "ListingId" || column.Name == "OwnerId") continue;

                
                if (column.Name == "Description" || column.Name == "Location")
                {
                    Result validationResult = _validationService.ValidateBodyText((string)value);
                    if (!validationResult.IsSuccessful)
                    {
                        return new(Result.Failure(validationResult.ErrorMessage!, StatusCodes.Status400BadRequest));
                    }
                }

                if (column.Name == "Price")
                {
                    listing.Price = Math.Round(Convert.ToDouble(listing.Price), 2, MidpointRounding.ToZero);
                }
            }

            Result updateListingResult = await _listingsService.UpdateListing(listing).ConfigureAwait(false);
            if (!updateListingResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to update listing.", StatusCodes.Status400BadRequest));
            }

            return Result.Success();
        }

        public async Task<Result> EditListingAvailabilities(List<ListingAvailabilityDTO> listingAvailabilities)
        {
            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                return new(Result.Failure("Error, invalid access token format.", StatusCodes.Status400BadRequest));
            }
            int userId = int.Parse(stringAccountId);

            //validate user is owner
            if (userId != listingAvailabilities[0].OwnerId)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
            }

            //split listingavailabilities into 3 groups
            List<ListingAvailabilityDTO> addListingAvailabilities = new();
            List<ListingAvailabilityDTO> updateListingAvailabilities = new();
            List<ListingAvailabilityDTO> deleteListingAvailabilities = new();

            foreach (ListingAvailabilityDTO listingAvailability in listingAvailabilities)
            {
                Result validationResult = _validationService.ValidateAvailability(listingAvailability);
                if (!validationResult.IsSuccessful)
                {
                    return new(Result.Failure(validationResult.ErrorMessage!, StatusCodes.Status400BadRequest));
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

            bool errorFound = false;
            string errorMessage = String.Empty;

            //add listingavailabilities
            if (addListingAvailabilities.Count > 0)
            {
                Result addListingAvailabilitiesResult = await _listingsService.AddListingAvailabilities(addListingAvailabilities).ConfigureAwait(false);
                if (!addListingAvailabilitiesResult.IsSuccessful)
                {
                    errorFound = true;
                    errorMessage += "Unable to add new listing availabilities.\n";
                }
            }
            
            //update listingavailabilties
            if (updateListingAvailabilities.Count > 0)
            {
                Result updateListingAvailabilitiesResult = await _listingsService.UpdateListingAvailabilities(updateListingAvailabilities).ConfigureAwait(false);
                if (!updateListingAvailabilitiesResult.IsSuccessful)
                {
                    errorFound = true;
                    errorMessage += "Unable to update preexisting listing availabilities.\n";
                }
            }

            //delete listingavailabilities
            if (deleteListingAvailabilities.Count > 0)
            {
                Result deleteListingAvailabilitiesResult = await _listingsService.DeleteListingAvailabilities(deleteListingAvailabilities).ConfigureAwait(false);
                if (!deleteListingAvailabilitiesResult.IsSuccessful)
                {
                    errorFound = true;
                    errorMessage += "Unable to remove preexisting listing availabilities.\n";
                }
            }
            
            if (errorFound)
            {
                return new(Result.Failure(errorMessage, StatusCodes.Status400BadRequest));
            }

            return Result.Success();
        }

        public async Task<Result> EditListingFiles(List<IFormFile> listingFiles)
        {
            //authorize

            //for each file, validate model and validate file (name, type, size)

            throw new NotImplementedException();
        }

        public async Task<Result> DeleteListing(int listingId)
        {
            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                return new(Result.Failure("Error, invalid access token format.", StatusCodes.Status400BadRequest));
            }
            int userId = int.Parse(stringAccountId);

            //check ownerId of listing = userId
            Result<int> getListingOwnerIdResult = await _listingsService.GetListingOwnerId(listingId).ConfigureAwait(false);
            if (!getListingOwnerIdResult.IsSuccessful)
            {
                return new(Result.Failure(getListingOwnerIdResult.ErrorMessage, StatusCodes.Status400BadRequest));
            }

            if (userId != getListingOwnerIdResult.Payload)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
            }

            Result deleteListingResult = await _listingsService.DeleteListing(listingId).ConfigureAwait(false);
            if (!deleteListingResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to delete listing.", StatusCodes.Status400BadRequest));
            }

            return Result.Success();
        }

        public async Task<Result> PublishListing(int listingId)
        {
            //Result result = new Result();

            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                return new(Result.Failure("Error, invalid access token format.", StatusCodes.Status400BadRequest));
            }
            int userId = int.Parse(stringAccountId);

            //get listing
            Result<ListingViewDTO> getListingResult = await _listingsService.GetListing(listingId).ConfigureAwait(false);
            if (!getListingResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to retrieve specified listing.", StatusCodes.Status400BadRequest));
            }
            int ownerId = (int)getListingResult.Payload!.OwnerId;

            if (!userId.Equals(ownerId))
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status401Unauthorized));
            }

            //validate listing model
            Result validationListingResult = _validationService.ValidateModel(getListingResult.Payload);
            if (!validationListingResult.IsSuccessful)
            {
                return new(Result.Failure(validationListingResult.ErrorMessage!, StatusCodes.Status400BadRequest));
            }

            //check if published
            if (getListingResult.Payload.Published == true)
            {
                return new(Result.Failure("Listing already published.", StatusCodes.Status400BadRequest));
            }

            if (getListingResult.Payload.Published == false)
            {
                //check userid to ownerId
                if (ownerId != userId)
                {
                    return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
                }
            }

            //get availabilities
            Result<List<ListingAvailabilityDTO>> getListingAvailabilityResult = await _listingsService.GetListingAvailabilities(listingId).ConfigureAwait(false);
            if (!getListingResult.IsSuccessful)
            {
                return new(Result.Failure(getListingResult.ErrorMessage!, StatusCodes.Status400BadRequest));
            }

            if (getListingAvailabilityResult.Payload is null)
            {
                return new(Result.Failure("Listing contains no availabilities.", StatusCodes.Status400BadRequest));
            }

            //validate availabilities model
            foreach (ListingAvailabilityDTO listingAvailability in getListingAvailabilityResult.Payload)
            {
                Result validationAvailabilityResult = _validationService.ValidateModel(listingAvailability);
                if (!validationAvailabilityResult.IsSuccessful)
                {
                    return new(Result.Failure(validationAvailabilityResult.ErrorMessage!, StatusCodes.Status400BadRequest));
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
                return new(Result.Failure("Unable to publish listing.", StatusCodes.Status400BadRequest));

            }

            return Result.Success();
        }

        public async Task<Result> AddRating(int listingId, int rating, string? comment, bool anonymous)
        {
            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                return new(Result.Failure("Error, invalid access token format.", StatusCodes.Status400BadRequest));
            }
            int userId = int.Parse(stringAccountId);

            //check listing history          
            Result<bool> checkListingHistoryResult = await _listingsService.CheckListingHistory(listingId, userId).ConfigureAwait(false);
            if (!checkListingHistoryResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to retrieve listing history.", StatusCodes.Status400BadRequest));
            }
            if (checkListingHistoryResult.Payload == false)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status401Unauthorized));
            }

            //validate comment
            if (comment is not null)
            {
                Result validationCommentResult = _validationService.ValidateBodyText(comment);
                if (!validationCommentResult.IsSuccessful)
                {
                    return new(Result.Failure(validationCommentResult.ErrorMessage!, StatusCodes.Status400BadRequest));
                }
            }

            //validate rating
            Result validationRatingResult = _validationService.ValidateRating(rating);
            if (!validationRatingResult.IsSuccessful)
            {
                return new(Result.Failure(validationRatingResult.ErrorMessage!, StatusCodes.Status400BadRequest));
            }

            Result addReviewResult = await _listingsService.AddRating(listingId, userId, rating, comment, anonymous).ConfigureAwait(false);
            if (!addReviewResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to add review.", StatusCodes.Status400BadRequest));
            }

            return Result.Success();
        }

        public async Task<Result> EditRating(ListingRatingEditorDTO listingRating)
        {
            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                return new(Result.Failure("Error, invalid access token format.", StatusCodes.Status400BadRequest));
            }
            int userId = int.Parse(stringAccountId);

            Result<bool> checkListingRating = await _listingsService.CheckListingRating(listingRating.ListingId, userId).ConfigureAwait(false);
            if (!checkListingRating.IsSuccessful)
            {
                return new(Result.Failure("Unable to retrieve count.", StatusCodes.Status400BadRequest));
            }

            if (checkListingRating.Payload == false)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
            }
            listingRating.UserId = userId;

            //validate comment
            if (listingRating.Comment is not null)
            {
                Result validationCommentResult = _validationService.ValidateBodyText(listingRating.Comment);
                if (!validationCommentResult.IsSuccessful)
                {
                    return new(Result.Failure(validationCommentResult.ErrorMessage!, StatusCodes.Status400BadRequest));
                }
            }

            //validate rating
            Result validationRatingResult = _validationService.ValidateRating(listingRating.Rating);
            if (!validationRatingResult.IsSuccessful)
            {
                return new(Result.Failure(validationRatingResult.ErrorMessage!, StatusCodes.Status400BadRequest));
            }

            Result updateReviewResult = await _listingsService.UpdateRating(listingRating).ConfigureAwait(false);
            if (!updateReviewResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to edit review.", StatusCodes.Status400BadRequest));
            }

            return Result.Success();
        }

        public async Task<Result> DeleteRating(int listingId)
        {
            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
            }

            //assign Id from token
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                return new(Result.Failure("Error, invalid access token format.", StatusCodes.Status400BadRequest));
            }
            int userId = int.Parse(stringAccountId);

            Result<bool> checkListingRating = await _listingsService.CheckListingRating(listingId, userId).ConfigureAwait(false);
            if (!checkListingRating.IsSuccessful)
            {
                return new(Result.Failure("Unable to retrieve count.", StatusCodes.Status400BadRequest));
            }

            if (checkListingRating.Payload == false)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status400BadRequest));
            }

            Result deleteRatingResult = await _listingsService.DeleteRating(listingId, userId).ConfigureAwait(false);
            if (!deleteRatingResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to delete review.", StatusCodes.Status400BadRequest));
            }

            return Result.Success();
        }
    }
}
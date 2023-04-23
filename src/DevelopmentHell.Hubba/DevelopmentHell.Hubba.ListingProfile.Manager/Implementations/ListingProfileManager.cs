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
using Microsoft.AspNetCore.StaticFiles;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
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
                return new(Result.Failure(getListingResult.ErrorMessage!, StatusCodes.Status400BadRequest));
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
            
            payload.Add("Listing", getListingResult.Payload);

            //get availabilities
            Result<List<ListingAvailabilityViewDTO>> getListingAvailabilityResult = await _listingsService.GetListingAvailabilities(listingId).ConfigureAwait(false);
            if (!getListingResult.IsSuccessful)
            {
                return new(Result.Failure(getListingResult.ErrorMessage!, StatusCodes.Status400BadRequest));
            }
            payload.Add("Availabilities", getListingAvailabilityResult.Payload);


            //get files
            List<string> files = new List<string>();
            Result<List<string>> getPictureFiles = await _fileService.GetFilesInDir("ListingProfiles/" + listingId + "/Pictures").ConfigureAwait(false);
            if (!getPictureFiles.IsSuccessful)
            {
                return new(Result.Failure(getPictureFiles.ErrorMessage!, StatusCodes.Status400BadRequest));
            }
            files.AddRange(getPictureFiles.Payload);

            Result<List<string>> getVideoFiles = await _fileService.GetFilesInDir("ListingProfiles/" + listingId + "/Videos").ConfigureAwait(false);
            if (!getVideoFiles.IsSuccessful)
            {
                return new(Result.Failure(getVideoFiles.ErrorMessage!, StatusCodes.Status400BadRequest));
            }
            files.AddRange(getVideoFiles.Payload);
            payload.Add("Files", files);

            //get rating comments
            Result<List<ListingRatingViewDTO>> getListingRatingsResult = await _listingsService.GetListingRatings(listingId).ConfigureAwait(false);
            if (!getListingRatingsResult.IsSuccessful)
            {
                return new(Result.Failure(getListingRatingsResult.ErrorMessage!, StatusCodes.Status400BadRequest));
            }
            payload.Add("Ratings", getListingRatingsResult.Payload);

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

            Result<ListingViewDTO> getUpdatedListing = await _listingsService.GetListing(listing.ListingId).ConfigureAwait(false);
            if (!getUpdatedListing.IsSuccessful)
            {
                return new(Result.Failure("Unable to check updated listing.", StatusCodes.Status400BadRequest));
            }

            if (getUpdatedListing.Payload.Description is null || getUpdatedListing.Payload.Price is null || getUpdatedListing.Payload.Location is null)
            {
                Result unpublish = await _listingsService.UnpublishListing(listing.ListingId).ConfigureAwait(false);
                if (!unpublish.IsSuccessful)
                {
                    return new(Result.Failure("Unable to unpublish listing", StatusCodes.Status400BadRequest));
                }
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

            int listingId = listingAvailabilities[0].ListingId;
            //split listingavailabilities into 3 groups
            List<ListingAvailabilityDTO> addListingAvailabilities = new();
            List<ListingAvailabilityDTO> updateListingAvailabilities = new();
            List<ListingAvailabilityDTO> deleteListingAvailabilities = new();

            foreach (ListingAvailabilityDTO listingAvailability in listingAvailabilities)
            {
                if (listingAvailability.Action == AvailabilityAction.Add)
                {
                    Result validationResult = _validationService.ValidateAvailability(listingAvailability);
                    if (!validationResult.IsSuccessful)
                    {
                        return new(Result.Failure(validationResult.ErrorMessage!, StatusCodes.Status400BadRequest));
                    }
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

            Result<List<ListingAvailabilityViewDTO>> availabilitiesCheck = await _listingsService.GetListingAvailabilities(listingId).ConfigureAwait(false);
            if (!availabilitiesCheck.IsSuccessful)
            {
                return new(Result.Failure("Unable to check remaining availabilities", StatusCodes.Status400BadRequest));
            }

            if (availabilitiesCheck.Payload.Count == 0)
            {
                Result unpublish = await _listingsService.UnpublishListing(listingId).ConfigureAwait(false);
                if (!unpublish.IsSuccessful)
                {
                    return new(Result.Failure("Unable to unpublish listing", StatusCodes.Status400BadRequest));
                }
            }

            return Result.Success();
        }

        public async Task<Result> EditListingFiles(int listingId, List<string>? deleteListingNames, Dictionary<string, byte[]> addListingFiles)
        {
            //authorize
            if (!_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                return new(Result.Failure("Unauthorized user.", StatusCodes.Status401Unauthorized));
            }
            int picturesStored;
            string dir = "ListingProfiles/" + listingId + "/";
            //for each file, validate model and validate file (name, type, size)
            if (addListingFiles is not null && addListingFiles.Count > 0)
            {
                Result validationResult = _validationService.ValidateFiles(addListingFiles);
                if (!validationResult.IsSuccessful)
                {
                    return new(Result.Failure(validationResult.ErrorMessage!, StatusCodes.Status400BadRequest));
                }

                Dictionary<string, byte[]> pictureFiles= new Dictionary<string, byte[]>();
                Dictionary<string, byte[]> videoFiles = new Dictionary<string, byte[]>();
                foreach (KeyValuePair<string, byte[]> file in addListingFiles)
                {
                    if (Path.GetExtension(file.Key) == ".mp4")
                    {
                        videoFiles.Add("Videos/"+file.Key, file.Value);

                    }
                    else if (Path.GetExtension(file.Key) == ".jpg" || Path.GetExtension(file.Key) == ".jpeg" || Path.GetExtension(file.Key) == ".png")
                    {
                        pictureFiles.Add("Pictures/" + file.Key, file.Value);
                    }
                    else
                    {
                        return new(Result.Failure("Invalid file type for " + file.Key, StatusCodes.Status400BadRequest));
                    }
                }

                await _fileService.CreateDir(dir + "Pictures/").ConfigureAwait(false);
                await _fileService.CreateDir(dir + "Videos/").ConfigureAwait(false);

                picturesStored = _fileService.GetFilesInDir(dir+"Pictures/").Result.Payload.Count();
                if (picturesStored + pictureFiles.Count > 10)
                {
                    return new(Result.Failure("Maximum of 10 pictures per listing.", StatusCodes.Status400BadRequest));
                }

                var videosStored = _fileService.GetFilesInDir(dir + "Videos/").Result.Payload.Count();
                if (videosStored + videoFiles.Count > 2)
                {
                    return new(Result.Failure("Maximum of 2 videos per listing.", StatusCodes.Status400BadRequest));
                }

                Result fileUploadResults = new Result();
                bool uploadErrorFound = false;
                foreach (KeyValuePair<string, byte[]> file in pictureFiles)
                {
                    try
                    {
                        Result uploadResult = await _fileService.UploadFile(dir, file.Key, file.Value).ConfigureAwait(false);
                        if (!uploadResult.IsSuccessful)
                        {
                            fileUploadResults.ErrorMessage += "Failed to upload " + file.Key + "\n";
                            uploadErrorFound = true;
                        }
                    }
                    catch (FluentFTP.Exceptions.FtpException ex)
                    {
                        // handle FluentFTP exceptions
                        fileUploadResults.ErrorMessage += "Failed to upload " + file.Key + Path.GetExtension(file.Key) + "\n";
                        uploadErrorFound = true;
                    }
                    catch (Exception ex)
                    {
                        // handle other exceptions
                        fileUploadResults.ErrorMessage += "Failed to upload " + file.Key + Path.GetExtension(file.Key) + "\n";
                        uploadErrorFound = true;
                    }
                }

                foreach (KeyValuePair<string, byte[]> file in videoFiles)
                {
                    try
                    {
                        Result uploadResult = await _fileService.UploadFile(dir, file.Key, file.Value).ConfigureAwait(false);
                        if (!uploadResult.IsSuccessful)
                        {
                            fileUploadResults.ErrorMessage += "Failed to upload " + file.Key + "\n";
                            uploadErrorFound = true;
                        }
                    }
                    catch (FluentFTP.Exceptions.FtpException ex)
                    {
                        // handle FluentFTP exceptions
                        fileUploadResults.ErrorMessage += "Failed to upload " + file.Key + Path.GetExtension(file.Key) + "\n";
                        uploadErrorFound = true;
                    }
                    catch (Exception ex)
                    {
                        // handle other exceptions
                        fileUploadResults.ErrorMessage += "Failed to upload " + file.Key + Path.GetExtension(file.Key) + "\n";
                        uploadErrorFound = true;
                    }
                }

                if (uploadErrorFound)
                {
                    return new(Result.Failure(fileUploadResults.ErrorMessage!, StatusCodes.Status400BadRequest));
                }
            }

            if (deleteListingNames is not null && deleteListingNames.Count > 0)
            {
                Result fileDeleteResults = new Result();
                bool deleteErrorFound = false;
                foreach (string filename in deleteListingNames)
                {
                    if (Path.GetExtension(filename) == ".png" || Path.GetExtension(filename) == ".jpeg" || Path.GetExtension(filename) == ".jpg")
                    {
                        Result deleteResult = await _fileService.DeleteFile(dir +"/Pictures/"+ filename).ConfigureAwait(false);
                        if (!deleteResult.IsSuccessful)
                        {
                            fileDeleteResults.ErrorMessage += "Failed to delete " + filename + "\n";
                            deleteErrorFound = true;
                        }
                    }

                    if (Path.GetExtension(filename) == ".mp4")
                    {
                        Result deleteResult = await _fileService.DeleteFile(dir + "/Videos/" + filename).ConfigureAwait(false);
                        if (!deleteResult.IsSuccessful)
                        {
                            fileDeleteResults.ErrorMessage += "Failed to delete " + filename + "\n";
                            deleteErrorFound = true;
                        }
                    }

                }
                if (deleteErrorFound)
                {
                    return new(Result.Failure(fileDeleteResults.ErrorMessage, StatusCodes.Status400BadRequest));
                }
            }
            picturesStored = _fileService.GetFilesInDir(dir + "Pictures/").Result.Payload.Count();
            if (picturesStored == 0)
            {
                Result unpublish = await _listingsService.UnpublishListing(listingId).ConfigureAwait(false);
                if (!unpublish.IsSuccessful)
                {
                    return new(Result.Failure("Unable to unpublish listing", StatusCodes.Status400BadRequest));
                }
            }


            return Result.Success();

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
            Result<List<ListingAvailabilityViewDTO>> getListingAvailabilityResult = await _listingsService.GetListingAvailabilities(listingId).ConfigureAwait(false);
            if (!getListingResult.IsSuccessful)
            {
                return new(Result.Failure(getListingResult.ErrorMessage!, StatusCodes.Status400BadRequest));
            }

            if (getListingAvailabilityResult.Payload is null)
            {
                return new(Result.Failure("Listing contains no availabilities.", StatusCodes.Status400BadRequest));
            }

            if (getListingAvailabilityResult.Payload is null || getListingAvailabilityResult.Payload.Count == 0)
            {
                return new(Result.Failure("Missing availabilities.", StatusCodes.Status400BadRequest));
            }

            //validate availabilities model
            foreach (ListingAvailabilityViewDTO listingAvailability in getListingAvailabilityResult.Payload)
            {
                Result validationAvailabilityResult = _validationService.ValidateModel(listingAvailability);
                if (!validationAvailabilityResult.IsSuccessful)
                {
                    return new(Result.Failure(validationAvailabilityResult.ErrorMessage!, StatusCodes.Status400BadRequest));
                }
            }

            //get files
            var filesStored = _fileService.GetFilesInDir("ListingProfiles/" + listingId + "/Pictures").Result.Payload.Count();
            if (filesStored == 0)
            {
                return new(Result.Failure("Listing contains no pictures", StatusCodes.Status400BadRequest));
            }

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

        public async Task<Result<List<string>>> GetListingFiles(int listingId)
        {
            //get listing
            Result<ListingViewDTO> getListingResult = await _listingsService.GetListing(listingId).ConfigureAwait(false);
            if (!getListingResult.IsSuccessful)
            {
                return new(Result.Failure(getListingResult.ErrorMessage!, StatusCodes.Status400BadRequest));
            }
            int ownerId = (int)getListingResult.Payload!.OwnerId;

            //check if published
            if (getListingResult.Payload!.Published == false)
            {
                return new(Result.Failure("Listing is not published.", StatusCodes.Status401Unauthorized));
            }


            Result<List<string>> getFiles = await _fileService.GetFilesInDir("ListingProfiles/" + listingId +"Pictures/").ConfigureAwait(false);
            if (!getFiles.IsSuccessful)
            {
                return new(Result.Failure(getFiles.ErrorMessage!, StatusCodes.Status400BadRequest));
            }
            return Result<List<string>>.Success(getFiles.Payload);
        }
    }
}
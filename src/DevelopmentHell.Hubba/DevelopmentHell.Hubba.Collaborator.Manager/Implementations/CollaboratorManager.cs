using System.Security.Claims;
using DevelopmentHell.Hubba.Collaborator.Manager.Abstractions;
using DevelopmentHell.Hubba.Collaborator.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.WebAPI.DTO.Collaborator;
using Microsoft.AspNetCore.Http;
using IAuthorizationService = DevelopmentHell.Hubba.Authorization.Service.Abstractions.IAuthorizationService;

namespace DevelopmentHell.Hubba.Collaborator.Manager.Implementations
{
    public class CollaboratorManager : ICollaboratorManager
    {
        private IAuthorizationService _authorizationService;
        private ILoggerService _loggerService;
        private IValidationService _validationService;
        private ICollaboratorService _collaboratorService;


        public CollaboratorManager(ICollaboratorService collaboratorService, IAuthorizationService authorizationService,
            ILoggerService loggerService, IValidationService validationService)
        {
            _authorizationService = authorizationService;
            _loggerService = loggerService;
            _validationService = validationService;
            _collaboratorService = collaboratorService;
        }

        public async Task<Result> ChangeVisibility(int collabId, bool isPublic)
        {
            if (collabId < 0)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Cannot change visibility of invalid negative collaborator id.");
                return new(Result.Failure("Profile changes failed. Invalid collaborator selected. " +
                    "Refresh page or try again later.", StatusCodes.Status412PreconditionFailed));
            }
            Result result = new Result();
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Profile changes failed. User is not logged in. " +
                    "Refresh page or try again later.", StatusCodes.Status412PreconditionFailed));
            }
            Result authorizationResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });

            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

            // try to get the ownerId
            var ownerIdResult = await _collaboratorService.GetOwnerId(collabId).ConfigureAwait(false);
            if (!ownerIdResult.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Could not find owner to change visibility.");
                return new(Result.Failure("Profile changes failed. Could not find owner to change visiblity. " +
                    "Refresh page or try again later.", StatusCodes.Status412PreconditionFailed));
            }
            // check if the user is authorized to change visibility
            if (ownerIdResult.Payload != accountIdInt)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Invalid collaborator id for visibility change.");
                return new(Result.Failure("Profile changes failed. Invalid collaborator id. Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }

            var changeVisibilityResult = await _collaboratorService.UpdatePublished(collabId, isPublic).ConfigureAwait(false);

            if (!changeVisibilityResult.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Could not update visibility status of collaborator.");
                return new(Result.Failure("Profile changes failed. Refresh page or try again later.", changeVisibilityResult.StatusCode));
            }

            return new Result()
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status202Accepted
            };

        }

        public async Task<Result> CreateCollaborator(CreateCollaboratorDTO collabDTO)
        {
            // check if user is logged in
            Result result = new Result();
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Unable to create collaborator. User is not logged in. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }
            Result authorizationResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
            if (!(authorizationResult.IsSuccessful))
            {
                return new(Result.Failure("Unable to create collaborator. User is not authorized. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }

            // validating file inputs
            if (collabDTO.UploadedFiles == null)
            {
                return new(Result.Failure("Unable to create collaborator. No files uploaded.", StatusCodes.Status412PreconditionFailed));
            }
            if (collabDTO.UploadedFiles.Length > 10)
            {
                return new(Result.Failure("Unable to create collaborator. More than 10 files uploaded.", StatusCodes.Status412PreconditionFailed));
            }
            // check if collaborator files are valid
            foreach (var file in collabDTO.UploadedFiles)
            {
                var validImageResult = _validationService.ValidateImageFile(file);
                if (!validImageResult.IsSuccessful)
                {
                    return new(Result.Failure("Unable to create collaborator. Invalid image uploaded. " + validImageResult.ErrorMessage, StatusCodes.Status412PreconditionFailed));
                }
            }
            // check if pfpfile is valid
            if (collabDTO.PfpFile != null)
            {
                var validPfpImageResult = _validationService.ValidateImageFile(collabDTO.PfpFile);
                if (!validPfpImageResult.IsSuccessful)
                {
                    return new(Result.Failure("Unable to create collaborator. Invalid image uploaded. " + validPfpImageResult.ErrorMessage, StatusCodes.Status412PreconditionFailed));
                }
            }

            // casting DTO to objects
            CollaboratorProfile collab = new CollaboratorProfile()
            {
                Name = collabDTO.Name,
                ContactInfo = collabDTO.ContactInfo,
                Tags = collabDTO.Tags,
                Description = collabDTO.Description,
                Availability = collabDTO.Availability,
                Votes = 0,
                Published = collabDTO.Published
            };
            IFormFile[] collabFiles = collabDTO.UploadedFiles;
            IFormFile? pfpFile = collabDTO.PfpFile;

            Result collabValidation = _validationService.ValidateCollaboratorAllowEmptyFiles(collab);

            if (!(collabValidation.IsSuccessful))
            {
                return new(Result.Failure("Unable to create collaborator. Invalid collaborator profile. " + collabValidation.ErrorMessage, StatusCodes.Status412PreconditionFailed));
            }

            // will work regardless of if pfpFile is null or not
            Result createCollabResult = await _collaboratorService.CreateCollaborator(collab, collabFiles, pfpFile).ConfigureAwait(false);

            if (!createCollabResult.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Unable to create collaborator.");
                return new(Result.Failure("Unable to create collaborator. Refresh page or try again later.", StatusCodes.Status500InternalServerError));
            }

            return new Result()
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status201Created
            };

        }

        public async Task<Result> DeleteCollaborator(int collabId)
        {
            if (collabId < 0)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Cannot delete negative collaborator id.");
                return new(Result.Failure("Unable to delete collaborator. Invalid collaborator selected.", StatusCodes.Status412PreconditionFailed));
            }
            // check if user is logged in
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Unable to delete collaborator. User is not logged in. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }

            // get ownerid of collaborator
            var getCollabAccountId = await _collaboratorService.GetOwnerId(collabId).ConfigureAwait(false);

            if (!getCollabAccountId.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Could not find owner id of collaborator.");
                return new(Result.Failure("Unable to delete collaborator. Could not find owner of collaborator profile. " +
                    "Refresh page or try again later.", StatusCodes.Status412PreconditionFailed));
            }
            int ownerId = (int)getCollabAccountId.Payload;

            // Get the ID of current thread
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

            // check if the user is authorized to delete
            Result authorizationResult = _authorizationService.Authorize(new string[] { "AdminUser" });
            if (!(authorizationResult.IsSuccessful) && ownerId != accountIdInt)
            {
                return new(Result.Failure("Unable to delete collaborator. User is not authorized. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }

            var deletionResult = await _collaboratorService.DeleteCollaborator(collabId).ConfigureAwait(false);
            if (!(deletionResult.IsSuccessful))
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Unable to delete collaborator.");
                return new(Result.Failure("Unable to delete collaborator. " +
                    "Refresh page or try again later.", StatusCodes.Status500InternalServerError));
            }

            return new Result()
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status202Accepted
            };
        }

        public async Task<Result> RemoveCollaborator(int collabId)
        {
            if (collabId < 0)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Unable to remove invalid negative collaborator id.");
                return new(Result.Failure("Profile removal failed. Invalid collaborator profile. " +
                    "Refresh page or try again later.", StatusCodes.Status412PreconditionFailed));
            }
            // check if user is logged in
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Profile removal failed. User is not logged in. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }

            // Get the ID of current thread
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

            var getCollabAccountId = await _collaboratorService.GetOwnerId(collabId).ConfigureAwait(false);
            if (!getCollabAccountId.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Unable to find owner id of collaborator for removal.");
                return new(Result.Failure("Profile removal failed. Unable to find associated collaborator profile. " +
                    "Refresh page or try again later.", StatusCodes.Status404NotFound));
            }
            int ownerId = getCollabAccountId.Payload;

            // check if the user is authorized to delete
            Result authorizationResult = _authorizationService.Authorize(new string[] { "AdminUser" });
            if (!(authorizationResult.IsSuccessful) && ownerId != accountIdInt)
            {
                return new(Result.Failure("Profile removal failed. User is not authorized for this request. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));

            }

            var removeResult = await _collaboratorService.RemoveCollaborator(collabId).ConfigureAwait(false);
            if (!removeResult.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Unable to remove collaborator profile.");
                return new(Result.Failure("Profile removal failed. " +
                    "Refresh page or try again later.", StatusCodes.Status500InternalServerError));
            }
            return new Result()
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status202Accepted
            };
        }

        public async Task<Result> RemoveOwnCollaborator()
        {
            // check if user is logged in
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Profile removal failed. User is not logged in. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }
            // Get the ID of current thread
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

            var getCollaboratorId = await _collaboratorService.GetCollaboratorId(accountIdInt).ConfigureAwait(false);
            if (!getCollaboratorId.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Unable to find collaborator for removal.");
                return new(Result.Failure("Profile removal failed. Unable to collaborator profile. " +
                    "Refresh page or try again later.", StatusCodes.Status412PreconditionFailed));
            }
            if (getCollaboratorId.Payload == null)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Account has no collaborator to remove.");
                return new(Result.Failure("Profile removal failed. Account has no collaborator profile. " +
                    "Refresh page or try again later.", StatusCodes.Status404NotFound));
            }

            int collabId = (int)getCollaboratorId.Payload;
            var removeResult = await _collaboratorService.RemoveCollaborator(collabId).ConfigureAwait(false);

            if (!removeResult.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Unable to remove collaborator profile.");
                return new(Result.Failure("Profile removal failed. " +
                    "Refresh page or try again later.", StatusCodes.Status500InternalServerError));
            }
            return new Result()
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status202Accepted
            };
        }

        public async Task<Result> DeleteCollaboratorWithAccountId(int accountId)
        {
            // check if user is logged in
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Unable to delete collaborator. User is not logged in. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }

            // Get the ID of current thread
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

            // check if the user is authorized to delete
            Result authorizationResult = _authorizationService.Authorize(new string[] { "AdminUser" });
            if (!(authorizationResult.IsSuccessful) && accountId != accountIdInt)
            {
                return new(Result.Failure("Unable to delete collaborator. User is not authorized. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }

            var deletionResult = await _collaboratorService.DeleteCollaboratorWithAccountId(accountId).ConfigureAwait(false);
            if (!(deletionResult.IsSuccessful))
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Unable to delete collaborator.");
                return new(Result.Failure("Unable to delete collaborator. " +
                    "Refresh page or try again later.", StatusCodes.Status500InternalServerError));
            }

            return new Result()
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status202Accepted
            };
        }

        public async Task<Result> EditCollaborator(EditCollaboratorDTO collabDTO)
        {
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Profile edits failed. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }

            Result authorizationResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
            if (!(authorizationResult.IsSuccessful))
            {
                return new(Result.Failure("Profile edits failed. User is not authorized. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }
            // validating file inputs
            if (collabDTO.UploadedFiles != null)
            {
                foreach (var file in collabDTO.UploadedFiles)
                {
                    var validImageResult = _validationService.ValidateImageFile(file);
                    if (!validImageResult.IsSuccessful)
                    {
                        return new(Result.Failure("Profile edits failed. " + validImageResult.ErrorMessage +
                            " Refresh page or try again later.", StatusCodes.Status412PreconditionFailed));
                    }
                }
            }
            if (collabDTO.PfpFile != null)
            {
                var validPfpImageResult = _validationService.ValidateImageFile(collabDTO.PfpFile);
                if (!validPfpImageResult.IsSuccessful)
                {
                    return new(Result.Failure("Profile edits failed. " + validPfpImageResult.ErrorMessage +
                        "Refresh page or try again later.", StatusCodes.Status412PreconditionFailed));
                }
            }

            // casting DTO to objects
            CollaboratorProfile collab = new CollaboratorProfile()
            {
                Name = collabDTO.Name,
                ContactInfo = collabDTO.ContactInfo,
                Tags = collabDTO.Tags,
                Description = collabDTO.Description,
                Availability = collabDTO.Availability,
                Votes = 0,
                Published = collabDTO.Published
            };

            IFormFile[]? collabFiles = collabDTO.UploadedFiles;
            IFormFile? pfpFile = collabDTO.PfpFile;
            string[]? removedFiles = collabDTO.RemovedFiles;

            // check amount of new uploaded files
            int uploadedCount = 0;
            int removedCount = 0;
            if (collabFiles != null)
                uploadedCount = collabFiles.Length;
            if (removedFiles != null)
                removedCount = removedFiles.Length;

            // get the owner ID to count the amount of files in database
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);
            var countFilesResult = await _collaboratorService.CountFilesWithoutPfp(accountIdInt).ConfigureAwait(false);
            if (!countFilesResult.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Could not determine count of collaborator files in database.");
                return new(Result.Failure("Profile edits failed. Image file is not valid. Unable to determine number of uploaded files currently stored in database. " +
                    "Refresh page or try again later.", StatusCodes.Status412PreconditionFailed));
            }

            // check if they are removing the pfp since it is optional to have and doesn't count towards total
            var pfpurl = await _collaboratorService.GetPfpUrl(accountIdInt).ConfigureAwait(false);
            if (!pfpurl.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Could not get profile picture url of collaborator.");
                return new(Result.Failure("Profile edits failed. Unable to check if profile picture is being removed. Refresh page or try again later."));
            }
            if (pfpurl.Payload != null)
            {
                if (removedFiles != null && removedFiles.Contains<string>(pfpurl.Payload))
                {
                    removedCount -= 1;
                }
            }

            // storing way more than the stored file limit
            if (uploadedCount > 10)
            {
                return new(Result.Failure("A maximum of 10 files can be uploaded to the server, including those already stored." +
                    " The Profile Picture does not count towards the 10 file maximum. Please remove another file instead.", StatusCodes.Status412PreconditionFailed));
            }
            // storing way more than the stored file limit
            if (uploadedCount + countFilesResult.Payload - removedCount > 10)
            {
                return new(Result.Failure("A maximum of 10 files can be uploaded to the server, including those already stored." +
                    " The Profile Picture does not count towards the 10 file maximum. Please remove another file instead.", StatusCodes.Status412PreconditionFailed));
            }
            // borderline case, should check if a profile picture is being removed since it
            // doesn't count towards total number of files stored
            if (uploadedCount + countFilesResult.Payload - removedCount <= 0)
            {
                return new(Result.Failure("A minimum of 1 file needs to be stored on the server to display a collaborator profile, " +
                    "excluding the profile picture.", StatusCodes.Status412PreconditionFailed));

            }

            var validateCollab = _validationService.ValidateCollaboratorAllowEmptyFiles(collab);

            if (!validateCollab.IsSuccessful)
            {
                return new(Result.Failure("New collaborator updates not valid. " + validateCollab.ErrorMessage, StatusCodes.Status412PreconditionFailed));
            }

            var updateCollabResult = await _collaboratorService.EditCollaborator(collab, collabFiles, removedFiles, pfpFile).ConfigureAwait(false);

            if (!updateCollabResult.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Unable to edit collaborator profile.");
                return new(Result.Failure("Profile edits failed. " +
                    "Refresh page or try again later.", StatusCodes.Status500InternalServerError));
            }

            return new Result()
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status202Accepted
            };
        }

        public async Task<Result<CollaboratorProfile>> GetCollaborator(int collabId)
        {
            if (collabId < 0)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Unable to get invalid negative collaborator id");
                return new(Result.Failure("Profile failed to load. Invalid collaborator profile. " +
                    "Refresh page or try again later.", StatusCodes.Status412PreconditionFailed));
            }
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Profile failed to load. User is not logged in. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }

            Result authorizationResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
            if (!(authorizationResult.IsSuccessful))
            {
                return new(Result.Failure("Profile failed to load. User is not authorized for this request. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }

            Result<CollaboratorProfile> getCollabResult = await _collaboratorService.GetCollaborator(collabId).ConfigureAwait(false);

            if (!getCollabResult.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Failed to retrieve collaborator profile.");
                return new(Result.Failure("Profile failed to load. " +
                    "Refresh page or try again later.", StatusCodes.Status500InternalServerError));
            }
            if (getCollabResult.Payload == null)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Cannot find collaborator.");
                return new(Result.Failure("Profile failed to load. Could not find collaborator profile. " +
                    "Refresh page or try again later.", StatusCodes.Status404NotFound));
            }

            // check if the profile is publicly visible
            if (!getCollabResult.Payload!.Published)
            {
                // check if this is the owner
                var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
                string accountIDStr = principal.FindFirstValue("sub");
                int.TryParse(accountIDStr, out int accountIdInt);
                var ownerIdResult = await _collaboratorService.GetOwnerId(collabId).ConfigureAwait(false);
                if (!ownerIdResult.IsSuccessful)
                {
                    _loggerService.Log(Models.LogLevel.INFO, Category.BUSINESS, "Hidden collaborator cannot be viewed and owner cannot be found.");
                    return new(Result.Failure("Profile failed to load. Hidden from public view. " +
                        "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
                }
                else if (ownerIdResult.Payload != accountIdInt)
                {
                    return new(Result.Failure("Profile failed to load. Hidden from public view. " +
                        "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
                }
            }

            getCollabResult.StatusCode = StatusCodes.Status200OK;
            return getCollabResult;
        }

        public async Task<Result<int>> GetCollaboratorId(int accountId)
        {
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Profile failed to load. User is not logged in. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }

            Result authorizationResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
            if (!(authorizationResult.IsSuccessful))
            {
                return new(Result.Failure("Profile failed to load. User is not authorized. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }

            var getCollabIdResult = await _collaboratorService.GetCollaboratorId(accountId).ConfigureAwait(false);
            if (!getCollabIdResult.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Failed to retrieve collaborator profile.");
                return new(Result.Failure("Profile failed to load. " +
                    "Refresh page or try again later.", StatusCodes.Status500InternalServerError));
            }
            if (getCollabIdResult.Payload == null)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Cannot find collaborator.");
                return new(Result.Failure("Profile failed to load. Could not find collaborator profile. " +
                    "Refresh page or try again later.", StatusCodes.Status404NotFound));
            }

            return new Result<int>
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Payload = (int)getCollabIdResult.Payload
            };
        }

        public async Task<Result<bool>> HasCollaborator(int? accountId = null)
        {
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Unable to check collaborator profile of user. User is not logged in. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }

            Result authorizationResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
            if (!(authorizationResult.IsSuccessful))
            {
                return new(Result.Failure("Unable to check collaborator profile of user. User is not authorized. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }
            if (accountId == null)
            {
                var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
                string accountIDStr = principal.FindFirstValue("sub");
                int.TryParse(accountIDStr, out int accountIdInt);
                accountId = accountIdInt;
            }

            var hasCollaboratorResult = await _collaboratorService.HasCollaborator((int)accountId).ConfigureAwait(false);
            if (!hasCollaboratorResult.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Error checking collaborator of user.");
                return new(Result.Failure("Unable to check collaborator profile of user. " +
                    "Refresh page or try again later.", StatusCodes.Status500InternalServerError));
            }
            hasCollaboratorResult.StatusCode = StatusCodes.Status200OK;
            return hasCollaboratorResult;
        }

        public async Task<Result> Vote(int collabId, bool upvote)
        {
            if (collabId < 0)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Cannot vote on invalid negative collaborator id.");
                return new(Result.Failure("Vote error. Invalid collaborator profile page. " +
                    "Refresh page or try again later.", StatusCodes.Status412PreconditionFailed));
            }
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Vote error. User is not logged in. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));
            }

            Result authorizationResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
            if (!(authorizationResult.IsSuccessful))
            {
                return new(Result.Failure("Vote error. User is not authorized. " +
                    "Refresh page or try again later.", StatusCodes.Status401Unauthorized));

            }

            var voteResult = await _collaboratorService.Vote(collabId, upvote).ConfigureAwait(false);
            if (!(voteResult.IsSuccessful))
            {
                return new(Result.Failure("Vote error. " + voteResult.ErrorMessage +
                    " Refresh page or try again later.", StatusCodes.Status500InternalServerError));
            }
            return new Result()
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status202Accepted
            };

        }

        public async Task<Result<string[]>> GetFileUrls(int collabId)
        {
            if (collabId < 0)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Invalid negative collaborator id for get file urls.");
                return new(Result.Failure("Unable to get collaborator files. " +
                    "Refresh page or try again later.", StatusCodes.Status412PreconditionFailed));
            }
            var getFileUrlsResult = await _collaboratorService.GetFileUrls(collabId).ConfigureAwait(false);
            if (!getFileUrlsResult.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, "Unable to retrieve file urls from provided collaborator.");
                return new(Result.Failure("Unable to get collaborator files. " +
                    "Refresh page or try again later.", StatusCodes.Status500InternalServerError));

            }
            return new Result<string[]>()
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Payload = getFileUrlsResult.Payload
            };
        }
    }
}

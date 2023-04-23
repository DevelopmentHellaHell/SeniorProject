using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Collaborator.Manager.Abstractions;
using DevelopmentHell.Hubba.Collaborator.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.WebAPI.DTO.Collaborator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System.Security.Claims;
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
                return new(Result.Failure("Invalid collaborator id.", StatusCodes.Status412PreconditionFailed));
            }
            Result result = new Result();
            if (Thread.CurrentPrincipal is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, user is not logged in.";
                return result;
            }
            Result authorizationResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });

            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

            // try to get the ownerId
            var ownerIdResult = await _collaboratorService.GetOwnerId(collabId).ConfigureAwait(false);
            if (!ownerIdResult.IsSuccessful)
            {
                return new(Result.Failure("Could not find owner to change visibility. " + ownerIdResult.ErrorMessage));
            }
            // check if the user is authorized to change visibility
            if (ownerIdResult.Payload != accountIdInt)
            {
                return new(Result.Failure("Unauthorized to change visibility.", StatusCodes.Status401Unauthorized));
            }

            var changeVisibilityResult = await _collaboratorService.UpdatePublished(collabId, isPublic).ConfigureAwait(false);

            if (!changeVisibilityResult.IsSuccessful)
            {
                return new(Result.Failure("Could not update public viewing status."));
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
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, user is not logged in.";
                return result;
            }
            Result authorizationResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
            if (!(authorizationResult.IsSuccessful))
            {
                return authorizationResult;
            }

            // validating file inputs
            if (collabDTO.UploadedFiles == null)
            {
                return new(Result.Failure("No files uploaded.", StatusCodes.Status412PreconditionFailed));
            }
            if(collabDTO.UploadedFiles.Length > 10)
            {
                return new(Result.Failure("More than 10 files uploaded.", StatusCodes.Status412PreconditionFailed));
            }
            // check if collaborator files are valid
            foreach(var file in collabDTO.UploadedFiles)
            {
                var validImageResult = _validationService.ValidateImageFile(file);
                if (!validImageResult.IsSuccessful)
                {
                    return validImageResult;
                }
            }
            // check if pfpfile is valid
            if (collabDTO.PfpFile != null)
            {
                var validPfpImageResult = _validationService.ValidateImageFile(collabDTO.PfpFile);
                if (!validPfpImageResult.IsSuccessful)
                {
                    return validPfpImageResult;
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
                return collabValidation;
            }

            // will work regardless of if pfpFile is null or not
            Result createCollabResult = await _collaboratorService.CreateCollaborator(collab, collabFiles, pfpFile).ConfigureAwait(false);

            if (!createCollabResult.IsSuccessful)
            {
                result.ErrorMessage = "Unable to create collaborator. " + createCollabResult.ErrorMessage;
                return result;
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
                return new(Result.Failure("Invalid collaborator id.", StatusCodes.Status412PreconditionFailed));
            }
            // check if user is logged in
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Error, user is not logged in", StatusCodes.Status401Unauthorized));
            }

            // get ownerid of collaborator
            var getCollabAccountId = await _collaboratorService.GetOwnerId(collabId).ConfigureAwait(false);

            if(!getCollabAccountId.IsSuccessful)
            {
                return new(Result.Failure("Could not find owner in Collaborators for deletion.",
                    StatusCodes.Status412PreconditionFailed));
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
                return new(Result.Failure("Error, user is not authorized for this request.", StatusCodes.Status401Unauthorized));
            }

            var deletionResult = await _collaboratorService.DeleteCollaborator(collabId).ConfigureAwait(false);
            if(!(deletionResult.IsSuccessful))
            {
                return new(Result.Failure($"Unable to delete specified account." + deletionResult.ErrorMessage));
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
                return new(Result.Failure("Invalid collaborator id.", StatusCodes.Status412PreconditionFailed));
            }
            // check if user is logged in
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Error, user is not logged in", StatusCodes.Status401Unauthorized));
            }

            // Get the ID of current thread
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

            var getCollabAccountId = await _collaboratorService.GetOwnerId(collabId).ConfigureAwait(false);
            if(!getCollabAccountId.IsSuccessful)
            {
                return new(Result.Failure("Unable to find associated user with collaborator provided. "
                    + getCollabAccountId.ErrorMessage, StatusCodes.Status404NotFound));
            }
            int ownerId = getCollabAccountId.Payload;
            
            // check if the user is authorized to delete
            Result authorizationResult = _authorizationService.Authorize(new string[] { "AdminUser" });
            if (!(authorizationResult.IsSuccessful) && ownerId != accountIdInt)
            {
                return new(Result.Failure("Error, user is not authorized for this request.", StatusCodes.Status401Unauthorized));
            }

            var removeResult = await _collaboratorService.RemoveCollaborator(collabId).ConfigureAwait(false);
            if (!removeResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to remove collaborator. " + removeResult.ErrorMessage, removeResult.StatusCode));
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
                return new(Result.Failure("Error, user is not logged in", StatusCodes.Status401Unauthorized));
            }

            // Get the ID of current thread
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

            // check if the user is authorized to delete
            Result authorizationResult = _authorizationService.Authorize(new string[] { "AdminUser" });
            if (!(authorizationResult.IsSuccessful) && accountId != accountIdInt)
            {
                return new(Result.Failure("Error, user is not authorized for this request.", StatusCodes.Status401Unauthorized));
            }

            var deletionResult = await _collaboratorService.DeleteCollaboratorWithAccountId(accountId).ConfigureAwait(false);
            if (!(deletionResult.IsSuccessful))
            {
                return new(Result.Failure($"Unable to delete specified account." + deletionResult.ErrorMessage));
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
                return new(Result.Failure("Error, user is not logged in", StatusCodes.Status401Unauthorized));
            }

            Result authorizationResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
            if (!(authorizationResult.IsSuccessful))
            {
                return new(Result.Failure("Error, user is not authorized for this request.", StatusCodes.Status401Unauthorized));
            }
            // validating file inputs
            if (collabDTO.UploadedFiles != null)
            {
                foreach (var file in collabDTO.UploadedFiles)
                {
                    var validImageResult = _validationService.ValidateImageFile(file);
                    if (!validImageResult.IsSuccessful)
                    {
                        return validImageResult;
                    }
                }
            }
            if (collabDTO.PfpFile != null)
            {
                var validPfpImageResult = _validationService.ValidateImageFile(collabDTO.PfpFile);
                if (!validPfpImageResult.IsSuccessful)
                {
                    return validPfpImageResult;
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
                return new(Result.Failure("Unable to determine number of uploaded files currently stored in database. " + countFilesResult.ErrorMessage));
            }

            if (uploadedCount - removedCount +  countFilesResult.Payload > 10)
            {
                return new(Result.Failure("A maximum of 10 files can be uploaded to the server, including those already stored. ",StatusCodes.Status412PreconditionFailed));
            }

            var validateCollab = _validationService.ValidateCollaboratorAllowEmptyFiles(collab);

            if (!validateCollab.IsSuccessful)
            {
                return new(Result.Failure("New collaborator updates not valid. ", StatusCodes.Status412PreconditionFailed));
            }

            var updateCollabResult = await _collaboratorService.EditCollaborator(collab, collabFiles, removedFiles, pfpFile).ConfigureAwait(false);

            if (!updateCollabResult.IsSuccessful)
            {
                return Result.Failure(updateCollabResult.ErrorMessage!);
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
                return new(Result.Failure("Invalid collaborator id.", StatusCodes.Status412PreconditionFailed));
            }
            if (Thread.CurrentPrincipal is null)
            {
                return new (Result.Failure("Error, user is not logged in", StatusCodes.Status401Unauthorized));
            }

            Result authorizationResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
            if (!(authorizationResult.IsSuccessful))
            {
                return new(Result.Failure("Error, user is not authorized for this page.", StatusCodes.Status401Unauthorized));
            }

            Result<CollaboratorProfile> getCollabResult = await _collaboratorService.GetCollaborator(collabId).ConfigureAwait(false);

            if (!getCollabResult.IsSuccessful)
            {
                return new(Result.Failure(getCollabResult.ErrorMessage!, StatusCodes.Status500InternalServerError));
            }
            if(getCollabResult.Payload == null)
            {
                return new(Result.Failure("Collaborator not found." + getCollabResult.ErrorMessage, StatusCodes.Status404NotFound));
            }

            // check if the profile is publicly visible
            if(!getCollabResult.Payload!.Published)
            {
                // check if this is the owner
                var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
                string accountIDStr = principal.FindFirstValue("sub");
                int.TryParse(accountIDStr, out int accountIdInt);
                var ownerIdResult = await _collaboratorService.GetOwnerId(collabId).ConfigureAwait(false);
                if (!ownerIdResult.IsSuccessful)
                {
                    return new(Result.Failure("Unable to validate owner and collaborator is hidden " +
                        "from public view.", StatusCodes.Status401Unauthorized));
                }
                else if (ownerIdResult.Payload != accountIdInt)
                {
                    return new(Result.Failure("Collaborator is hidden from public view.", StatusCodes.Status401Unauthorized));
                }
            }

            getCollabResult.StatusCode = StatusCodes.Status200OK;
            return getCollabResult;
        }

        public async Task<Result<bool>> HasCollaborator(int? accountId = null)
        {
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Error, user is not logged in", StatusCodes.Status401Unauthorized));
            }

            Result authorizationResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
            if (!(authorizationResult.IsSuccessful))
            {
                return new(Result.Failure("Error, user is not authorized for this request.", StatusCodes.Status401Unauthorized));
            }
            if (accountId == null)
            {
                var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
                string accountIDStr = principal.FindFirstValue("sub");
                int.TryParse(accountIDStr, out int accountIdInt);
                accountId = accountIdInt;
            }

            var hasCollaboratorResult = await _collaboratorService.HasCollaborator((int)accountId).ConfigureAwait(false);
            if(hasCollaboratorResult.IsSuccessful)
                hasCollaboratorResult.StatusCode = StatusCodes.Status200OK;
            return hasCollaboratorResult;
        }

        public async Task<Result> Vote(int collabId, bool upvote)
        {
            if (collabId < 0)
            {
                return new(Result.Failure("Invalid collaborator id.", StatusCodes.Status412PreconditionFailed));
            }
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Error, user is not logged in", StatusCodes.Status401Unauthorized));
            }

            Result authorizationResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
            if (!(authorizationResult.IsSuccessful))
            {
                return new(Result.Failure("Error, user is not authorized for this request.", StatusCodes.Status401Unauthorized));
            }

            var voteResult = await _collaboratorService.Vote(collabId, upvote).ConfigureAwait(false);
            if (!(voteResult.IsSuccessful))
            {
                return new(Result.Failure("Error, user is not authorized for this request.", StatusCodes.Status401Unauthorized));
            }
            return new Result() { 
                IsSuccessful= true,
                StatusCode = StatusCodes.Status202Accepted
            };

        }

        public async Task<Result<string[]>> GetFileUrls(int collabId)
        {
            if (collabId < 0)
            {
                return new(Result.Failure("Invalid collaborator id.", StatusCodes.Status412PreconditionFailed));
            }
            var getFileUrlsResult = await _collaboratorService.GetFileUrls(collabId).ConfigureAwait(false);
            if (!getFileUrlsResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to retrieve file urls from provided collaborator. " + getFileUrlsResult.ErrorMessage, getFileUrlsResult.StatusCode));
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

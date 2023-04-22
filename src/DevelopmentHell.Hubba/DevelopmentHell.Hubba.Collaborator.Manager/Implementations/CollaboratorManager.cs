using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Collaborator.Manager.Abstractions;
using DevelopmentHell.Hubba.Collaborator.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
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

            return changeVisibilityResult;

        }

        public async Task<Result> CreateCollaborator(CollaboratorProfile collab, IFormFile[] collabFiles, IFormFile? pfpFile = null)
        {
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

            Result collabValidation = _validationService.ValidateCollaboratorAllowEmptyFiles(collab);

            if (!(collabValidation.IsSuccessful))
            {
                return collabValidation;
            }

            if (collabFiles == null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "No files uploaded, unable to create collaborator profile.";
                return result;
            }

            Result createCollabResult;
            if (pfpFile != null)
                createCollabResult = await _collaboratorService.CreateCollaborator(collab, collabFiles, pfpFile).ConfigureAwait(false);
            else
                createCollabResult = await _collaboratorService.CreateCollaborator(collab, collabFiles).ConfigureAwait(false);

            if (!createCollabResult.IsSuccessful)
            {
                result.ErrorMessage = "Unable to create collaborator. " + createCollabResult.ErrorMessage;

                // remove uploaded files 
                //if (pfpFile != null)
                //{
                    //Result deletePfp = await CollaboratorFileUploadManager.Delete(pfpString).ConfigureAwait(false);
                    //if(!deletePfp.IsSuccessful)
                    //{
                        //Result logRes = _loggerService.Log(Models.LogLevel.ERROR, Category.SERVER, $"Unable to remove uploaded collab" +
                        //    $" profile picture files. Account id: {accountIdInt}", null);
                    //}
                //}
                //foreach (var file in collabString)
                //{
                    //var deleteFile = await CollaboratorFileUploadManager.Delete(file).ConfigureAwait(false);
                    //if(deleteFile == null || !deleteFile.IsSuccessful)
                    //{
                        //Result logRes = _loggerService.Log(Models.LogLevel.ERROR, Category.SERVER, $"Unable to remove uploaded collab" +
                        //    $" files. Account id: {accountIdInt}", null);
                    //}
                //}
                return result;
            }

            result.IsSuccessful = true;
            return result;

        }

        public async Task<Result> DeleteCollaborator(int collabId)
        {
            // check if user is logged in
            if (Thread.CurrentPrincipal is null)
            {
                return new(Result.Failure("Error, user is not logged in", StatusCodes.Status401Unauthorized));
            }

            // get ownerid of collaborator
            var getCollabAccountId = await _collaboratorService.GetOwnerId(collabId).ConfigureAwait(false);

            if(!getCollabAccountId.IsSuccessful)
            {
                return new(Result.Failure("Could not get owner Id to authorize deletion" + getCollabAccountId.ErrorMessage,
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
            };
        }

        public async Task<Result> RemoveCollaborator(int collabId)
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
            };
        }

        public async Task<Result> EditCollaborator(CollaboratorProfile collab, IFormFile[]? collabFiles = null, 
            string[]? removedFiles = null, IFormFile? pfpFile = null)
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

            var validateCollab = _validationService.ValidateCollaboratorAllowEmptyFiles(collab);

            if (validateCollab == null || !validateCollab.IsSuccessful)
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
            };
        }

        public async Task<Result<CollaboratorProfile>> GetCollaborator(int collabId)
        {
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

            // validate collaborator object
            var validationResult = _validationService.ValidateCollaborator(getCollabResult.Payload!);
            if (!validationResult.IsSuccessful)
            {
                return new(Result.Failure("Collaborator found not properly formatted." + validationResult.ErrorMessage,
                    StatusCodes.Status500InternalServerError));
            }

            // check if the profile is publicly visible
            if(!getCollabResult.Payload!.Published)
            {
                return new(Result.Failure("Collaborator is hidden from public view.", StatusCodes.Status401Unauthorized));
            }

            return getCollabResult;
        }

        public async Task<Result<bool>> HasCollaborator(int accountId)
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

            var hasCollaboratorResult = await _collaboratorService.HasCollaborator(accountId).ConfigureAwait(false);
            return hasCollaboratorResult;
        }

        public async Task<Result> Vote(int collabId, bool upvote)
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

            var voteResult = await _collaboratorService.Vote(collabId, upvote).ConfigureAwait(false);
            if (!(voteResult.IsSuccessful))
            {
                return new(Result.Failure("Error, user is not authorized for this request.", StatusCodes.Status401Unauthorized));
            }
            return new Result() { IsSuccessful= true};

        }
    }
}

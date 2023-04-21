using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Collaborator.Manager.Abstractions;
using DevelopmentHell.Hubba.Collaborator.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public async Task<Result> RemoveCollaborator(int collabId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> DeleteCollaboratorWithAccountId(int accountId)
        {
            throw new NotImplementedException();
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

            if (validateCollab == null || validateCollab.IsSuccessful)
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


    }
}

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


        public CollaboratorManager(ICollaboratorService collaboratorService, IAuthorizationService authorizationService, ILoggerService loggerService, IValidationService validationService)
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

        public async Task<Result> CreateCollaborator(CollaboratorProfile collab, IFormFile[] collabFiles, IFormFile pfpFile)
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

            if (pfpFile != null)
            {
                // TODO: Complete upload file manager
                //Result uploadPfp = await CollaboratorFileUploadManager.Upload(pfpFile).ConfigureAwait(false);
                //if(!uploadPfp.IsSuccessful)
                //{
                //    result.IsSuccessful = false;
                //    result.ErrorMessage = "Profile picture upload failed.";
                //    return result;
                //}
                //collab.PfpUrl = uploadPfp.Payload;
            }

            foreach (var file in collabFiles)
            {
                // TODO: Complete upload file manager
                //var uploadFile = await CollaboratorFileUploadManager.Upload(file).ConfigureAwait(false);
                //if(uploadFile == null || !uploadFile.IsSuccessful)
                //{
                //    result.IsSuccessful = false;
                //    result.ErrorMessage = "Profile picture upload failed.";
                //    return result;
                //}
                //collab.CollabUrls.append(uploadFile.Payload);
            }

            var createCollabResult = await _collaboratorService.CreateCollaborator(collab).ConfigureAwait(false);

            if (!createCollabResult.IsSuccessful)
            {
                result.ErrorMessage = "Unable to create collaborator";
                // remove uploaded files 
                //if (pfpFile != null)
                //{
                //Result deletePfp = await CollaboratorFileUploadManager.Delete(pfpString).ConfigureAwait(false);
                //if(!deletePfp.IsSuccessful)
                //{
                //    result.IsSuccessful = false;
                //    result.ErrorMessage = "Profile picture upload failed.";
                //    return result;
                //}
                //collab.PfpUrl = deletePfp.Payload;
                //}
                //foreach (var file in collabString)
                //{
                //var deleteFile = await CollaboratorFileUploadManager.Delete(file).ConfigureAwait(false);
                //if(deleteFile == null || !deleteFile.IsSuccessful)
                //{
                //    Result logRes = _loggerService.Log(Models.LogLevel.ERROR, Category.SERVER, $"Unable to remove uploaded files. Collab Id: {collab.}", null);
                //}
                //collab.CollabUrls.append(deleteFile.Payload);
                //}
                return result;
            }
            throw new NotImplementedException();

        }

        public async Task<Result> DeleteCollaborator(int collabId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> DeleteCollaboratorWithAccountId(int accountId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> EditCollaborator(CollaboratorProfile collab, IFormFile[] collabFiles, IFormFile pfpFile, string[] removedFiles)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<CollaboratorProfile>> GetCollaborator(int collabId)
        {
            throw new NotImplementedException();
        }
    }
}

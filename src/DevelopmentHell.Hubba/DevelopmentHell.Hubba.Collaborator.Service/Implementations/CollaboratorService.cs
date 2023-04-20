using DevelopmentHell.Hubba.Collaborator.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.Hubba.Collaborator.Service.Implementations
{
    public class CollaboratorService : ICollaboratorService
    {
        private ICollaboratorDataAccess _collaboratorDataAccess;
        private ICollaboratorFileDataAccess _collaboratorFileDataAccess;
        private ICollaboratorFileJunctionDataAccess _collaboratorFileJunctionDataAccess;
        private IValidationService _validationService;

        public CollaboratorService(ICollaboratorDataAccess collaboratorDataAccess, CollaboratorFileDataAccess collaboratorFileDataAccess,
            CollaboratorFileJunctionDataAccess collaboratorFileJunctionDataAccess, IValidationService validationService)
        {
            _collaboratorDataAccess = collaboratorDataAccess;
            _collaboratorFileDataAccess = collaboratorFileDataAccess;
            _collaboratorFileJunctionDataAccess = collaboratorFileJunctionDataAccess;
            _validationService = validationService;
        }

        public async Task<Result> CreateCollaborator(CollaboratorProfile collab, IFormFile[] collabFiles, IFormFile? pfpFile = null)
        {
            Result result = new Result();
            result.IsSuccessful = false;
            if (collab == null)
            {
                return Result.Failure("Error, collaborator object is null.",
                    (int)StatusCodes.Status412PreconditionFailed);
            }
            var validateCollab = _validationService.ValidateCollaborator(collab);
            if (validateCollab == null)
            {
                return Result.Failure("Unable to validate collaborator with files.",
                    (int)StatusCodes.Status412PreconditionFailed);
            }
            if (!validateCollab.IsSuccessful)
            {
                return validateCollab;
            }
            if (collabFiles == null)
            {
                return Result.Failure("Collaborator files are null. Unable to upload collaborator files to junction table database.",
                    (int)StatusCodes.Status412PreconditionFailed);
            }
            if (collabFiles.Length != collab.CollabUrls!.Count)
            {
                return Result.Failure(string.Format("Length of collaborator object ({0}) not equal expected amount of collaborator files ({1}).",
                    collabFiles.Length, collab.CollabUrls!.Count), 
                    (int)StatusCodes.Status412PreconditionFailed);
            }


            // create new collaborator entry into collaborators table
            var createCollabResult = await _collaboratorDataAccess.CreateCollaborator(collab).ConfigureAwait(false);
            if (!createCollabResult.IsSuccessful)
            {
                result.ErrorMessage = createCollabResult.ErrorMessage;
                return result;
            }

            // add profile picture if applicable and update collaborator
            if(collab.PfpUrl != null && pfpFile != null) { 
                var createPfpFileResult = await _collaboratorFileDataAccess.InsertFileWithOutput(collab.PfpUrl, pfpFile).ConfigureAwait(false);
                if (!createPfpFileResult.IsSuccessful)
                {
                    result.ErrorMessage = createPfpFileResult.ErrorMessage;
                    return result;
                }

                var updatePfPFileResult = await _collaboratorDataAccess.UpdatePfpFileId(createCollabResult.Payload, createPfpFileResult.Payload).ConfigureAwait(false);
                if (!updatePfPFileResult.IsSuccessful)
                {
                    result.ErrorMessage = updatePfPFileResult.ErrorMessage;
                    return result;
                }
            }

            // create junction table for each collaborator file that has been uploaded
            for(int i = 0; i < collabFiles.Length && i < collab.CollabUrls!.Count; i++)
            {
                var fileResult = await _collaboratorFileDataAccess.InsertFileWithOutput(collab.CollabUrls[i], collabFiles[i]).ConfigureAwait(false);
                if(!fileResult.IsSuccessful)
                {
                    return Result.Failure(fileResult.ErrorMessage!,
                    (int)StatusCodes.Status500InternalServerError);
                }
                var junctionResult = await _collaboratorFileJunctionDataAccess.InsertCollaboratorFile(createCollabResult.Payload, fileResult.Payload).ConfigureAwait(false);
                if (!junctionResult.IsSuccessful)
                {
                    return Result.Failure(junctionResult.ErrorMessage!,
                    (int)StatusCodes.Status500InternalServerError);
                }
            }
            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> DeleteCollaborator(int collabId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> DeleteCollaboratorWithAccountId(int accountId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> EditCollaborator(int collabId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> UpdatePfpFileId(int collabId, int pfpFileId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<CollaboratorProfile>> GetCollaborator(int collabId)
        {
            throw new NotImplementedException();
        }
        public async Task<Result> ChangeVisibility(int collabId, bool isPublic)
        {
            throw new NotImplementedException();
        }
    }
}

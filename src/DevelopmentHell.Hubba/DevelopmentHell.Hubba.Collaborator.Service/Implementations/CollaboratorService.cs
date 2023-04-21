using DevelopmentHell.Hubba.Collaborator.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.Collaborator.Service.Implementations
{
    public class CollaboratorService : ICollaboratorService
    {
        private ICollaboratorsDataAccess _collaboratorDataAccess;
        private ICollaboratorFileDataAccess _collaboratorFileDataAccess;
        private ICollaboratorFileJunctionDataAccess _collaboratorFileJunctionDataAccess;
        private IValidationService _validationService;

        public CollaboratorService(ICollaboratorsDataAccess collaboratorDataAccess, ICollaboratorFileDataAccess collaboratorFileDataAccess,
            ICollaboratorFileJunctionDataAccess collaboratorFileJunctionDataAccess, IValidationService validationService)
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
            if (collabFiles == null)
            {
                return Result.Failure("Collaborator files are null. Unable to upload collaborator files to junction table database.",
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
            if(pfpFile != null) { 
                var createPfpFileResult = await _collaboratorFileDataAccess.InsertFileWithOutputId("Placeholder Url", pfpFile).ConfigureAwait(false);
                if (!createPfpFileResult.IsSuccessful)
                {
                    result.ErrorMessage = createPfpFileResult.ErrorMessage;
                    return result;
                }

                // TODO: Complete upload file manager
                //Result uploadPfp = await CollaboratorFileUploadManager.Upload(pfpFile, createPfpFileResult.Payload).ConfigureAwait(false);
                //if(!uploadPfp.IsSuccessful)
                //{
                //    result.IsSuccessful = false;
                //    result.ErrorMessage = "Profile picture upload failed.";
                //    return result;
                //}
                //collab.PfpUrl = uploadPfp.Payload;

                collab.PfpUrl = string.Format("www.server.com/collaborator/{0}/file/{1}", createCollabResult.Payload, createPfpFileResult.Payload);

                var updatePfpFileResult = await _collaboratorFileDataAccess.UpdateFileUrl(createPfpFileResult.Payload, collab.PfpUrl);
                if (!updatePfpFileResult.IsSuccessful)
                {
                    result.ErrorMessage = "Unable to update pfp url in file database after upload." + updatePfpFileResult.ErrorMessage;
                    return result;
                }

                var updateCollabPfpResult = await _collaboratorDataAccess.UpdatePfpFileId(createCollabResult.Payload, createPfpFileResult.Payload).ConfigureAwait(false);
                if (!updateCollabPfpResult.IsSuccessful)
                {
                    result.ErrorMessage = "Unable to update pfp url in collab database after upload" + updateCollabPfpResult.ErrorMessage;
                    return result;
                }
            }

            // create junction table for each collaborator file that has been uploaded
            for(int i = 0; i < collabFiles.Length; i++)
            {
                var fileResult = await _collaboratorFileDataAccess.InsertFileWithOutputId("Placeholder Url", collabFiles[i]).ConfigureAwait(false);
                if(!fileResult.IsSuccessful)
                {
                    return Result.Failure(fileResult.ErrorMessage!,
                    (int)StatusCodes.Status500InternalServerError);
                }

                // TODO: Complete upload file manager
                //var uploadFile = await CollaboratorFileUploadManager.Upload(collabFiles[i], fileResult.Payload).ConfigureAwait(false);
                //if(uploadFile == null || !uploadFile.IsSuccessful)
                //{
                //    result.IsSuccessful = false;
                //    result.ErrorMessage = "Profile picture upload failed.";
                //    return result;
                //}
                //collab.CollabUrls.append(uploadFile.Payload);

                collab.CollabUrls!.Add(string.Format("www.server.com/collaborator/{0}/file/{1}", createCollabResult.Payload, fileResult.Payload));

                var updateFileResult = await _collaboratorFileDataAccess.UpdateFileUrl(fileResult.Payload, collab.CollabUrls[i]);
                if (!updateFileResult.IsSuccessful)
                {
                    result.ErrorMessage = "Unable to update pfp url in file database after upload." + updateFileResult.ErrorMessage;
                    return result;
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

        public async Task<Result> EditCollaborator(CollaboratorProfile collab, IFormFile[]? collabFiles = null,
            string[]? removedFiles = null, IFormFile? pfpFile = null)
        {
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

            var collabIdResult = await _collaboratorDataAccess.GetCollaboratorId(accountIdInt).ConfigureAwait(false);
            if (!collabIdResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to find requested collaborator Id for edits using ownerId."));
            }

            var collabResult = await _collaboratorDataAccess.GetCollaboratorWithPfpId(collabIdResult.Payload).ConfigureAwait(false);
            if (!collabResult.IsSuccessful) 
            {
                return new(Result.Failure("Unable to find requested collaborator object for edits using collabId."));
            }

            if (removedFiles != null &&  removedFiles.Length > 0)
            {
                var deleteResult = await _collaboratorFileDataAccess.DeleteFilesFromUrl(removedFiles).ConfigureAwait(false);
                if (!deleteResult.IsSuccessful)
                {
                    return new(Result.Failure("Unable to delete removed files. " + deleteResult.ErrorMessage));
                }   
            }

            // check if the profile picture was one of the removed files
            if (collabResult.Payload != null && !string.IsNullOrEmpty(collabResult.Payload!.PfpUrl))
            {
                int.TryParse(collabResult.Payload!.PfpUrl, out int pfpIdInt);
                List<int> pfpIdList = new List<int>() { pfpIdInt };
                // selecting to see if pfp file still exists
                var checkPfpResult = await _collaboratorFileDataAccess.SelectFileUrls(pfpIdList).ConfigureAwait(false);

                // if the pfp file was removed, either set to null or add the new one
                if(checkPfpResult.IsSuccessful && checkPfpResult.Payload!.Count == 0)
                {
                    if(pfpFile == null)
                    {
                        var updateCollabResult = _collaboratorDataAccess.UpdatePfpFileId(collabIdResult.Payload);
                    }
                    else
                    {
                        var createPfpFileResult = await _collaboratorFileDataAccess.InsertFileWithOutputId("Placeholder Url", pfpFile).ConfigureAwait(false);
                        if (!createPfpFileResult.IsSuccessful)
                        {
                            return new(Result.Failure(""+createPfpFileResult.ErrorMessage));
                        }

                        // TODO: Complete upload file manager
                        //Result uploadPfp = await CollaboratorFileUploadManager.Upload(pfpFile, createPfpFileResult.Payload).ConfigureAwait(false);
                        //if(!uploadPfp.IsSuccessful)
                        //{
                        //    result.IsSuccessful = false;
                        //    result.ErrorMessage = "Profile picture upload failed.";
                        //    return result;
                        //}
                        //collab.PfpUrl = uploadPfp.Payload;

                        collab.PfpUrl = collab.PfpUrl = string.Format("www.server.com/collaborator/{0}/file/{1}", collabIdResult.Payload, createPfpFileResult.Payload);

                        var updatePfpFileResult = await _collaboratorFileDataAccess.UpdateFileUrl(createPfpFileResult.Payload, collab.PfpUrl);
                        if (!updatePfpFileResult.IsSuccessful)
                        {
                            return new(Result.Failure("Unable to update pfp url in file database after upload." + updatePfpFileResult.ErrorMessage));
                        }

                        var updateCollabPfpResult = await _collaboratorDataAccess.UpdatePfpFileId(collabIdResult.Payload, createPfpFileResult.Payload).ConfigureAwait(false);
                        if (!updateCollabPfpResult.IsSuccessful)
                        {
                            return new(Result.Failure("Unable to update pfp url in collab database after upload" + updateCollabPfpResult.ErrorMessage));
                        }
                    }
                }
            }

            // create junction table for each collaborator file that has been uploaded
            if (collabFiles != null)
            {
                for (int i = 0; i < collabFiles.Length; i++)
                {
                    // upload file details to database
                    var fileResult = await _collaboratorFileDataAccess.InsertFileWithOutputId("Placeholder Url", collabFiles[i]).ConfigureAwait(false);
                    if (!fileResult.IsSuccessful)
                    {
                        return Result.Failure(fileResult.ErrorMessage!,
                        (int)StatusCodes.Status500InternalServerError);
                    }

                    // TODO: Complete upload file manager
                    //var uploadFile = await CollaboratorFileUploadManager.Upload(collabFiles[i], fileResult.Payload).ConfigureAwait(false);
                    //if(uploadFile == null || !uploadFile.IsSuccessful)
                    //{
                    //    result.IsSuccessful = false;
                    //    result.ErrorMessage = "Profile picture upload failed.";
                    //    return result;
                    //}
                    //collab.CollabUrls.append(uploadFile.Payload);

                    collab.CollabUrls!.Add(string.Format("www.server.com/collaborator/{0}/file/{1}", collabIdResult.Payload, fileResult.Payload));

                    var updateFileResult = await _collaboratorFileDataAccess.UpdateFileUrl(fileResult.Payload, collab.CollabUrls[i]);
                    if (!updateFileResult.IsSuccessful)
                    {
                        return new(Result.Failure("Unable to update pfp url in file database after upload." + updateFileResult.ErrorMessage));
                    }

                    var junctionResult = await _collaboratorFileJunctionDataAccess.InsertCollaboratorFile(collabIdResult.Payload, 
                        fileResult.Payload).ConfigureAwait(false);
                    if (!junctionResult.IsSuccessful)
                    {
                        return Result.Failure(junctionResult.ErrorMessage!,
                        (int)StatusCodes.Status500InternalServerError);
                    }
                }
            }

            var updateResult = await _collaboratorDataAccess.Update(collabIdResult.Payload, collab);
            if (!updateResult.IsSuccessful)
            {
                return Result.Failure(updateResult.ErrorMessage!);
            }

            return new Result()
            {
                IsSuccessful = true,
            };
        }

        public async Task<Result> UpdatePfpFileId(int collabId, int pfpFileId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<CollaboratorProfile>> GetCollaborator(int collabId)
        {
            Result<CollaboratorProfile> getCollabResult = await _collaboratorDataAccess.GetCollaboratorWithPfpId(collabId).ConfigureAwait(false);

            // get the base collaborator information from collaborator table
            // NOTE: THE COLLABORATOR PFP URL IS A FILE ID REFERENCE AT THIS POINT
            if(!getCollabResult.IsSuccessful)
            {
                return new(Result.Failure(string.Format("Failed to get collaborator {0} ", collabId) + 
                    getCollabResult.ErrorMessage,StatusCodes.Status500InternalServerError));
            }
            if(getCollabResult.Payload == null)
            {
                return new(Result.Failure(string.Format("Failed to find collaborator {0} ", collabId) +
                    getCollabResult.ErrorMessage, StatusCodes.Status404NotFound));
            }

            // if the user has a collaborator profile picture then correctly attach the URL
            if (getCollabResult.Payload.PfpUrl != null)
            {
                int.TryParse(getCollabResult.Payload.PfpUrl, out int pfpId);
                List<int> pfpIdList = new List<int>() { pfpId };
                var getPfpUrlResult = await _collaboratorFileDataAccess.SelectFileUrls(pfpIdList).ConfigureAwait(false);
                if (!getPfpUrlResult.IsSuccessful)
                {
                    return new(Result.Failure(string.Format("Failed to get collaborator {0} pfp urls ", collabId) +
                        getPfpUrlResult.ErrorMessage, StatusCodes.Status500InternalServerError));
                }
                if (getPfpUrlResult.Payload == null)
                {
                    return new(Result.Failure(string.Format("Failed to find collaborator {0} pfp urls ", collabId) +
                        getPfpUrlResult.ErrorMessage, StatusCodes.Status404NotFound));
                }
                if (getPfpUrlResult.Payload.Count > 1)
                {
                    return new(Result.Failure(string.Format("Found unexpected number of collaborator {} profile picutres ", collabId) +
                        getPfpUrlResult.ErrorMessage, StatusCodes.Status404NotFound));
                }

                getCollabResult.Payload.PfpUrl = getPfpUrlResult.Payload[0];
            }

            // get the file ids from junction table
            var getFileIdsResult = await _collaboratorFileJunctionDataAccess.SelectFiles(collabId).ConfigureAwait(false);
            if (!getFileIdsResult.IsSuccessful)
            {
                return new(Result.Failure(string.Format("Failed to get collaborator {0} files ", collabId) +
                    getFileIdsResult.ErrorMessage, StatusCodes.Status500InternalServerError));
            }
            if (getFileIdsResult.Payload == null)
            {
                return new(Result.Failure(string.Format("Failed to find collaborator {0} files ", collabId) +
                    getFileIdsResult.ErrorMessage, StatusCodes.Status404NotFound));
            }

            // get the correct file urls from file table
            var getFileUrlsResult = await _collaboratorFileDataAccess.SelectFileUrls(getFileIdsResult.Payload).ConfigureAwait(false);
            if (!getFileUrlsResult.IsSuccessful)
            {
                return new(Result.Failure(string.Format("Failed to get collaborator {0} file urls ", collabId) +
                    getFileIdsResult.ErrorMessage, StatusCodes.Status500InternalServerError));
            }
            if (getFileUrlsResult.Payload == null)
            {
                return new(Result.Failure(string.Format("Failed to find collaborator {0} file urls ", collabId) +
                    getFileIdsResult.ErrorMessage, StatusCodes.Status404NotFound));
            }

            // add file urls to collaborator object then validate
            getCollabResult.Payload.CollabUrls = getFileUrlsResult.Payload;
            var validationResult = _validationService.ValidateCollaborator(getCollabResult.Payload);
            if (!validationResult.IsSuccessful)
            {
                return new(Result.Failure("Collaborator found not properly formatted. " + validationResult.ErrorMessage,
                    StatusCodes.Status500InternalServerError));
            }

            return getCollabResult;
        }
        public async Task<Result> ChangeVisibility(int collabId, bool isPublic)
        {
            throw new NotImplementedException();
        }
    }
}

using DevelopmentHell.Hubba.Collaborator.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.Collaborator.Service.Implementations
{
    public class CollaboratorService : ICollaboratorService
    {
        private ICollaboratorsDataAccess _collaboratorDataAccess;
        private ICollaboratorFileDataAccess _collaboratorFileDataAccess;
        private ICollaboratorFileJunctionDataAccess _collaboratorFileJunctionDataAccess;
        private ICollaboratorUserVoteDataAccess _collaboratorUserVoteDataAccess;
        private ILoggerService _loggerService;
        private IValidationService _validationService;

        public CollaboratorService(ICollaboratorsDataAccess collaboratorDataAccess, ICollaboratorFileDataAccess collaboratorFileDataAccess,
            ICollaboratorFileJunctionDataAccess collaboratorFileJunctionDataAccess, ICollaboratorUserVoteDataAccess collaboratorUserVoteDataAccess,
            ILoggerService loggerService, IValidationService validationService)
        {
            _collaboratorDataAccess = collaboratorDataAccess;
            _collaboratorFileDataAccess = collaboratorFileDataAccess;
            _collaboratorFileJunctionDataAccess = collaboratorFileJunctionDataAccess;
            _collaboratorUserVoteDataAccess = collaboratorUserVoteDataAccess;
            _loggerService = loggerService;
            _validationService = validationService;
        }

        public async Task<Result> CreateCollaborator(CollaboratorProfile collab, IFormFile[] collabFiles, IFormFile? pfpFile = null)
        {
            // Get the ID of current thread
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

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

            // this list is to keep track of waht is uploaded to the server so that if anything fails, they will be removed
            List<int> successfullyUploadedFileIds = new List<int>();

            // add profile picture if applicable and update collaborator
            if (pfpFile != null) { 
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
                //string PfpUrl = uploadPfp.Payload;

                string pfpUrl = string.Format("www.server.com/collaborator/{0}/file/{1}", accountIdInt, createPfpFileResult.Payload);
                successfullyUploadedFileIds.Add(createPfpFileResult.Payload);

                // update the file url in the file database, if they fail remove the file from the server
                var updatePfpFileResult = await _collaboratorFileDataAccess.UpdateFileUrl(createPfpFileResult.Payload, pfpUrl);
                if (!updatePfpFileResult.IsSuccessful)
                {
                    await RemoveFileFromServer(accountIdInt, createPfpFileResult.Payload).ConfigureAwait(false);
                    result.ErrorMessage = "Unable to update pfp url in file database after upload." + updatePfpFileResult.ErrorMessage;
                    return result;
                }

                var updateCollabPfpResult = await _collaboratorDataAccess.UpdatePfpFileId(createCollabResult.Payload, createPfpFileResult.Payload).ConfigureAwait(false);
                if (!updateCollabPfpResult.IsSuccessful)
                {
                    await RemoveFileFromServer(accountIdInt, createPfpFileResult.Payload).ConfigureAwait(false);
                    result.ErrorMessage = "Unable to update pfp url in file database after upload" + updateCollabPfpResult.ErrorMessage;
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


                successfullyUploadedFileIds.Add(fileResult.Payload);
                string collabUrl = (string.Format("www.server.com/collaborator/{0}/file/{1}", accountIdInt, fileResult.Payload));

                var updateFileResult = await _collaboratorFileDataAccess.UpdateFileUrl(fileResult.Payload, collabUrl);
                if (!updateFileResult.IsSuccessful)
                {
                    // remove all the previously uploaded files
                    foreach(int fileId in successfullyUploadedFileIds)
                    {
                        await RemoveFileFromServer(accountIdInt, fileId).ConfigureAwait(false);
                    }
                    result.ErrorMessage = "Unable to update file url in file database after upload." + updateFileResult.ErrorMessage;
                    return result;
                }

                var junctionResult = await _collaboratorFileJunctionDataAccess.InsertCollaboratorFile(createCollabResult.Payload, fileResult.Payload).ConfigureAwait(false);
                if (!junctionResult.IsSuccessful)
                {
                    // remove all the previously uploaded files
                    foreach (int fileId in successfullyUploadedFileIds)
                    {
                        await RemoveFileFromServer(accountIdInt, fileId).ConfigureAwait(false);
                    }
                    return Result.Failure("Could not update junction table in collaborator database. " + junctionResult.ErrorMessage!,
                    (int)StatusCodes.Status500InternalServerError);
                }
            }
            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> DeleteCollaborator(int collabId)
        {
            // get accountId
            var getOwnerIdResult = await _collaboratorDataAccess.GetOwnerId(collabId).ConfigureAwait(false);
            if (!getOwnerIdResult.IsSuccessful)
            {
                return new(Result.Failure("Cannot find owner of collaborator to be removed. " + getOwnerIdResult.ErrorMessage));
            }
            int ownerId = getOwnerIdResult.Payload;


            // get files to be deleted from server from file database
            var fileIdResult = await _collaboratorFileDataAccess.SelectFileIdsFromOwner(ownerId).ConfigureAwait(false);
            if(!fileIdResult.IsSuccessful)
            {
                return new(Result.Failure("Cannot find files to be removed. " + fileIdResult.ErrorMessage));
            }
            if (fileIdResult.Payload == null)
            {
                return new(Result.Failure("Cannot find files to be removed. " + fileIdResult.ErrorMessage));
            }

            // delete file database
            var deleteFilesResult = await _collaboratorFileDataAccess.DeleteFilesFromFileId(fileIdResult.Payload!).ConfigureAwait(false);
            if(!deleteFilesResult.IsSuccessful)
            {
                return new(Result.Failure("Files were unable to be deleted. " + deleteFilesResult.ErrorMessage));
            }

            // delete collaborator database
            var deleteCollabResult = await _collaboratorDataAccess.Delete(collabId).ConfigureAwait(false);
            if (!deleteCollabResult.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.SERVER, $"Unable to delete collaborator" +
                    $" {collabId} but files were removed from database.", null);
                return new(Result.Failure($"Collaborator {collabId} was unable to be deleted, files were removed from database. " 
                    + deleteCollabResult.ErrorMessage));
            }

            // delete files from server
            foreach (int fileId in fileIdResult.Payload!) { 
                var removeFileFromServerResult = RemoveFileFromServer(ownerId, fileId).ConfigureAwait(false);
            }

            return new Result() { IsSuccessful = true};
        }

        public async Task<Result> DeleteCollaboratorWithAccountId(int accountId)
        {
            var hasCollaboratorResult = await HasCollaborator(accountId).ConfigureAwait(false); 
            if(!hasCollaboratorResult.IsSuccessful)
            {
                return new(Result.Failure("Error deleting account's collaborator profile. " + hasCollaboratorResult.ErrorMessage));
            }

            // if there is no collaborator, there is nothing to delete
            if(!hasCollaboratorResult.Payload)
            {
                return new Result() { IsSuccessful = true };
            }

            // get the corresponding collaborator Id
            var getCollaboratorResult = await _collaboratorDataAccess.GetCollaboratorId(accountId).ConfigureAwait(false);
            if (!getCollaboratorResult.IsSuccessful)
            {
                return new(Result.Failure("Error selecting collaborator id for deletion. " + getCollaboratorResult.ErrorMessage));
            }

            // otherwise delete the account
            var deleteCollaborator = await DeleteCollaborator(getCollaboratorResult.Payload).ConfigureAwait(false);
            return deleteCollaborator;
        }

        public async Task<Result> RemoveCollaborator(int collabId)
        {
            // get accountId
            var getOwnerIdResult = await _collaboratorDataAccess.GetOwnerId(collabId).ConfigureAwait(false);
            if (!getOwnerIdResult.IsSuccessful)
            {
                return new(Result.Failure("Cannot find owner of collaborator to be removed. " + getOwnerIdResult.ErrorMessage,
                    StatusCodes.Status404NotFound));
            }
            int ownerId = getOwnerIdResult.Payload;


            // get files to be deleted from server from file database
            var fileIdResult = await _collaboratorFileDataAccess.SelectFileIdsFromOwner(ownerId).ConfigureAwait(false);
            if (!fileIdResult.IsSuccessful)
            {
                return new(Result.Failure("Cannot find files to be removed. " + fileIdResult.ErrorMessage, StatusCodes.Status404NotFound));
            }
            if (fileIdResult.Payload == null)
            {
                return new(Result.Failure("Cannot find files to be removed. " + fileIdResult.ErrorMessage, StatusCodes.Status404NotFound));
            }

            // delete file database
            var deleteFilesResult = await _collaboratorFileDataAccess.DeleteFilesFromFileId(fileIdResult.Payload!).ConfigureAwait(false);
            if (!deleteFilesResult.IsSuccessful)
            {
                return new(Result.Failure("Files were unable to be deleted. " + deleteFilesResult.ErrorMessage));
            }

            CollaboratorProfile defaultValuesCollab = new CollaboratorProfile()
            {
                Name = "",
                PfpUrl = null,
                ContactInfo = "",
                Tags = "",
                Description = "",
                Availability = "",
                Votes = 0,
                CollabUrls = null,
                Published = false
            };

            // clear collaborator database
            var removeCollabResult = await _collaboratorDataAccess.Update(collabId, defaultValuesCollab).ConfigureAwait(false);
            if (!removeCollabResult.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.SERVER, $"Unable to clear collaborator" +
                    $" {collabId} but files were removed from database.", null);
                return new(Result.Failure($"Collaborator {collabId} was unable to be cleared, files were removed from database. "
                    + removeCollabResult.ErrorMessage));
            }

            // remove pfpurl from collaborator database
            var removepfpUrl = await _collaboratorDataAccess.UpdatePfpFileId(collabId).ConfigureAwait(false);
            if (!removepfpUrl.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.ERROR, Category.SERVER, $"Unable to clear pfp url of collaborator" +
                    $" {collabId} but details were cleared and files were removed from database.", null);
                return new(Result.Failure($"Collaborator {collabId} pfp url was unable to be deleted, data and files were removed from database. "
                    + removepfpUrl.ErrorMessage));
            }

            // delete files from server
            foreach (int fileId in fileIdResult.Payload!)
            {
                var removeFileFromServerResult = RemoveFileFromServer(ownerId, fileId).ConfigureAwait(false);
            }

            return new Result() { IsSuccessful = true };

        }

        public async Task<Result> EditCollaborator(CollaboratorProfile collab, IFormFile[]? collabFiles = null,
            string[]? removedFileUrls = null, IFormFile? pfpFile = null)
        {
            // get account id of user
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

            if (removedFileUrls != null)
            {
                var removedFileIds = _collaboratorFileDataAccess.SelectFileIdsFromUrl(removedFileUrls.ToList());
            }

            var collabIdResult = await _collaboratorDataAccess.GetCollaboratorId(accountIdInt).ConfigureAwait(false);
            if (!collabIdResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to find requested collaborator Id for edits using ownerId."));
            }

            int collabId = (int)collabIdResult.Payload;

            var collabResult = await _collaboratorDataAccess.GetCollaborator(collabId).ConfigureAwait(false);
            if (!collabResult.IsSuccessful) 
            {
                return new(Result.Failure("Unable to find requested collaborator object for edits using collabId."));
            }

            // removing provided array of removedFilesUrls
            if (removedFileUrls != null &&  removedFileUrls.Length > 0)
            {
                var deleteResult = await _collaboratorFileDataAccess.DeleteFilesFromUrl(removedFileUrls).ConfigureAwait(false);
                if (!deleteResult.IsSuccessful)
                {
                    return new(Result.Failure("Unable to delete removed files. " + deleteResult.ErrorMessage));
                }

                // TODO: Complete upload file manager - remove files
                //var deleteFileResult = await CollaboratorFileUploadManager.Upload(collabFiles[i], fileResult.Payload).ConfigureAwait(false);
                //if(!deleteFileResult.IsSuccessful)
                //{
                //    result.IsSuccessful = false;
                //    result.ErrorMessage = "File removals failed.";
                //    return result;
                //}
                //collab.CollabUrls.append(uploadFile.Payload);
            }

            // keeping track of all uploaded files to server so they can be removed if an error occurs
            List<int> successfullyUploadedFileIds = new List<int>();

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
                        var updateCollabResult = _collaboratorDataAccess.UpdatePfpFileId(collabId);
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

                        collab.PfpUrl = collab.PfpUrl = string.Format("www.server.com/collaborator/{0}/file/{1}", accountIdInt, createPfpFileResult.Payload);
                        successfullyUploadedFileIds.Add(createPfpFileResult.Payload);

                        // removing the profile picture from the server if an error occurred
                        var updatePfpFileResult = await _collaboratorFileDataAccess.UpdateFileUrl(createPfpFileResult.Payload, collab.PfpUrl);
                        if (!updatePfpFileResult.IsSuccessful)
                        {
                            await RemoveFileFromServer(accountIdInt, createPfpFileResult.Payload).ConfigureAwait(false);
                            return new(Result.Failure("Unable to update pfp url in file database after upload." + updatePfpFileResult.ErrorMessage));
                        }

                        var updateCollabPfpResult = await _collaboratorDataAccess.UpdatePfpFileId(collabId, createPfpFileResult.Payload).ConfigureAwait(false);
                        if (!updateCollabPfpResult.IsSuccessful)
                        {
                            await RemoveFileFromServer(accountIdInt, createPfpFileResult.Payload).ConfigureAwait(false);
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

                    successfullyUploadedFileIds.Add(fileResult.Payload);
                    collab.CollabUrls!.Add(string.Format("www.server.com/collaborator/{0}/file/{1}", accountIdInt, fileResult.Payload));

                    var updateFileResult = await _collaboratorFileDataAccess.UpdateFileUrl(fileResult.Payload, collab.CollabUrls[i]);
                    if (!updateFileResult.IsSuccessful)
                    {
                        // remove all the previously uploaded files
                        foreach (int fileId in successfullyUploadedFileIds)
                        {
                            await RemoveFileFromServer(accountIdInt, fileId).ConfigureAwait(false);
                        }
                        return new(Result.Failure("Unable to update file url in file database after upload." + updateFileResult.ErrorMessage));
                    }

                    var junctionResult = await _collaboratorFileJunctionDataAccess.InsertCollaboratorFile(collabId, 
                        fileResult.Payload).ConfigureAwait(false);
                    if (!junctionResult.IsSuccessful)
                    {
                        // remove all the previously uploaded files
                        foreach (int fileId in successfullyUploadedFileIds)
                        {
                            await RemoveFileFromServer(accountIdInt, fileId).ConfigureAwait(false);
                        }
                        return Result.Failure(junctionResult.ErrorMessage!,
                        (int)StatusCodes.Status500InternalServerError);
                    }
                }
            }

            var updateResult = await _collaboratorDataAccess.Update(collabId, collab);
            if (!updateResult.IsSuccessful)
            {
                return Result.Failure(updateResult.ErrorMessage!);
            }

            return new Result()
            {
                IsSuccessful = true,
            };
        }


        public async Task<Result<CollaboratorProfile>> GetCollaborator(int collabId)
        {
            Result<CollaboratorProfile> getCollabResult = await _collaboratorDataAccess.GetCollaborator(collabId).ConfigureAwait(false);

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

        public async Task<Result<int>> GetOwnerId(int collabId)
        {
            if(collabId < 0)
            {
                return new(Result.Failure("Invalid collaborator id.", StatusCodes.Status412PreconditionFailed));
            }
            var accountIdResult = await _collaboratorDataAccess.GetOwnerId(collabId).ConfigureAwait(false);
            if (!accountIdResult.IsSuccessful)
            {
                return new(Result.Failure("Could not get owner Id. " + accountIdResult.ErrorMessage));
            }
            return accountIdResult;   
        }

        public async Task<Result> UpdatePublished(int collabId, bool isPublic)
        {
            if (collabId < 0)
            {
                return new(Result.Failure("Invalid collaborator id.", StatusCodes.Status412PreconditionFailed));
            }
            // change the published status of the collab
            var changeVisibilityResult = await _collaboratorDataAccess.SetPublished(collabId, isPublic).ConfigureAwait(false);
            if (!changeVisibilityResult.IsSuccessful)
            {
                return new(Result.Failure("Error while changing. " + changeVisibilityResult.ErrorMessage));
            }

            return changeVisibilityResult;
        }

        public async Task<Result<bool>> HasCollaborator(int accountId)
        {
            if (accountId < 0)
            {
                return new(Result.Failure("Invalid account id.", StatusCodes.Status412PreconditionFailed));
            }
            var hasCollaboratorResult = await _collaboratorDataAccess.HasCollaborator(accountId).ConfigureAwait(false);
            if(!hasCollaboratorResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to check if account has collaborator. " + hasCollaboratorResult.ErrorMessage));
            }
            return hasCollaboratorResult;
        }

        public async Task<Result> Vote(int collabId, bool upvote)
        {
            // get account id of user
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

            Result vote = new Result()
            {
                IsSuccessful = true,
            };

            if (collabId < 0)
            {
                return new(Result.Failure("Invalid collaborator id.", StatusCodes.Status412PreconditionFailed));
            }
            if(upvote)
            {
                vote = await _collaboratorUserVoteDataAccess.Upvote(collabId, accountIdInt).ConfigureAwait(false);
            }
            else
            {
                vote = await _collaboratorUserVoteDataAccess.Downvote(collabId, accountIdInt).ConfigureAwait(false);
            }
            if (!vote.IsSuccessful)
            {
                return new(Result.Failure("Unable to count vote. " + vote.ErrorMessage, vote.StatusCode));
            }
            return vote;
        }

        private async Task<Result> RemoveFileFromServer(int accountId, int fileID)
        {
            // this will be used to log files removed from server if they fail
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);
            //var deleteFile = await CollaboratorFileUploadManager.Delete(file).ConfigureAwait(false);
            //if(deleteFile == null || !deleteFile.IsSuccessful)
            //{
                //Result logRes = _loggerService.Log(Models.LogLevel.ERROR, Category.SERVER, $"Unable to remove uploaded" +
                //    $" files. Account id: {accountIdInt} File id: {fileId}", null);
            //}
            return new Result() { IsSuccessful = true };
        }
    }
}

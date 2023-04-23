using DevelopmentHell.Hubba.Collaborator.Service.Abstractions;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System.Configuration;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.Collaborator.Service.Implementations
{
    public class CollaboratorService : ICollaboratorService
    {
        private ICollaboratorsDataAccess _collaboratorDataAccess;
        private ICollaboratorFileDataAccess _collaboratorFileDataAccess;
        private ICollaboratorFileJunctionDataAccess _collaboratorFileJunctionDataAccess;
        private ICollaboratorUserVoteDataAccess _collaboratorUserVoteDataAccess;
        private IFileService _fileService;
        private ILoggerService _loggerService;
        private IValidationService _validationService;
        private string _dirPath = "Collaborators";
        private string _ftpServer = ConfigurationManager.AppSettings["FTPServer"]!;

        public CollaboratorService(ICollaboratorsDataAccess collaboratorDataAccess, ICollaboratorFileDataAccess collaboratorFileDataAccess,
            ICollaboratorFileJunctionDataAccess collaboratorFileJunctionDataAccess, ICollaboratorUserVoteDataAccess collaboratorUserVoteDataAccess,
            IFileService fileService, ILoggerService loggerService, IValidationService validationService)
        {
            _collaboratorDataAccess = collaboratorDataAccess;
            _collaboratorFileDataAccess = collaboratorFileDataAccess;
            _collaboratorFileJunctionDataAccess = collaboratorFileJunctionDataAccess;
            _collaboratorUserVoteDataAccess = collaboratorUserVoteDataAccess;
            _fileService = fileService;
            _loggerService = loggerService;
            _validationService = validationService;
        }

        public async Task<Result> CreateCollaborator(CollaboratorProfile collab, IFormFile[] collabFiles, IFormFile? pfpFile = null)
        {
            // Get the ID of current thread
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIdStr = principal.FindFirstValue("sub");
            int.TryParse(accountIdStr, out int accountIdInt);

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

                // uploading profile picture to web server file directory
                var uploadFileResult = await UploadFileToServer(accountIdInt, createPfpFileResult.Payload, pfpFile).ConfigureAwait(false);
                if (!uploadFileResult.IsSuccessful)
                {
                    return uploadFileResult;
                }

                // storing the url and uploaded files
                string fileExtension = Path.GetExtension(pfpFile.FileName);
                string pfpUrl = string.Format("http://{0}/{1}/{2}{3}", _ftpServer, _dirPath + $"/{accountIdStr}", createPfpFileResult.Payload, fileExtension);
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
                    StatusCodes.Status500InternalServerError);
                }

                // uploading  picture to web server file directory
                var uploadFileResult = await UploadFileToServer(accountIdInt, fileResult.Payload, collabFiles[i]).ConfigureAwait(false);
                if(!uploadFileResult.IsSuccessful)
                {
                    return uploadFileResult;
                }

                // storing the url and uploaded files
                string fileExtension = Path.GetExtension(collabFiles[i].FileName);
                string collabUrl = string.Format("http://{0}/{1}/{2}{3}", _ftpServer, _dirPath + $"/{accountIdStr}", fileResult.Payload, fileExtension);

                successfullyUploadedFileIds.Add(fileResult.Payload);

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
            if(!fileIdResult.IsSuccessful || fileIdResult.Payload == null)
            {
                return new(Result.Failure("Cannot find files to be removed. " + fileIdResult.ErrorMessage));
            }

            // delete files from server
            foreach (int fileId in fileIdResult.Payload!)
            {
                await RemoveFileFromServer(ownerId, fileId).ConfigureAwait(false);
            }

            // delete file sql database
            var deleteFilesResult = await _collaboratorFileDataAccess.DeleteFilesFromFileId(fileIdResult.Payload!).ConfigureAwait(false);
            if(!deleteFilesResult.IsSuccessful)
            {
                return new(Result.Failure("Files were unable to be deleted. " + deleteFilesResult.ErrorMessage));
            }

            // delete collaborator sql database
            var deleteCollabResult = await _collaboratorDataAccess.Delete(collabId).ConfigureAwait(false);
            if (!deleteCollabResult.IsSuccessful)
            {
                return new(Result.Failure($"Collaborator {collabId} was unable to be deleted, files were removed from database. " 
                    + deleteCollabResult.ErrorMessage));
            }


            _loggerService.Log(Models.LogLevel.INFO, Category.BUSINESS, $"Deleted collaborator" +
                $" {collabId}.", null);
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
            if (!fileIdResult.IsSuccessful || fileIdResult.Payload == null)
            {
                return new(Result.Failure("Cannot find files to be removed. " + fileIdResult.ErrorMessage, StatusCodes.Status404NotFound));
            }

            // delete files from server
            foreach (int fileId in fileIdResult.Payload!)
            {
                var removeFileFromServerResult = await RemoveFileFromServer(ownerId, fileId).ConfigureAwait(false);
                if (!removeFileFromServerResult.IsSuccessful)
                {
                    return new(Result.Failure($"Collaborator uploaded files were unable to be deleted. "
                    + removeFileFromServerResult.ErrorMessage));
                    //TODO: ADD ROLLBACK
                }
            }

            // delete file database
            var deleteFilesResult = await _collaboratorFileDataAccess.DeleteFilesFromFileId(fileIdResult.Payload!).ConfigureAwait(false);
            if (!deleteFilesResult.IsSuccessful)
            {
                return new(Result.Failure("Files were unable to be deleted. " + deleteFilesResult.ErrorMessage));
                // TODO: ADD ROLLBACK
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
                return new(Result.Failure($"Collaborator was unable to be cleared. "
                    + removeCollabResult.ErrorMessage));
                // TODO: ADD ROLLBACK
            }

            // remove pfpurl from collaborator database
            var removepfpUrl = await _collaboratorDataAccess.UpdatePfpFileId(collabId).ConfigureAwait(false);
            if (!removepfpUrl.IsSuccessful)
            {
                return new(Result.Failure($"Collaborator pfp url was unable to be deleted. "
                    + removepfpUrl.ErrorMessage));
                //TODO: ADD ROLLBACK
            }

            return new Result() { IsSuccessful = true };

        }

        public async Task<Result> EditCollaborator(CollaboratorProfile collab, IFormFile[]? collabFiles = null,
            string[]? removedFileUrls = null, IFormFile? pfpFile = null)
        {
            // get account id of user
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIdStr = principal.FindFirstValue("sub");
            int.TryParse(accountIdStr, out int accountIdInt);

            var collabIdResult = await _collaboratorDataAccess.GetCollaboratorId(accountIdInt).ConfigureAwait(false);
            if (!collabIdResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to find owner of requested collaborator. " + collabIdResult.ErrorMessage));
            }

            int collabId = (int)collabIdResult.Payload;

            var collabResult = await GetCollaborator(collabId).ConfigureAwait(false);
            if (!collabResult.IsSuccessful) 
            {
                return new(Result.Failure("Unable to find requested collaborator object for edits using collabId." + collabResult.ErrorMessage));
            }

            // do not allow editing if a new profile picture is being uploaded without being removed
            if ((removedFileUrls != null && collabResult.Payload!.PfpUrl != null && !removedFileUrls.Contains<string>(collabResult.Payload!.PfpUrl))
                || (removedFileUrls == null && collabResult.Payload!.PfpUrl != null))
            {
                return new(Result.Failure("Cannot upload Pfp file without firt removing current Pfp file.", StatusCodes.Status412PreconditionFailed));
            }

            // if the Pfp is going to be removed, delete reference from sql database
            if (removedFileUrls != null && collabResult.Payload!.PfpUrl != null && removedFileUrls.Contains<string>(collabResult.Payload!.PfpUrl))
            {
                var updateCollabResult = await _collaboratorDataAccess.UpdatePfpFileId(collabId).ConfigureAwait(false);
                if (!updateCollabResult.IsSuccessful)
                {
                    return new(Result.Failure("Unable to clear pfp url in file database after removal." + updateCollabResult.ErrorMessage));
                }
            }

            // removing provided array of removedFilesUrls
            if (removedFileUrls != null &&  removedFileUrls.Length > 0)
            {

                // get the ids of removed files
                var deletedFileIdsResult = await _collaboratorFileDataAccess.SelectFileIdsFromUrl(removedFileUrls).ConfigureAwait(false); 
                if (!deletedFileIdsResult.IsSuccessful)
                {
                    return new(Result.Failure("Could not find files to be deleted. " + deletedFileIdsResult.ErrorMessage));
                }

                // remove files from web server
                foreach (var fileId in deletedFileIdsResult.Payload!)
                {
                    var removeFileFromServerResult = await RemoveFileFromServer(accountIdInt, fileId).ConfigureAwait(false);
                    if (!removeFileFromServerResult.IsSuccessful)
                    {
                        return new(Result.Failure($"Collaborator uploaded files were unable to be deleted. "
                        + removeFileFromServerResult.ErrorMessage));
                    }
                }

                // remove files from sql database
                var deleteResult = await _collaboratorFileDataAccess.DeleteFilesFromUrl(removedFileUrls).ConfigureAwait(false);
                if (!deleteResult.IsSuccessful)
                {
                    return new(Result.Failure("Unable to delete removed files. " + deleteResult.ErrorMessage));
                }
                
            }

            // keeping track of all uploaded files to server so they can be removed if an error occurs
            List<int> successfullyUploadedFileIds = new List<int>();

            // upload new profile picture
            if(pfpFile != null)
            {
                var createPfpFileResult = await _collaboratorFileDataAccess.InsertFileWithOutputId("Placeholder Url", pfpFile).ConfigureAwait(false);
                if (!createPfpFileResult.IsSuccessful)
                {
                    return new(Result.Failure("" + createPfpFileResult.ErrorMessage));
                }

                // uploading profile picture to web server file directory
                var uploadFileResult = await UploadFileToServer(accountIdInt, createPfpFileResult.Payload, pfpFile).ConfigureAwait(false);
                if (!uploadFileResult.IsSuccessful)
                {
                    return uploadFileResult;
                }

                // storing the url and uploaded files
                string fileExtension = Path.GetExtension(pfpFile.FileName);
                collab.PfpUrl = string.Format("http://{0}/{1}/{2}{3}", _ftpServer, _dirPath + $"/{accountIdStr}", createPfpFileResult.Payload, fileExtension);
                successfullyUploadedFileIds.Add(createPfpFileResult.Payload);

                // updating the profile picture to the sql server 
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

                    // uploading  picture to web server file directory
                    var uploadFileResult = await UploadFileToServer(accountIdInt, fileResult.Payload, collabFiles[i]).ConfigureAwait(false);
                    if (!uploadFileResult.IsSuccessful)
                    {
                        return uploadFileResult;
                    }

                    // storing the url and uploaded files
                    string fileExtension = Path.GetExtension(collabFiles[i].FileName);
                    successfullyUploadedFileIds.Add(fileResult.Payload);
                    if(collab.CollabUrls == null)
                    {
                        collab.CollabUrls = new List<string>();
                    }
                    collab.CollabUrls!.Add(string.Format("http://{0}/{1}/{2}{3}", _ftpServer, _dirPath + $"/{accountIdStr}", fileResult.Payload, fileExtension));

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
                    getCollabResult.ErrorMessage,StatusCodes.Status404NotFound));
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
            var getFileIdsResult = await _collaboratorFileJunctionDataAccess.SelectFileIdsFromCollabId(collabId).ConfigureAwait(false);
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
                return new(Result.Failure(string.Format("Failed to get collaborator file urls ", collabId) +
                    getFileIdsResult.ErrorMessage, StatusCodes.Status500InternalServerError));
            }
            if (getFileUrlsResult.Payload == null)
            {
                return new(Result.Failure(string.Format("Failed to find collaborator file urls ", collabId) +
                    getFileIdsResult.ErrorMessage, StatusCodes.Status404NotFound));
            }

            // add file urls to collaborator object then validate
            getCollabResult.Payload.CollabUrls = getFileUrlsResult.Payload;
            var validationResult = _validationService.ValidateCollaborator(getCollabResult.Payload);

            // if validation fails, check if the request came from owner
            if (!validationResult.IsSuccessful)
            {
                var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
                string accountIDStr = principal.FindFirstValue("sub");
                int.TryParse(accountIDStr, out int accountIdInt);
                var ownerIdResult = await GetOwnerId(collabId).ConfigureAwait(false);

                // if its not able to verify owner don't return
                if (!ownerIdResult.IsSuccessful)
                {
                    return new(Result.Failure("Collaborator found not properly formatted. " + validationResult.ErrorMessage,
                    StatusCodes.Status500InternalServerError));
                }
                // if it's not the owner dont return
                else if (ownerIdResult.Payload != accountIdInt)
                {
                    return new(Result.Failure("Collaborator is not properly formatted.", StatusCodes.Status401Unauthorized));
                }
                return getCollabResult;
            }
            return getCollabResult;
        }

        public async Task<Result<int>> GetOwnerId(int collabId)
        {
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
            var getCollaboratorResult = await GetCollaborator(collabId).ConfigureAwait(false);
            if (!getCollaboratorResult.IsSuccessful || getCollaboratorResult.Payload == null)
            {
                return new(Result.Failure("Unable to find collaborator to change visibility. " + getCollaboratorResult.ErrorMessage, StatusCodes.Status412PreconditionFailed));
            }
            var validateCollaboratorResult = _validationService.ValidateCollaborator(getCollaboratorResult.Payload);
            if (!validateCollaboratorResult.IsSuccessful)
            {
                return new(Result.Failure("Cannot change invalid collaborator format's visibility. " + getCollaboratorResult.ErrorMessage, StatusCodes.Status412PreconditionFailed));
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

        public async Task<Result<string[]>> GetFileUrls(int collabId)
        {
            var getPublishedResult = await _collaboratorDataAccess.GetPublished(collabId).ConfigureAwait(false);
            if (!getPublishedResult.IsSuccessful)
            {
                return new(Result.Failure("Cannot determine visibility of collaborator.", StatusCodes.Status500InternalServerError));
            }
            // if the published status is false, do not return any files
            if(!getPublishedResult.Payload)
            {
                return new Result<String[]>()
                {
                    IsSuccessful = true,
                    Payload = new string[0]                
                };
            }
            
            var getFilesResult = await _collaboratorFileJunctionDataAccess.SelectFileUrlsFromCollabId(collabId).ConfigureAwait(false);
            if(!getFilesResult.IsSuccessful)
            {
                return new(Result.Failure("Could not retrieve file urls from collaborator.", StatusCodes.Status500InternalServerError));
            }
            string[] fileUrls = getFilesResult.Payload!.ToArray();

            return new Result<string[]> { 
                IsSuccessful = true,
                Payload = fileUrls 
            };
        }

        public async Task<Result<int>> CountFilesWithoutPfp(int accountId)
        {
            var getFilesResult = await _collaboratorFileJunctionDataAccess.SelectFileUrlsFromOwnerId(accountId).ConfigureAwait(false);
            if (!getFilesResult.IsSuccessful)
            {
                return new(Result.Failure("Could not retrieve count of files from Owner. ", StatusCodes.Status500InternalServerError));
            }
            int count = getFilesResult.Payload!.Count();
            return new Result<int>
            {
                IsSuccessful = true,
                Payload = count
            };
        }

        private async Task<Result> RemoveFileFromServer(int accountId, int fileId)
        {
            Result result = new Result();
            string accountIdStr = accountId.ToString();
            string dirPath = _dirPath + $"/{accountIdStr}";
            var fileExtensionResult = await _collaboratorFileDataAccess.SelectFileExtension(fileId).ConfigureAwait(false);
            if (!fileExtensionResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to find extension for file to be removed. " + fileExtensionResult.ErrorMessage));
            }
            var deleteResult = await _fileService.DeleteFile(dirPath + "/" + fileId + fileExtensionResult.Payload);
            if (!deleteResult.IsSuccessful)
            {
                result.ErrorMessage = deleteResult.ErrorMessage;
            }
            Thread.Sleep(100);
            return new Result() { IsSuccessful = true };
        }

        private async Task<Result> UploadFileToServer(int accountId, int fileId, IFormFile file)
        {
            Result result = new Result();
            string accountIdStr = accountId.ToString();
            // build personal directory path
            string dirPath = _dirPath + $"/{accountIdStr}";
            var createDirResult = await _fileService.CreateDir(dirPath);
            if (!createDirResult.IsSuccessful)
            {
                result.ErrorMessage = createDirResult.ErrorMessage;
            }
            Thread.Sleep(100);
            // upload file with fileId as name
            string fileExtension = Path.GetExtension(file.FileName);
            
            var uploadResult = await _fileService.UploadIFormFile(dirPath, fileId + fileExtension, file).ConfigureAwait(false);
            if (!uploadResult.IsSuccessful)
            {
                result.ErrorMessage = uploadResult.ErrorMessage;
            }
            Thread.Sleep(100);
            result.IsSuccessful = true;
            return result;
        }
    }
}

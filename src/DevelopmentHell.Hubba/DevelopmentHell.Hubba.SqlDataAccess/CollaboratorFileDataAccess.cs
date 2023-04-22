using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class CollaboratorFileDataAccess : ICollaboratorFileDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private string _tableName;

        public CollaboratorFileDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _tableName = tableName;
        }
        public async Task<Result<int>> InsertFileWithOutputId(string fileUrl, IFormFile file)
        {
            Result<int> result = new Result<int>();

            // Get the ID of current thread
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

            var insertResult = await _insertDataAccess.InsertWithOutput(
                _tableName,
                new Dictionary<string, object>()
                {
                    { "FileName", file.Name},
                    { "FileType", Path.GetExtension(file.FileName).Substring(1)},
                    { "FileSize", file.Length},
                    { "FileUrl", fileUrl},
                    { "OwnerId", accountIdInt},
                    { "LastModifiedUser", accountIdInt},
                    { "CreateDate", DateTime.Now}
                },
                "FileId"
            ).ConfigureAwait(false);
            
            // check if the insertion was successful
            if(!insertResult.IsSuccessful) 
            {
                result.ErrorMessage = insertResult.ErrorMessage;
                return result;
            }

            if (!insertResult.IsSuccessful || insertResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = insertResult.ErrorMessage;
                return result;
            }
            if (insertResult.Payload.Count != 1)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = string.Format("Unexpected number of files added. Number was {0}", insertResult.Payload.Count);
                return result;
            }
            result.IsSuccessful = true;
            result.Payload = (int)(insertResult.Payload[0]["FileId"]);
            return result;

        }
        public async Task<Result> UpdateFileUrl(int fileId, string fileUrl)
        {
            Result result = new Result();

            // Get the ID of current thread
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

            var insertResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("FileId", "=", fileId),
                },
                new Dictionary<string, object>()
                {
                    { "FileUrl", fileUrl},
                    { "LastModifiedUser", accountIdInt},
                    { "UpdateDate", DateTime.Now}
                }
            ).ConfigureAwait(false);

            // check if the update was successful
            if (!insertResult.IsSuccessful)
            {
                result.ErrorMessage = insertResult.ErrorMessage;
                return result;
            }
            
            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result<List<string>>> SelectFileUrls(List<int> fileIds)
        {
            List<string> fileIdsString = new List<string>();
            foreach (int fileId in fileIds)
            {
                fileIdsString.Add(fileId.ToString());
            }

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.SelectWhereIn(
                _tableName,
                new List<String>() { "FileUrl" },
                "FileId",
                fileIdsString                
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                return new(Result.Failure("" + selectResult.ErrorMessage));
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 10)
            {
                return new(Result.Failure($"Selected more than the valid number of files: {payload.Count}" + selectResult.ErrorMessage));
            }

            List<String> fileUrls = new List<string>();

            foreach (var row in payload)
            {
                fileUrls.Add((string)row["FileUrl"]);
            }

            return new Result<List<string>>()
            {
                IsSuccessful = true,
                Payload = fileUrls
            };
        }

        public async Task<Result<List<int>>> SelectFileIdsFromUrl(List<string> fileUrls)
        {
            List<string> fileUrlsString = new List<string>();
            foreach (string fileUrl in fileUrls)
            {
                fileUrlsString.Add(fileUrl);
            }

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.SelectWhereIn(
                _tableName,
                new List<String>() { "FileId" },
                "FileUrl",
                fileUrlsString
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                return new(Result.Failure("" + selectResult.ErrorMessage));
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 10)
            {
                return new(Result.Failure($"Selected more than the valid number of files: {payload.Count}" + selectResult.ErrorMessage));
            }

            List<int> fileIds = new List<int>();

            foreach (var row in payload)
            {
                fileIds.Add((int)row["FileId"]);
            }

            return new Result<List<int>>()
            {
                IsSuccessful = true,
                Payload = fileIds
            };
        }

        public async Task<Result<List<int>>> SelectFileIdsFromOwner(int accountId)
        {
            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<String>() { "FileId" },
                new List<Comparator>()
                {
                    new Comparator("OwnerId", "=", accountId)
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                return new(Result.Failure("" + selectResult.ErrorMessage));
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 10)
            {
                return new(Result.Failure($"Selected more than the valid number of files: {payload.Count}" + selectResult.ErrorMessage));
            }

            List<int> fileIds = new List<int>();

            foreach (var row in payload)
            {
                fileIds.Add((int)row["FileId"]);
            }

            return new Result<List<int>>()
            {
                IsSuccessful = true,
                Payload = fileIds
            };
        }
        public async Task<Result> DeleteFilesFromUrl(string[] removedFileUrls)
        {
            List<string> removedFileUrlsList = removedFileUrls.ToList<string>();
            var deleteResult = await _deleteDataAccess.DeleteWhereIn(_tableName, "FileUrl", removedFileUrlsList).ConfigureAwait(false);

            return deleteResult;
        }

        public async Task<Result> DeleteFilesFromFileId(List<int> fileIds)
        {
            List<string> fileIdsString = new List<string>();
            foreach (var fileId in fileIds)
            {
                fileIdsString.Add(fileId.ToString());
            }

            var deleteResult = await _deleteDataAccess.DeleteWhereIn(_tableName, "FileId", fileIdsString).ConfigureAwait(false);
            return deleteResult;
        }

        public async Task<Result> DeleteFilesFromOwnerId(int ownerId)
        {
            List<string> ownerIdString = new List<string>();
            ownerIdString.Add(ownerId.ToString());

            var deleteResult = await _deleteDataAccess.DeleteWhereIn(_tableName, "OwnerId", ownerIdString).ConfigureAwait(false);
            return deleteResult;
        }
    }
}

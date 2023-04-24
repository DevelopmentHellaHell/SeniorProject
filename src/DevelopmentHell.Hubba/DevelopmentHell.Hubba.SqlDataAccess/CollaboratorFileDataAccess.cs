using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
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
        private readonly string _fileName = "FileName";
        private readonly string _fileType = "FileType";
        private readonly string _fileSize = "FileSize";
        private readonly string _fileUrl = "FileUrl";
        private readonly string _ownerId = "OwnerId";
        private readonly string _lastModifiedUser = "LastModifiedUser";
        private readonly string _createDate = "CreateDate";
        private readonly string _fileId = "FileId";
        private readonly string _pfpFile = "PfpFile";
        private readonly string _updateDate = "UpdateDate";


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
                    { _fileName, file.Name},
                    { _fileType, Path.GetExtension(file.FileName).Substring(1)},
                    { _fileSize, file.Length},
                    { _fileUrl, fileUrl},
                    { _ownerId, accountIdInt},
                    { _lastModifiedUser, accountIdInt},
                    { _createDate, DateTime.Now}
                },
                _fileId
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
            result.Payload = (int)(insertResult.Payload[0][_fileId]);
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
                    new Comparator(_fileId, "=", fileId),
                },
                new Dictionary<string, object>()
                {
                    { _fileUrl, fileUrl},
                    { _lastModifiedUser, accountIdInt},
                    { _updateDate, DateTime.Now}
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
                new List<String>() { _fileUrl },
                _fileId,
                fileIdsString                
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                return new(Result.Failure("" + selectResult.ErrorMessage));
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 11)
            {
                return new(Result.Failure($"Selected more than the valid number of files: {payload.Count}" + selectResult.ErrorMessage));
            }

            List<String> fileUrls = new List<string>();

            foreach (var row in payload)
            {
                fileUrls.Add((string)row[_fileUrl]);
            }

            return new Result<List<string>>()
            {
                IsSuccessful = true,
                Payload = fileUrls
            };
        }

        public async Task<Result<List<int>>> SelectFileIdsFromUrl(string[] fileUrls)
        {
            List<string> fileUrlsString = new List<string>();
            foreach (string fileUrl in fileUrls)
            {
                fileUrlsString.Add(fileUrl);
            }

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.SelectWhereIn(
                _tableName,
                new List<String>() { _fileId },
                _fileUrl,
                fileUrlsString
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                return new(Result.Failure("" + selectResult.ErrorMessage));
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 11)
            {
                return new(Result.Failure($"Selected more than the valid number of files: {payload.Count}" + selectResult.ErrorMessage));
            }

            List<int> fileIds = new List<int>();

            foreach (var row in payload)
            {
                fileIds.Add((int)row[_fileId]);
            }

            return new Result<List<int>>()
            {
                IsSuccessful = true,
                Payload = fileIds
            };
        }

        public async Task<Result<List<int>>> SelectFileIdsFromOwner(int ownerId)
        {
            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<String>() { _fileId },
                new List<Comparator>()
                {
                    new Comparator(_ownerId, "=", ownerId)
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                return new(Result.Failure("" + selectResult.ErrorMessage));
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 11)
            {
                return new(Result.Failure($"Selected more than the valid number of files: {payload.Count}"));
            }

            List<int> fileIds = new List<int>();

            foreach (var row in payload)
            {
                fileIds.Add((int)row[_fileId]);
            }

            return new Result<List<int>>()
            {
                IsSuccessful = true,
                Payload = fileIds
            };
        }

        public async Task<Result<string>> SelectFileExtension(int fileId)
        {
            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<String>() { _fileType },
                new List<Comparator>()
                {
                    new Comparator(_fileId, "=", fileId)
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                return new(Result.Failure("" + selectResult.ErrorMessage));
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 1)
            {
                return new(Result.Failure($"Selected more than the valid number of files: {payload.Count}"));
            }
            return new Result<string>()
            {
                IsSuccessful = true,
                Payload = $".{(string)payload[0][_fileType]}"
            };
        }

        public async Task<Result<string?>> SelectPfpUrl(int ownerId)
        {
            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<String>() { _fileUrl },
                new List<Comparator>()
                {
                    new Comparator(_ownerId, "=", ownerId),
                    new Comparator(_fileName, "=", _pfpFile)
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                return new(Result.Failure("" + selectResult.ErrorMessage));
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 1)
            {
                return new(Result.Failure($"Selected more than the valid number of files: {payload.Count}"));
            }

            if (payload.Count == 0)
            {
                return new Result<string?>()
                {
                    IsSuccessful = true,
                    Payload = null
                };
            }

            return new Result<string?>()
            {
                IsSuccessful = true,
                Payload = (string)payload[0][_fileUrl]
            };
        }

        public async Task<Result> DeleteFilesFromUrl(string[] removedFileUrls)
        {
            if (removedFileUrls.Length == 0)
                return new Result() { IsSuccessful = true };
            List<string> removedFileUrlsList = removedFileUrls.ToList<string>();
            var deleteResult = await _deleteDataAccess.DeleteWhereIn(_tableName, _fileUrl, removedFileUrlsList).ConfigureAwait(false);

            return deleteResult;
        }

        public async Task<Result> DeleteFilesFromFileId(List<int> fileIds)
        {
            if (fileIds.Count == 0)
                return new Result() { IsSuccessful = true };
            List<string> fileIdsString = new List<string>();
            foreach (var fileId in fileIds)
            {
                fileIdsString.Add(fileId.ToString());
            }

            var deleteResult = await _deleteDataAccess.DeleteWhereIn(_tableName, _fileId, fileIdsString).ConfigureAwait(false);
            return deleteResult;
        }

        public async Task<Result> DeleteFilesFromOwnerId(int ownerId)
        {
            List<string> ownerIdString = new List<string>();
            ownerIdString.Add(ownerId.ToString());
            var deleteResult = await _deleteDataAccess.DeleteWhereIn(_tableName, _ownerId, ownerIdString).ConfigureAwait(false);
            return deleteResult;
        }
    }
}

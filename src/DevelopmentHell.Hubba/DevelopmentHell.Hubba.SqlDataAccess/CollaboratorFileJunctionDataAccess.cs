using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class CollaboratorFileJunctionDataAccess : ICollaboratorFileJunctionDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private string _tableName;
        private string _fileTableName = "Files";
        private string _fileId = "FileId";
        private string _ownerid = "OwnerId";
        private string _collaboratorId = "CollaboratorId";
        private string _fileUrl = "FileUrl";

        public CollaboratorFileJunctionDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _tableName = tableName;
        }

        public async Task<Result> InsertCollaboratorFile(int collabId, int fileId)
        {
            Result insertResult = await _insertDataAccess.Insert(
               _tableName,
               new Dictionary<string, object>()
               {
                    { _collaboratorId, collabId },
                    { _fileId,fileId }
               }
           ).ConfigureAwait(false);

            return insertResult;
        }

        public async Task<Result<List<int>>> SelectFileIdsFromCollabId(int collabId)
        {
            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<String>() { _fileId },
                new List<Comparator>() {
                    new Comparator(_collaboratorId,"=", collabId)
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
                fileIds.Add((int)row[_fileId]);
            }
            return new Result<List<int>>()
            {
                IsSuccessful = true,
                Payload = fileIds
            };
        }

        public async Task<Result<List<string>>> SelectFileUrlsFromCollabId(int collabId)
        {
            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.SelectInnerJoin(
                new List<String>() { _fileUrl },
                new List<Comparator>() {
                    new Comparator(_collaboratorId,"=", collabId)
                },
                _fileTableName,
                _tableName,
                _fileId,
                _fileId
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

            List<string> fileUrls = new List<string>();

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

        public async Task<Result<List<string>>> SelectFileUrlsFromOwnerId(int ownerId)
        {
            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.SelectInnerJoin(
                new List<String>() { _fileUrl },
                new List<Comparator>() {
                    new Comparator(_ownerid,"=", ownerId)
                },
                _fileTableName,
                _tableName,
                _fileId,
                _fileId
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

            List<string> fileUrls = new List<string>();

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
    }
}

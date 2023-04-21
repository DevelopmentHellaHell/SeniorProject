using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class CollaboratorFileJunctionDataAccess : ICollaboratorFileJunctionDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private string _tableName;

        public CollaboratorFileJunctionDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _tableName = tableName;
        }

        public async Task<Result> InsertCollaboratorFile(int collabId, int fileId)
        {
            Result insertResult = await _insertDataAccess.Insert(
               _tableName,
               new Dictionary<string, object>()
               {
                    { "CollaboratorId", collabId },
                    { "FileId",fileId }
               }
           ).ConfigureAwait(false);

            return insertResult;
        }

        public async Task<Result<List<int>>> SelectFiles(int collabId)
        {
            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<String>() { "FileId" },
                new List<Comparator>() { 
                    new Comparator("CollaboratorId","=", collabId) 
                }
            ).ConfigureAwait(false);
            
            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                return new (Result.Failure("" + selectResult.ErrorMessage));
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
    }
}

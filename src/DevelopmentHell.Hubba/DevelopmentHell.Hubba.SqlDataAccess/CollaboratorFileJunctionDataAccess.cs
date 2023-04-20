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
    }
}

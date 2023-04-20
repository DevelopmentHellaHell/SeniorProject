using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class CollaboratorDataAccess : ICollaboratorDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private string _tableName;

        public CollaboratorDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _tableName = tableName;
        }

        public async Task<Result> Delete(int collabId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<CollaboratorProfile>> GetCollaborator(int collabId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<int>> GetCollaboratorId(int ownerId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<int>> GetOwnerId(int collabId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> GetPublished(int collabId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<int>> CreateCollaborator(CollaboratorProfile collab)
        {
            var result = new Result<int>();

            // Get the ID of current thread
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

            //TODO: Create file objects

            var insertDict = new Dictionary<string, object>()
                {
                    { "Name", collab.Name!},
                    { "ContactInfo", collab.ContactInfo!},
                    { "Description", collab.Description!},
                    { "Availability", collab.Availability!},
                    { "OwnerId", accountIdInt},
                    { "LastModifiedUser", accountIdInt},
                    { "CreateDate", DateTime.Now},
                    { "Published", collab.Published}
                };
            if(collab.PfpUrl != null)
            {
                insertDict["ProfilePicture"] = collab.PfpUrl;
            }
            if (collab.Tags != null)
            {
                insertDict["Tags"] = collab.Tags;
            }
            if (collab.Availability != null)
            {
                insertDict["Availability"] = collab.Availability;
            }

            var insertResult = await _insertDataAccess.InsertWithOutput(
                _tableName,
                insertDict,
                "CollaboratorId"
            ).ConfigureAwait(false);

            if (!insertResult.IsSuccessful || insertResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = insertResult.ErrorMessage;
                return result;
            }
            if(insertResult.Payload.Count != 1)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = string.Format("Unexpected number of collaborators created. Number was {0}", insertResult.Payload.Count);
                return result;
            }
            result.IsSuccessful = true;
            result.Payload = (int)(insertResult.Payload[0]["CollaboratorId"]);
            return result;
        }

        public async Task<Result> SetPublished(int collabId, bool published)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> Update(CollaboratorProfile collab)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> UpdatePfpFileId(int collabId, int pfpFileId)
        {

            var values = new Dictionary<string, object>();
            values["ProfilePicture"] = pfpFileId;

            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                // comparator helps create WHERE SQL statement
                new List<Comparator>()
                {
                    new Comparator("CollaboratorId", "=", collabId),
                },
                values
            ).ConfigureAwait(false);

            return updateResult;
        }

        public async Task<Result> Remove(CollaboratorProfile collab)
        {
            throw new NotImplementedException();
        }
    }
}

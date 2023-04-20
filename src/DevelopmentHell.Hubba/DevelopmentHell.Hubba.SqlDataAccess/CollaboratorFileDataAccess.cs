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
        public async Task<Result<int>> InsertFileWithOutput(string fileUrl, IFormFile file)
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

    }
}

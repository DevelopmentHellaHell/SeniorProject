﻿using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public class CollaboratorsDataAccess : ICollaboratorsDataAccess
	{
		private readonly ExecuteDataAccess _executeDataAccess;
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private string _tableName;

		public CollaboratorsDataAccess(string connectionString, string tableName)
		{
			_executeDataAccess = new ExecuteDataAccess(connectionString);
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

        public async Task<Result<CollaboratorProfile>> GetCollaboratorWithPfpId(int collabId)
        {
            using (SqlCommand insertQuery = new SqlCommand())
            {
                bool first = true;
                StringBuilder sbFilter = new();
                StringBuilder sbColumn = new();

                var columns = new List<String>(){
                    "Name",
                    "ProfilePicture",
                    "ContactInfo",
                    "Tags",
                    "Description",
                    "Availability",
                    "Published"
                };

                Comparator filter = new Comparator("CollaboratorId", "=", collabId);
                sbFilter.Append($"c.{filter.Key} {filter.Op} @{filter.Key}");

                insertQuery.Parameters.Add(new SqlParameter(filter.Key.ToString(), filter.Value.ToString()));

                first = true;
                foreach (string column in columns)
                {
                    if (!first)
                    {
                        sbColumn.Append(", ");
                    }
                    first = false;
                    sbColumn.Append("c."+column);
                }

                insertQuery.CommandText = $"SELECT {sbColumn}, COUNT(uv.CollaboratorId) as Votes " +
                    $"FROM Collaborators as c " +
                    $"LEFT JOIN UserVotes uv on c.CollaboratorId = uv.CollaboratorId " +
                    $"WHERE {sbFilter} " +
                    $"GROUP BY {sbColumn}";
                
                var queryResult = await SendQuery(insertQuery).ConfigureAwait(false);

                if(queryResult.Payload == null)
                {
                    return new(Result.Failure("Server error: " + queryResult.ErrorMessage));
                }
                List<Dictionary<string, object>> payload = queryResult.Payload!;

                if (payload.Count > 1)
                {
                    return new(Result.Failure("Invalid number of Collaborators selected", StatusCodes.Status500InternalServerError));
                }

                var collab = new CollaboratorProfile()
                {
                    Name = (string)payload[0]["Name"],
                    ContactInfo = (string)payload[0]["ContactInfo"],
                    Tags = (string)payload[0]["Tags"],
                    Description = (string)payload[0]["Description"],
                    Availability = (string)payload[0]["Availability"],
                    Votes = (int)payload[0]["Votes"],
                    CollabUrls = new List<string>(),
                    Published = (bool)payload[0]["Published"]
                };

                if (payload[0]["ProfilePicture"] != null )
                {
                    collab.PfpUrl = ((int)payload[0]["ProfilePicture"]).ToString();
                }

                return new Result<CollaboratorProfile>()
                {
                    IsSuccessful = true,
                    Payload =collab
                };
            }
        }

        public async Task<Result<int>> GetCollaboratorId(int ownerId)
        {
            Result<int> result = new Result<int>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "CollaboratorId" },
                new List<Comparator>()
                {
                    new Comparator("OwnerId", "=", ownerId)
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 1)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid number of Collaborators selected.";
                return result;
            }

            result.IsSuccessful = true;
            if (payload.Count > 0) result.Payload = (int)payload[0]["CollaboratorId"];
            return result;
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
            if (collab.PfpUrl != null)
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
            if (insertResult.Payload.Count != 1)
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

        public async Task<Result> Update(int collabId, CollaboratorProfile collab)
        {
            Result result = new Result();

            // Get the ID of current thread
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string accountIDStr = principal.FindFirstValue("sub");
            int.TryParse(accountIDStr, out int accountIdInt);

            var updateDict = new Dictionary<string, object>()
            {
                { "Name", collab.Name!},
                { "ContactInfo", collab.ContactInfo!},
                { "Description", collab.Description!},
                { "Published", collab.Published},
                { "LastModifiedUser", accountIdInt},
                { "UpdateDate", DateTime.Now}
            };
            if (collab.Tags != null)
            {
                updateDict["Tags"] = collab.Tags;
            }
            if (collab.Availability != null)
            {
                updateDict["Availability"] = collab.Availability;
            }
            var updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("CollaboratorId", "=", collabId),
                },
                updateDict
            ).ConfigureAwait(false);

            // check if the update was successful
            if (!updateResult.IsSuccessful)
            {
                result.ErrorMessage = updateResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> UpdatePfpFileId(int collabId, int? pfpFileId = null)
        {

            var values = new Dictionary<string, object?>();
            if (pfpFileId == null)
            {
                values["ProfilePicture"] = null;
            }
            else
            {
                values["ProfilePicture"] = pfpFileId;
            }

            Result updateResult = await _updateDataAccess.UpdateAllowNull(
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

        public async Task<Result<List<Dictionary<string, object>>>> Curate(int offset = 0)
		{
			var result = await _executeDataAccess.Execute("CurateCollaborators", new Dictionary<string, object>() {
				{ "Offset", offset },
			}).ConfigureAwait(false);

			return result;
		}

		public async Task<Result<List<Dictionary<string, object>>>> Search(string query, int offset = 0, double FTTWeight = 0.5, double VCWeight = 0.5)
		{
			var result = await _executeDataAccess.Execute("SearchCollaborators", new Dictionary<string, object>()
			{
				{ "Query", query },
				{ "Offset", offset },
				{ "FTTableRankWeight", FTTWeight },
				{ "VotesCountRankWeight", VCWeight },
			}).ConfigureAwait(false);

			return result;
		}


        private async Task<Result<List<Dictionary<string, object>>>> SendQuery(SqlCommand query)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!))
                {
                    query.Connection = conn;
                    List<Dictionary<string, object>> payload = new();
                    await conn.OpenAsync().ConfigureAwait(false);
                    using (SqlDataReader reader = await query.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> nextLine = new();
                            IDataRecord dataRecord = reader;
                            for (int i = 0; i < dataRecord.FieldCount; i++)
                            {
                                nextLine.Add(reader.GetName(i), dataRecord.GetValue(i));
                            }
                            payload.Add(nextLine);
                        }
                        return new Result<List<Dictionary<string, object>>>()
                        {
                            IsSuccessful = true,
                            Payload = payload,
                        };
                    }
                }

            }
            catch (Exception e)
            {
                return new Result<List<Dictionary<string, object>>>()
                {
                    IsSuccessful = false,
                    ErrorMessage = e.Message,
                };
            }
        }
    }
}

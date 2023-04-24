﻿using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class CollaboratorUserVoteDataAccess : ICollaboratorUserVoteDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private string _tableName;
        private readonly string _collaboratorIdColumn = "CollaboratorId";
        private readonly string _accountIdColumn = "AccountId";

        public CollaboratorUserVoteDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _tableName = tableName;
        }
        public async Task<Result> Downvote(int collabId, int accountId)
        {
            var deleteUpvoteResult = await _deleteDataAccess.Delete(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator(_collaboratorIdColumn, "=", collabId),
                    new Comparator(_accountIdColumn, "=", accountId)
                }
            ).ConfigureAwait(false);
            return new Result(deleteUpvoteResult);
        }

        public async Task<Result> Upvote(int collabId, int accountId)
        {
            var selectUpvoteResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { _accountIdColumn },
                new List<Comparator>()
                { 
                    new Comparator(_collaboratorIdColumn, "=", collabId),
                    new Comparator(_accountIdColumn, "=", accountId) 
                }
            ).ConfigureAwait(false);
            if (!selectUpvoteResult.IsSuccessful)
            {
                return selectUpvoteResult;
            }
            if(selectUpvoteResult.Payload!.Count > 1)
            {
                return new(Result.Failure("User has already voted multiple times. "));
            }
            else if(selectUpvoteResult.Payload!.Count == 1)
            {
                return new Result() { IsSuccessful = true };
            }
            var insertUpvoteResult = await _insertDataAccess.Insert(
                    _tableName,
                    new Dictionary<string, object>()
                    {
                        {_collaboratorIdColumn,  collabId},
                        {_accountIdColumn,  accountId}
                    }).ConfigureAwait(false);
            if(!insertUpvoteResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to insert vote. " + insertUpvoteResult.ErrorMessage));
            }

            return new Result() { IsSuccessful = true };
        }
    }
}
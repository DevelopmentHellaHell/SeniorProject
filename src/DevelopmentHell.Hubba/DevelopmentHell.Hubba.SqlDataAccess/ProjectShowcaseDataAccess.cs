using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class ProjectShowcaseDataAccess : IProjectShowcaseDataAccess
    {
        private readonly string _connectionString;
        private readonly string _showcaseTableName;
        private readonly string _commentTableName;
        private readonly string _showcaseLikeTableName;
        private readonly string _commentLikeTableName;
        private readonly string _showcaseReportTableName;
        private readonly string _commentReportTableName;

        private readonly string _userAccountsTableName;

        private readonly InsertDataAccess _insertDataAccess;
        private readonly UpdateDataAccess _updateDataAccess;
        private readonly SelectDataAccess _selectDataAccess;
        private readonly DeleteDataAccess _deleteDataAccess;


        ProjectShowcaseDataAccess(
            string connectionString, 
            string showcaseTableName,
            string commentTableName,
            string showcaseLikeTableName,
            string commentLikeTableName,
            string showcaseReportTableName,
            string commentReportTableName,
            string userAccountsTableName)
        {
            _connectionString = connectionString;
            _showcaseTableName = showcaseTableName;
            _commentTableName = commentTableName;
            _showcaseLikeTableName = showcaseLikeTableName;
            _commentLikeTableName = commentLikeTableName;
            _showcaseReportTableName = showcaseReportTableName;
            _commentReportTableName = commentReportTableName;

            _userAccountsTableName = userAccountsTableName;


            _insertDataAccess = new InsertDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
        }

        public async Task<Result> AddComment(string showcaseId, int accountId, string commentText, DateTime time)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> AddCommentReport(int commentId, int reporterId, string reason, DateTime time)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> AddShowcaseReport(string showcaseId, int reporterId, string reason, DateTime time)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> ChangePublishStatus(string showcaseId, bool isPublished, DateTime time)
        {
            var updateResult = await _updateDataAccess.Update
            (
                _showcaseTableName, 
                new List<Comparator>() { new("Id", "=", showcaseId) }, 
                new Dictionary<string, object>() 
                { 
                    { "IsPublished", isPublished }, 
                    { "PublishTimestamp", time } 
                }
            );
            if (!updateResult.IsSuccessful)
            {
                return Result.Failure("Failed to change publish status"); 
            }
            return new()
            {
                IsSuccessful = true
            };
        }

        public async Task<Result> DeleteComment(int commentId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> DeleteShowcase(string showcaseId)
        {
            var deleteResult = await _deleteDataAccess.Delete(_showcaseTableName, new() { new("Id", "=", showcaseId) });
            if (!deleteResult.IsSuccessful)
            {
                return new() {
                    IsSuccessful = false, 
                    ErrorMessage = "Failed to delete showcase"
                };
            }

            return new()
            {
                IsSuccessful = true
            };
        }

        public async Task<Result> EditShowcase(string showcaseId, string? title, string? description, DateTime time)
        {
            Dictionary<string, object> updateDict = new();
            if(title != null)
            {
                updateDict["Title"] = title;
            }
            if(description != null)
            {
                updateDict["Description"] = description;
            }

            return await _updateDataAccess.Update(_showcaseTableName, new List<Comparator>() { new("Id", "=", showcaseId!) }, updateDict);
        }

        public async Task<Result<Dictionary<string, object>>> GetCommentDetails(string showcaseId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<Dictionary<string, object>>>> GetComments(string showcaseId, int commentCount, int page)
        {
            //SELECT TOP 10 ShowcaseComments.Id, Email, Text, Rating, Timestamp<br/>FROM ShowcaseComments INNER JOIN UserAccounts<br/>ON ShowcaseComments.CommenterId=UserAccounts.Id<br/>WHERE ShowcaseId={showcaseId} ORDER BY Timestamp<br/>OFFSET {{page-1}*{commentCount}-1} ROWS
            var result = await _selectDataAccess.Select
            (
                SQLManip.InnerJoinTables(
                    new Joiner(_commentTableName,_userAccountsTableName,"CommenterId","Id")
                ),
                new() { $"{_commentTableName}.Id", "Email", "Text", "Rating", "Timestamp", "EditTimestamp" },
                new() { new("ShowcaseId", "=", showcaseId) },
                "",
                "Timestamp",
                commentCount,
                (page - 1) * commentCount - 1 // need to offset by the number I want to start at -1, and no offset for page 1, hence the expression
            ).ConfigureAwait(false);

            if (!result.IsSuccessful)
            {
                return new(Result.Failure($"Error in getting showcase comments from Db: {result.ErrorMessage}"));
            }
            return result;
        }

        public async Task<Result<Dictionary<string, object>>> GetCommentUserRating(int commentId, int voterId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<Dictionary<string, object>>> GetDetails(string showcaseId)
        {
            var result = await _selectDataAccess.Select
            (
                SQLManip.InnerJoinTables(
                    new Joiner(_showcaseTableName, _userAccountsTableName, "ShowcaseUserId", "Id")
                ),
                new() { $"{_showcaseTableName}.Id", "Email", "ShowcaseUserId", $"{_userAccountsTableName}.Id", "ListingId", "Title", "IsPublished", "Rating" },
                new() { new($"{_showcaseTableName}.Id", "=", showcaseId) }
            ).ConfigureAwait(false);
            if (!result.IsSuccessful)
            {
                return new(Result.Failure($"Error in getting showcase details from Db: {result.ErrorMessage}"));
            }
            if (result.Payload!.Count() > 1)
            {
                return new(Result.Failure($"More than one showcase with given Id: {result.ErrorMessage}"));
            }
            if (result.Payload!.Count() == 0)
            {
                return new(Result.Failure($"Id Does not exist: {result.ErrorMessage}"));
            }
            return new() { IsSuccessful = true, Payload = result.Payload![0] };
        }

        public async Task<Result<Dictionary<string, object>>> GetShowcase(string showcaseId)
        {
            var result = await _selectDataAccess.Select
            (
                SQLManip.InnerJoinTables(
                    new Joiner(_showcaseTableName,_userAccountsTableName,"ShowcaseUserId","Id")
                ),
                new() { $"{_showcaseTableName}.Id", "Email", "ShowcaseUserId", $"{_userAccountsTableName}.Id", "ListingId", "Title", "Description", "IsPublished", "Rating"}, 
                new() { new($"{_showcaseTableName}.Id","=",showcaseId) }
            ).ConfigureAwait(false);
            if (!result.IsSuccessful)
            {
                return new(Result.Failure($"Error in getting showcase from Db: {result.ErrorMessage}"));
            }
            if (result.Payload!.Count() > 1)
            {
                return new(Result.Failure($"More than one showcase with given Id: {result.ErrorMessage}"));
            }
            if (result.Payload!.Count() == 0)
            {
                return new(Result.Failure($"Id Does not exist: {result.ErrorMessage}"));
            }
            return Result<Dictionary<string, object>>.Success(result.Payload![0]);
        }

        public async Task<Result<float>> IncrementShowcaseLikes(string showcaseId)
        {
            var updateResult = await _updateDataAccess.Update(_showcaseTableName, new() { new("Id", "=", showcaseId) }, new() { { "Rating", "Rating+1" } } );
            if (!updateResult.IsSuccessful)
            {
                return new(Result.Failure($"Unable to update showcase rating: {updateResult.ErrorMessage}"));
            }

            var selectResult = await _selectDataAccess.Select(_showcaseTableName, new() { "Rating" }, new() { new("Id", "=", showcaseId) }).ConfigureAwait(false);
            if (!selectResult.IsSuccessful)
            {
                return new(Result.Failure($"Unable to select updated showcase rating: {selectResult.ErrorMessage}"));
            }
            
            if (selectResult.Payload!.Count() == 0)
            {
                return new(Result.Failure($"Unable to find rating for showcase {showcaseId} in Db"));
            }
            if (selectResult.Payload!.Count() > 1)
            {
                return new(Result.Failure($"Retrieved more than one rating for showcase {showcaseId} in Db"));
            }

            return Result<float>.Success((float)selectResult.Payload![0]["Rating"]);
        }

        public async Task<Result> InsertShowcase(int accountId, string showcaseId, int listingId, string title, string description, DateTime time)
        {
            return await _insertDataAccess.Insert(_showcaseTableName, new() {
                { "Id", showcaseId },
                { "Title", title },
                { "Description", description },
                { "IsPublished", false },
                { "Rating", 0 },
                { "EditTimestamp", time }
            }).ConfigureAwait(false);
        }

        public async Task<Result> InsertUserCommentRating(int commentId, int voterId, int isUpvote)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> RecordUserLike(int raterId, string showcaseId)
        {
            var insertResult = await _insertDataAccess.Insert(_showcaseLikeTableName, new() 
            { 
                { "UserAccountId", raterId }, 
                { "ShowcaseId", showcaseId } 
            }).ConfigureAwait(false);
            return insertResult;
        }

        public async Task<Result> RemoveShowcaseListing(string showcaseId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> UpdateComment(int commentId, string commentText, DateTime time)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> UpdateCommentRating(int commentId, int difference)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> UpdateUserCommentRating(int commentId, int voterId, int isUpvote)
        {
            throw new NotImplementedException();
        }
    }
}

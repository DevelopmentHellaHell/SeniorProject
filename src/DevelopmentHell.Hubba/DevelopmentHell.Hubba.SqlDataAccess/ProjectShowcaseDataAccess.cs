using Azure;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class ProjectShowcaseDataAccess : IProjectShowcaseDataAccess
    {
        private readonly string _showcaseTableName;
        private readonly string _commentTableName;
        private readonly string _showcaseLikeTableName;
        private readonly string _commentLikeTableName;
        private readonly string _showcaseReportTableName;
        private readonly string _commentReportTableName;

		private readonly ExecuteDataAccess _executeDataAccess;
		private readonly InsertDataAccess _insertDataAccess;
        private readonly UpdateDataAccess _updateDataAccess;
        private readonly SelectDataAccess _selectDataAccess;
        private readonly DeleteDataAccess _deleteDataAccess;


        public ProjectShowcaseDataAccess(
            string connectionString,
            string showcaseTableName,
            string commentTableName,
            string showcaseLikeTableName,
            string commentLikeTableName,
            string showcaseReportTableName,
            string commentReportTableName)
        {
            _showcaseTableName = showcaseTableName;
            _commentTableName = commentTableName;
            _showcaseLikeTableName = showcaseLikeTableName;
            _commentLikeTableName = commentLikeTableName;
            _showcaseReportTableName = showcaseReportTableName;
            _commentReportTableName = commentReportTableName;

			_executeDataAccess = new ExecuteDataAccess(connectionString);
			_insertDataAccess = new InsertDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
        }

		public async Task<Result<List<Dictionary<string, object>>>> Curate(int offset = 0)
		{
			var result = await _executeDataAccess.Execute("CurateShowcases", new Dictionary<string, object>() {
				{ "Offset", offset },
			}).ConfigureAwait(false);

			return result;
		}

		public async Task<Result<List<Dictionary<string, object>>>> Search(string query, int offset = 0, double FTTWeight = 0.5, double RWeight = 0.5)
		{
			var result = await _executeDataAccess.Execute("SearchShowcases", new Dictionary<string, object>()
			{
				{ "Query", query },
				{ "Offset", offset },
				{ "FTTableRankWeight", FTTWeight },
				{ "RatingsRankWeight", RWeight },
			}).ConfigureAwait(false);

			return result;
		}

		public async Task<Result> AddComment(string showcaseId, int accountId, string commentText, DateTime time)
        {
            if (showcaseId == null || showcaseId.Length == 0)
            {
                return Result.Failure("Showcase Id is not valid", 400);
            }
            if (commentText == null || commentText.Length == 0)
            {
                return Result.Failure("Comment is not valid", 400);
            }

            return await _insertDataAccess.Insert(_commentTableName, new() {
                { "ShowcaseId", showcaseId },
                { "CommenterId", accountId },
                { "Text", commentText },
                { "Rating", 0 },
                { "Timestamp", time }
            }).ConfigureAwait(false);
        }

        public async Task<Result> AddCommentReport(long commentId, int reporterId, string reason, DateTime time)
        {
            if (commentId < 0)
            {
                return Result.Failure("Comment Id is not valid", 400);
            }
            if (reporterId < 0)
            {
                return Result.Failure("Reporter Id is not valid", 400);
            }

            return await _insertDataAccess.Insert(_commentReportTableName, new()
            {
                { "CommentId", commentId },
                { "ReporterId", reporterId },
                { "Reason", reason },
                { "Timestamp", time },
                { "IsResolved", false },
            }).ConfigureAwait(false);
        }

        public async Task<Result> AddShowcaseReport(string showcaseId, int reporterId, string reason, DateTime time)
        {
            if (showcaseId == null || showcaseId.Length == 0)
            {
                return Result.Failure("Showcase Id is not valid", 400);
            }
            if (reporterId < 0)
            {
                return Result.Failure("Reporter Id is not valid", 400);
            }

            return await _insertDataAccess.Insert(_showcaseReportTableName, new()
            {
                { "ShowcaseId", showcaseId },
                { "ReporterId", reporterId },
                { "Reason", reason },
                { "Timestamp", time },
                { "IsResolved", false }
            }).ConfigureAwait(false);
        }

        public async Task<Result> ChangePublishStatus(string showcaseId, bool isPublished, DateTime? time = null)
        {
            Dictionary<string, object> updateDict = new() { { "IsPublished", isPublished } };
            if (time != null)
            {
                updateDict.Add("PublishTimestamp", time);
            }
            var updateResult = await _updateDataAccess.Update
            (
                _showcaseTableName,
                new List<Comparator>() { new("Id", "=", showcaseId) },
                updateDict
            );
            if (!updateResult.IsSuccessful)
            {
                return Result.Failure("Failed to change publish status");
            }
            return Result.Success();
        }

        public async Task<Result> DeleteComment(long commentId)
        {
            var deleteResult = await _deleteDataAccess.Delete(_commentTableName, new() { new Comparator("Id", "=", commentId) });
            if (!deleteResult.IsSuccessful)
            {
                return Result.Failure("Failed to delete comment.");
            }

            return Result.Success();
        }

        public async Task<Result> DeleteShowcase(string showcaseId)
        {
            var deleteResult = await _deleteDataAccess.Delete(_showcaseTableName, new() { new("Id", "=", showcaseId) });
            if (!deleteResult.IsSuccessful)
            {
                return Result.Failure("Failed to delete showcase");
            }

            return Result.Success();
        }

        public async Task<Result> EditShowcase(string showcaseId, string? title, string? description, DateTime time)
        {
            if (title == null && description == null)
            {
                return Result.Success();
            }
            Dictionary<string, object> updateDict = new();
            if (title != null)
            {
                updateDict["Title"] = title;
            }
            if (description != null)
            {
                updateDict["Description"] = description;
            }

            return await _updateDataAccess.Update(_showcaseTableName, new List<Comparator>() { new("Id", "=", showcaseId!) }, updateDict);
        }

        public async Task<Result<Dictionary<string, object>>> GetCommentDetails(long commentId)
        {
            var result = await _selectDataAccess.Select
            (
                _commentTableName,
                new() { "Id", "CommenterId", "ShowcaseId", "Timestamp", "Rating", "EditTimestamp" },
                new() { new($"Id", "=", commentId) }
            ).ConfigureAwait(false);

            if (!result.IsSuccessful)
            {
                return new(Result.Failure($"Error in getting comment details from Db: {result.ErrorMessage}"));
            }
            if (result.Payload!.Count() > 1)
            {
                return new(Result.Failure($"More than one comment with given Id: {result.ErrorMessage}"));
            }
            if (result.Payload!.Count() == 0)
            {
                return new(Result.Failure($"Id Does not exist: {result.ErrorMessage}"));
            }
            return new() { IsSuccessful = true, Payload = result.Payload![0] };
        }

        public async Task<Result<List<Dictionary<string, object>>>> GetComments(string showcaseId, int commentCount, int page)
        {
            var result = await _selectDataAccess.Select
            (
                _commentTableName,
                new() { $"Id", "CommenterId", "ShowcaseId", "Text", "Rating", "Timestamp", "EditTimestamp" },
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
            result.Payload = result.Payload!.Take(commentCount).ToList< Dictionary<string, object>>();
            return result;
        }
        public async Task<Result<Dictionary<string, object>>> GetComment(long commentId)
        {
            var result = await _selectDataAccess.Select
            (
                _commentTableName,
                new() { $"Id", "CommenterId", "ShowcaseId", "Text", "Rating", "Timestamp", "EditTimestamp" },
                new() { new("Id", "=", commentId) }
            ).ConfigureAwait(false);

            if (!result.IsSuccessful)
            {
                return new(Result.Failure($"Error in getting showcase comments from Db: {result.ErrorMessage}"));
            }
            if (result.Payload!.Count > 1)
            {
                return new(Result.Failure("More than one row with CommentId"));
            }
            if (result.Payload!.Count == 0)
            {
                return new(Result.Failure("Unable to find comment with Id"));
            }

            return Result<Dictionary<string,object>>.Success(result.Payload![0]);
        }

        public async Task<Result<string>> GetCommentShowcase(long commentId)
        {
            var result = await _selectDataAccess.Select(
                TableManip.InnerJoinTables(_showcaseTableName, _commentTableName, "Id", "ShowcaseId"),
                new() { $"{_showcaseTableName}.Id" },
                new()
                    {
                        new($"{_commentTableName}.Id", "=", commentId)
                    }
                ).ConfigureAwait(false);
            if (!result.IsSuccessful)
            {
                return new(Result.Failure($"Error in getting showcase Id from Db: {result.ErrorMessage}"));
            }
            if (result.Payload!.Count > 1)
            {
                return new(Result.Failure("Database error: more than one comment with given Id"));
            }
            if (result.Payload!.Count == 0)
            {
                return new(Result.Failure("Comment does not exist in database."));
            }
            return Result<string>.Success((string)result.Payload![0]["Id"]);
        }

        public async Task<Result<bool?>> GetCommentUserRating(long commentId, int voterId)
        {
            var result = await _selectDataAccess.Select(
                _commentLikeTableName,
                new() { "IsUpvote" },
                new() {
                    new("CommentId","=",commentId),
                    new("VoterId","=",voterId)
                });
            if (!result.IsSuccessful)
            {
                return new(Result.Failure($"Error in getting User Rating from Db"));
            }
            if (result.Payload!.Count > 1)
            {
                return new(Result.Failure("More than one comment with given Id"));
            }
            if (result.Payload!.Count == 0)
            {
                return Result<bool?>.Success(null);
            }
            return Result<bool?>.Success((bool)result.Payload![0]["IsUpvote"]);
        }

        public async Task<Result<Dictionary<string, object>>> GetDetails(string showcaseId)
        {
            var result = await _selectDataAccess.Select
            (
                _showcaseTableName,
                new() { $"Id", "ShowcaseUserId", "ListingId", "Title", "IsPublished", "Rating", "PublishTimestamp", "EditTimestamp" },
                new() { new($"Id", "=", showcaseId) }
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
                _showcaseTableName,
                new() { $"Id", "ShowcaseUserId", "ListingId", "Title", "IsPublished", "Rating", "PublishTimestamp", "EditTimestamp", "Description" },
                new() { new($"Id", "=", showcaseId) }
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

        public async Task<Result<List<Dictionary<string, object>>>> GetUserShowcases(int userId, bool includeDescription = true)
        {
            List<string> selectList = new() { $"Id", "ShowcaseUserId", "ListingId", "Title", "IsPublished", "Rating", "PublishTimestamp", "EditTimestamp" };
            if (includeDescription)
            {
                selectList.Add("Description");
            }
            var result = await _selectDataAccess.Select
            (
                _showcaseTableName,
                selectList,
                new() { new($"ShowcaseUserId", "=", userId) }
            ).ConfigureAwait(false);
            if (!result.IsSuccessful)
            {
                return new(Result.Failure("Unknown error occured while trying to retrieve User's showcases from Db"));
            }
            return Result<List<Dictionary<string, object>>>.Success(result.Payload!);
        }

        public async Task<Result<List<Dictionary<string, object>>>> GetListingShowcases(int listingId)
        {
            List<string> selectList = new() { $"Id", "ShowcaseUserId", "ListingId", "Title", "IsPublished", "Rating", "PublishTimestamp", "EditTimestamp" };
            var result = await _selectDataAccess.Select
            (
                _showcaseTableName,
                selectList,
                new() { new($"ListingId", "=", listingId) }
            ).ConfigureAwait(false);
            if (!result.IsSuccessful)
            {
                return new(Result.Failure("Unknown error occured while trying to retrieve Listing's showcases from Db"));
            }
            return Result<List<Dictionary<string, object>>>.Success(result.Payload!);
        }

        public async Task<Result<List<Dictionary<string, object>>>> GetAllShowcaseReports()
        {
            List<string> selectList = new() { "Timestamp", "ShowcaseId", "ReporterId", "IsResolved", "Reason" };
            var result = await _selectDataAccess.Select
            (
                _showcaseReportTableName,
                selectList,
                new() { new("1", "=", "1") }
            );
            return Result<List<Dictionary<string, object>>>.Success(result.Payload!);
        }

        public async Task<Result<List<Dictionary<string, object>>>> GetShowcaseReports(string showcaseId)
        {
            List<string> selectList = new() { "Timestamp", "ShowcaseId", "ReporterId", "IsResolved", "Reason" };
            var result = await _selectDataAccess.Select
            (
                _showcaseReportTableName,
                selectList,
                new() { new("ShowcaseId", "=", showcaseId) }
            );
            return Result<List<Dictionary<string, object>>>.Success(result.Payload!);
        }

        public async Task<Result<List<Dictionary<string, object>>>> GetAllCommentReports()
        {
            List<string> selectList = new() { "Timestamp", "CommentId", "ReporterId", "IsResolved", "Reason" };
            var result = await _selectDataAccess.Select
            (
                _commentReportTableName,
                selectList,
                new() { new("1", "=", "1") }
            );
            if (!result.IsSuccessful)
            {
                return new(Result.Failure(result.ErrorMessage!));
            }
            return Result<List<Dictionary<string, object>>>.Success(result.Payload!);
        }

        public async Task<Result<List<Dictionary<string, object>>>> GetCommentReports(long commentId)
        {
            List<string> selectList = new() { "Timestamp", "CommentId", "ReporterId", "IsResolved", "Reason" };
            var result = await _selectDataAccess.Select
            (
                _commentReportTableName,
                selectList,
                new() { new("CommentId", "=", commentId) }
            );
            if (!result.IsSuccessful)
            {
                return new(Result.Failure(result.ErrorMessage!));
            }
            return Result<List<Dictionary<string, object>>>.Success(result.Payload!);
        }

        public async Task<Result<double>> IncrementShowcaseLikes(string showcaseId)
        {
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
            var updateResult = await _updateDataAccess.Update(_showcaseTableName, new() { new("Id", "=", showcaseId) }, new() { { "Rating", (double)selectResult.Payload![0]["Rating"] + 1 } });
            if (!updateResult.IsSuccessful)
            {
                return new(Result.Failure($"Unable to update showcase rating: {updateResult.ErrorMessage}"));
            }

            return Result<double>.Success((double)selectResult.Payload![0]["Rating"] + 1);
        }

        public async Task<Result> InsertShowcase(int accountId, string showcaseId, int? listingId, string title, string description, DateTime time)
        {
            Dictionary<string, object> insertDict = new Dictionary<string, object>()
            {
                { "Id", showcaseId },
                { "ShowcaseUserId", accountId },
                { "Title", title },
                { "Description", description },
                { "IsPublished", false },
                { "Rating", 0 },
                { "EditTimestamp", time },
            };
            if (listingId != null && listingId != 0)
            {
                insertDict["ListingId"] = listingId;
            }
            return await _insertDataAccess.Insert(_showcaseTableName, insertDict).ConfigureAwait(false);
        }

        public async Task<Result> InsertUserCommentRating(long commentId, int voterId, bool isUpvote)
        {
            return await _insertDataAccess.Insert(_commentLikeTableName, new()
            {
                { "VoterId", voterId },
                { "CommentId", commentId },
                { "IsUpvote", isUpvote }
            }).ConfigureAwait(false);
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
            var updateResult = await _updateDataAccess.Update(_showcaseTableName, new()
            {
                new("Id","=",showcaseId)
            },
            new() {
                { "ListingId", DBNull.Value }
            }).ConfigureAwait(false);
            return updateResult;
        }
        public async Task<Result> LinkShowcaseListing(string showcaseId, int listingId)
        {
            var updateResult = await _updateDataAccess.Update(_showcaseTableName, new()
            {
                new("Id","=",showcaseId)
            },
            new() {
                { "ListingId", listingId }
            }).ConfigureAwait(false);
            return updateResult;
        }

        public async Task<Result> UpdateComment(long commentId, string commentText, DateTime time)
        {
            return await _updateDataAccess.Update(_commentTableName, new()
            { new Comparator("Id","=",commentId) },
            new()
            {
                { "Text", commentText },
                { "EditTimestamp", time }
            }).ConfigureAwait(false);
        }

        public async Task<Result<int>> UpdateCommentRating(long commentId, int difference)
        {
            var selectResult = await _selectDataAccess.Select(_commentTableName, new()
            { "Rating" },
            new()
            {
                new("Id","=",commentId)
            });
            if (!selectResult.IsSuccessful)
            {
                return new(selectResult);
            }
            if (selectResult.Payload!.Count() > 1)
            {
                return new(Result.Failure($"More than one comment with given Id: {selectResult.ErrorMessage}"));
            }
            if (selectResult.Payload!.Count() == 0)
            {
                return new(Result.Failure($"Comment Id Does not exist: {selectResult.ErrorMessage}"));
            }

            var updateResult = await _updateDataAccess.Update(_commentTableName, new()
            { new Comparator("Id","=",commentId) },
            new()
            {
                { "Rating", (int)selectResult.Payload![0]["Rating"] + difference }
            });
            if (!updateResult.IsSuccessful)
            {
                return new(Result.Failure("Unable to update Comment Rating"));
            }

            return Result<int>.Success((int)selectResult.Payload![0]["Rating"] + difference);
        }

        public async Task<Result> UpdateUserCommentRating(long commentId, int voterId, bool isUpvote)
        {
            return await _updateDataAccess.Update(_commentLikeTableName, new()
            {
                new("CommentId","=",commentId),
                new("VoterId","=",voterId)
            },
            new()
            {
                { "IsUpvote", isUpvote }
            });
        }
    }
}



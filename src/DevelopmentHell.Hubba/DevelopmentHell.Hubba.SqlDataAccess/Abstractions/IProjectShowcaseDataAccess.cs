using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IProjectShowcaseDataAccess
    {
        Task<Result<Dictionary<string, object>>> GetDetails(string showcaseId);
        Task<Result<Dictionary<string, object>>> GetCommentDetails(long commentId);
        Task<Result<string>> GetCommentShowcase(long commentId);
        Task<Result<Dictionary<string, object>>> GetShowcase(string showcaseId);
        Task<Result<List<Dictionary<string, object>>>> GetUserShowcases(int userId, bool includeDescription = true);
        Task<Result<List<Dictionary<string, object>>>> GetListingShowcases(int listingId);
        Task<Result<List<Dictionary<string, object>>>> GetComments(string showcaseId, int commentCount, int page);
        Task<Result<Dictionary<string,object>>> GetComment(long commentId);
        Task<Result<List<Dictionary<string, object>>>> GetAllShowcaseReports();
        Task<Result<List<Dictionary<string, object>>>> GetShowcaseReports(string showcaseId);
        Task<Result<List<Dictionary<string, object>>>> GetAllCommentReports();
        Task<Result<List<Dictionary<string, object>>>> GetCommentReports(long commentId);
        Task<Result> RecordUserLike(int raterId, string showcaseId);
        Task<Result<double>> IncrementShowcaseLikes(string showcaseId);
        Task<Result> InsertShowcase(int accountId, string showcaseId, int? listingId, string title, string description, DateTime time);
        Task<Result> EditShowcase(string showcaseId, string? title, string? description, DateTime time);
        Task<Result> DeleteShowcase(string showcaseId);
        Task<Result> ChangePublishStatus(string showcaseId, bool isPublished, DateTime? time = null);
        Task<Result> AddComment(string showcaseId, int accountId, string commentText, DateTime time);
        Task<Result> UpdateComment(long commentId, string commentText, DateTime time);
        Task<Result> DeleteComment(long commentId);
        Task<Result<bool?>> GetCommentUserRating(long commentId, int voterId);
        Task<Result<int>> UpdateCommentRating(long commentId, int difference);
        Task<Result> InsertUserCommentRating(long commentId, int voterId, bool isUpvote);
        Task<Result> UpdateUserCommentRating(long commentId, int voterId, bool isUpvote);
        Task<Result> AddCommentReport(long commentId, int reporterId, string reason, DateTime time);
        Task<Result> AddShowcaseReport(string showcaseId, int reporterId, string reason, DateTime time);
        Task<Result> RemoveShowcaseListing(string showcaseId);
		Task<Result<List<Dictionary<string, object>>>> Curate(int offset = 0);
		Task<Result<List<Dictionary<string, object>>>> Search(string query, int offset = 0, double FTTWeight = 0.5, double RWeight = 0.5);
	}
}

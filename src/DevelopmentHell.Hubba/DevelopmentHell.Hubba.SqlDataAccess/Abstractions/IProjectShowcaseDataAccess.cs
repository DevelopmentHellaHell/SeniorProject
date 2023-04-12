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
        Task<Result<Dictionary<string, object>>> GetCommentDetails(string showcaseId);
        Task<Result<Dictionary<string, object>>> GetShowcase(string showcaseId);
        Task<Result<List<Dictionary<string, object>>>> GetComments(string showcaseId, int commentCount, int page);
        Task<Result> RecordUserLike(int raterId, string showcaseId);
        Task<Result<float>> IncrementShowcaseLikes(string showcaseId);
        Task<Result> InsertShowcase(int accountId, int listingId, string title, string description, DateTime time);
        Task<Result> EditShowcase(string? showcaseId, string? title, string? description, DateTime time);
        Task<Result> DeleteShowcase(string showcaseId);
        Task<Result> ChangePublishStatus(string showcaseId, bool isPublished, DateTime time);
        Task<Result> AddComment(string showcaseId, int accountId, string commentText, DateTime time);
        Task<Result> UpdateComment(int commentId, string commentText, DateTime time);
        Task<Result> DeleteComment(int commentId);
        Task<Result<Dictionary<string, object>>> GetCommentUserRating(int commentId, int voterId);
        Task<Result> UpdateCommentRating(int commentId, int difference);
        Task<Result> InsertUserCommentRating(int commentId, int voterId, int isUpvote);
        Task<Result> UpdateUserCommentRating(int commentId, int voterId, int isUpvote);
        Task<Result> AddCommentReport(int commentId, int reporterId, string reason, DateTime time);
        Task<Result> AddShowcaseReport(string showcaseId, int reporterId, string reason, DateTime time);
        Task<Result> RemoveShowcaseListing(string showcaseId);
    }
}

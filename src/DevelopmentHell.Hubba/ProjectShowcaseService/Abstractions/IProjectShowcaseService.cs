using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevelopmentHell.Hubba.Models;
using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.Hubba.ProjectShowcase
{
    public struct ShowcaseComment
    {
        public long? Id { get; set; }
        public int? CommenterId { get; set; }
        public string? CommenterEmail { get; set; }
        public string? ShowcaseId { get; set; }
        public string? Text { get; set; }
        public int? Rating { get; set; }
        public bool? Reported { get; set; }
        public DateTime? Timestamp { get; set; }
        public DateTime? EditTimestamp { get; set; }
    }
    public struct Showcase
    {
        public string? Id { get; set; }
        public int? ShowcaseUserId { get; set; }
        public string? ShowcaseUserEmail { get; set; }
        public int? ListingId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? Reported { get; set; }
        public bool? IsPublished { get; set; }
        public double? Rating { get; set; }
        public DateTime? EditTimestamp { get; set; }
        public DateTime? PublishTimestamp { get; set; }
    }
    public struct ShowcaseReport
    {
        public DateTime? Timestamp { get; set; }
        public string? Reason { get; set; }
        public string? ShowcaseId { get; set; }
        public int? ReporterId { get; set; }
        public bool? IsResolved { get; set; }
    }
    public struct CommentReport
    {
        public DateTime? Timestamp { get; set; }
        public string? Reason { get; set; }
        public long? CommentId { get; set; }
        public int? ReporterId { get; set; }
        public bool? IsResolved { get; set; }
    }
}

namespace DevelopmentHell.Hubba.ProjectShowcase.Service.Abstractions
{
    public interface IProjectShowcaseService
    {
        Task<Result<Dictionary<string, object>>> GetDetails(string showcaseId);
        Task<Result<Dictionary<string, object>>> GetCommentDetails(long commentId);
        Task<Result<Showcase>> GetShowcase(string showcaseId);
        Task<Result<List<Showcase>>> GetUserShowcases(int userId, bool includeDescription = true);
        Task<Result<List<Showcase>>> GetListingShowcases(int listingId);
        Task<Result<List<ShowcaseComment>>> GetComments(string showcaseId, int commentCount, int page);
        Task<Result<ShowcaseComment>> GetComment(long commentId);
        Task<Result<List<ShowcaseReport>>> GetAllShowcaseReports();
        Task<Result<List<ShowcaseReport>>> GetShowcaseReports(string showcaseId);
        Task<Result<List<CommentReport>>> GetAllCommentReports();
        Task<Result<List<CommentReport>>> GetCommentReports(long commentId);
        Task<Result<double>> LikeShowcase(string showcaseId);
        Task<Result<string>> CreateShowcase(int listingId, string title, string description);
        Task<Result> EditShowcase(string showcaseId, int? listingId, string? title, string? description);
        Task<Result> DeleteShowcase(string showcaseId);
        Task<Result> Publish(string showcaseId, int? listingId);
        Task<Result> Unpublish(string showcaseId);
        Task<Result> AddComment(string showcaseId, string commentText);
        Task<Result> EditComment(long commentId, string commentText);
        Task<Result> DeleteComment(long commentId);
        Task<Result<int>> RateComment(long commentId, bool isUpvote);
        Task<Result> ReportComment(long commentId, string reasonText);
        Task<Result> ReportShowcase(string showcaseId, string reasonText);
        Task<Result> Unlink(string showcaseId);
        Task<Result> Link(string showcaseId, int listingId);
    }
}

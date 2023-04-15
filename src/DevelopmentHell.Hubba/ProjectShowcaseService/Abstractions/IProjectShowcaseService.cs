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
        public int? Id { get; set; }
        public int? CommenterEmail { get; set; }
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
        public int? ShowcaseUserEmail { get; set; }
        public int? ListingId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? Reported { get; set; }
        public bool? Published { get; set; }
        public DateTime? EditTimestamp { get; set; }
        public DateTime? PublishTimestamp { get; set; }
    }
}

namespace DevelopmentHell.Hubba.ProjectShowcase.Service.Abstractions
{
    public interface IProjectShowcaseService
    {
        Task<Result<Dictionary<string, object>>> GetDetails(string showcaseId);
        Task<Result<Dictionary<string, object>>> GetCommentDetails(string showcaseId);
        Task<Result<Showcase>> GetShowcase(string showcaseId);
        Task<Result<List<ShowcaseComment>>> GetComments(string showcaseId, int commentCount, int page);
        Task<Result<float>> LikeShowcase(string showcaseId);
        Task<Result<string>> CreateShowcase(int listingId, string title, string description);
        Task<Result> EditShowcase(string showcaseId, int? listingId, string? title, string? description);
        Task<Result> DeleteShowcase(string showcaseId);
        Task<Result> Publish(string showcaseId, int? listingId);
        Task<Result> Unpublish(string showcaseId);
        Task<Result> AddComment(string showcaseId, string commentText);
        Task<Result> EditComment(int commentId, string commentText);
        Task<Result> DeleteComment(int commentId);
        Task<Result> RateComment(int commentId, bool isUpvote);
        Task<Result> ReportComment(int commentId, string reasonText);
        Task<Result> ReportShowcase(string showcaseId, string reasonText);
        Task<Result> Unlink(string showcaseId);
    }
}

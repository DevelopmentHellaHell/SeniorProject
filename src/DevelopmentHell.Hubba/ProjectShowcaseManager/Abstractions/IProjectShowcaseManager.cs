using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevelopmentHell.Hubba.Models;
using Microsoft.AspNetCore.Http;
using DevelopmentHell.Hubba.ProjectShowcase.Service.Abstractions;

namespace DevelopmentHell.Hubba.ProjectShowcase.Manager.Abstractions
{

    public struct PackagedShowcase
    {
        public Showcase? Showcase { get; set; }
        public List<string>? FilePaths { get; set; }
        public List<ShowcaseComment>? Comments { get; set; }
    }

    public interface IProjectShowcaseManager
    {
        Task<Result<bool>> VerifyOwnership(string showcaseId);
        Task<Result<bool>> VerifyCommentOwnership(int commentId);
        Task<Result<PackagedShowcase>> GetShowcase(string showcaseId);
        Task<Result<List<Showcase>>> GetUserShowcases(int userId, bool includeDescription = true);
        Task<Result<List<ShowcaseReport>>> GetAllShowcaseReports();
        Task<Result<List<ShowcaseReport>>> GetShowcaseReports(string showcaseId);
        Task<Result<List<ShowcaseComment>>> GetComments(string showcaseId, int? commentCount, int? page);
        Task<Result<ShowcaseComment>> GetComment(int commentId);
        Task<Result<List<CommentReport>>> GetAllCommentReports();
        Task<Result<List<CommentReport>>> GetCommentReports(int commentId);
        Task<Result<float>> LikeShowcase(string showcaseId);
        Task<Result<string>> CreateShowcase(int listingId, string title, string description, List<IFormFile> files);
        Task<Result> EditShowcase(string showcaseId, int? listingId, string? title, string? description, List<IFormFile>? files);
        Task<Result> DeleteShowcase(string showcaseId);
        Task<Result> Publish(string showcaseId, int? listingId);
        Task<Result> Unpublish(string showcaseId);
        Task<Result> AddComment(string showcaseId, string commentText);
        Task<Result> EditComment(int commentId, string commentText);
        Task<Result> DeleteComment(int commentId);
        Task<Result<int>> RateComment(int commentId, bool isUpvote);
        Task<Result> ReportComment(int commentId, string reasonText);
        Task<Result> ReportShowcase(string showcaseId, string reasonText);
        Task<Result> Unlink(string showcaseId);
    }
}

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
        Task<Result<bool>> VerifyCommentOwnership(long commentId);
        Task<Result<List<string>>> GetShowcaseFiles(string showcaseId);
        Task<Result<PackagedShowcase>> GetShowcase(string showcaseId);
        Task<Result<List<Showcase>>> GetUserShowcases(int userId, bool includeDescription = true);
        Task<Result<List<ShowcaseReport>>> GetAllShowcaseReports();
        Task<Result<List<ShowcaseReport>>> GetShowcaseReports(string showcaseId);
        Task<Result<List<ShowcaseComment>>> GetComments(string showcaseId, int? commentCount = 10, int? page = 1);
        Task<Result<ShowcaseComment>> GetComment(long commentId);
        Task<Result<List<CommentReport>>> GetAllCommentReports();
        Task<Result<List<CommentReport>>> GetCommentReports(long commentId);
        Task<Result<double>> LikeShowcase(string showcaseId);
        Task<Result<string>> CreateShowcase(int listingId, string title, string description, List<Tuple<string,string>> files);
        Task<Result> EditShowcase(string showcaseId, int? listingId, string? title, string? description, List<Tuple<string,string>>? files);
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
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.ProjectShowcase.Service.Abstractions
{
    public interface IProjectShowcaseService
    {
        Task<Result<Dictionary<string, object>>> GetDetails(string showcaseId);
        Task<Result<Dictionary<string, object>>> GetCommentDetails(string showcaseId);
        Task<Result<Showcase>> GetShowcase(string showcaseId);
        Task<Result<List<ShowcaseComment>>> GetComments(string showcaseId, int commentCount, int page);
        Task<Result<float>> LikeShowcase(string showcaseId);
        Task<Result> CreateShowcase(ShowcaseDTO uploadedShowcase);
        Task<Result> EditShowcase(ShowcaseDTO editedShowcase);
        Task<Result> DeleteShowcase(string showcaseId);
        Task<Result> Publish(string showcaseId);
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

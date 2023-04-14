using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.ProjectShowcase.Manager.Abstractions;
using DevelopmentHell.Hubba.ProjectShowcase.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.ProjectShowcase.Manager.Implementations
{
    public class ProjectShowcaseManager : IProjectShowcaseManager
    {
        private readonly IProjectShowcaseService _projectShowcaseService;
        private readonly IFileService _fileService;
        private readonly ILoggerService _logger;
        private readonly IAuthorizationService _authorizationService;
        public ProjectShowcaseManager(IProjectShowcaseService projectShowcaseService, IFileService fileService, ILoggerService loggerService, IAuthorizationService authorizationService) 
        {
            _projectShowcaseService = projectShowcaseService;
            _fileService = fileService;
            _logger = loggerService;
            _authorizationService = authorizationService;
        }
        public async Task<Result> AddComment(string showcaseId, string commentText)
        {

            throw new NotImplementedException();
        }

        public async Task<Result> CreateShowcase(int listingId, string title, string description, List<HttpPostedFileBase> uploadedShowcase)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> DeleteComment(int commentId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> DeleteShowcase(string showcaseId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> EditComment(int commentId, string commentText)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> EditShowcase(int listingId, string title, string description, List<HttpPostedFileBase> uploadedShowcase)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<ShowcaseComment>>> GetComments(string showcaseId, int? commentCount, int? page)
        {
            if (commentCount == null)
            {
                commentCount = 10;
            }
            if (page == null)
            {
                page = 1;
            }

            var testResult = await _projectShowcaseService.GetDetails(showcaseId);
            if (!testResult.IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = testResult.ErrorMessage
                };
            }
            if (!(bool)testResult.Payload!["IsPublished"])
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unauthorized access. "
                };
            }

            return await _projectShowcaseService.GetComments(showcaseId, (int)commentCount, (int)page).ConfigureAwait(false);
        }

        public async Task<Result<PackagedShowcase>> GetShowcase(string showcaseId)
        {
            var getShowcaseResult = await _projectShowcaseService.GetShowcase(showcaseId).ConfigureAwait(false);
            if (!getShowcaseResult.IsSuccessful)
            {
                var errorMessage = $"Error in getting Showcase {showcaseId}: {getShowcaseResult.ErrorMessage}";
                _logger.Log(LogLevel.WARNING, Category.BUSINESS, errorMessage);
                return new() { 
                    IsSuccessful = false,
                    ErrorMessage = errorMessage
                };
            }

            Result<List<string>> getFilesResult = await _fileService.GetFolder($"showcases/{showcaseId}");
            if (!getFilesResult.IsSuccessful)
            {
                var errorMessage = $"Error in getting files for Showcase {showcaseId}: {getFilesResult.ErrorMessage}";
                _logger.Log(LogLevel.WARNING, Category.BUSINESS, errorMessage);
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = errorMessage
                };
            }

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (!(bool)getShowcaseResult.Payload.Published! 
                && int.Parse(stringAccountId!) != (int)getShowcaseResult.Payload.ShowcaseUserId!
                && claimsPrincipal?.FindFirstValue("role") != "AdminUser")
            {
                var errorMessage = $"Invalid access to showcase {showcaseId} by user : {getFilesResult.ErrorMessage}";
                _logger.Log(LogLevel.WARNING, Category.BUSINESS, errorMessage);
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = errorMessage
                };
            }

            var getCommentsResult = await _projectShowcaseService.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
            if (!getCommentsResult.IsSuccessful)
            {
                var errorMessage = $"Error in getting comments for Showcase {showcaseId}: {getFilesResult.ErrorMessage}";
                _logger.Log(LogLevel.WARNING, Category.BUSINESS, errorMessage);
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = errorMessage
                };
            }

            return new()
            {
                IsSuccessful = true,
                Payload = new()
                {
                    Comments = getCommentsResult.Payload,
                    Showcase = getShowcaseResult.Payload,
                    FilePaths = getFilesResult.Payload,
                }
            };
        }

        public async Task<Result<float>> LikeShowcase(string showcaseId)
        {
            var authResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
            if (!authResult.IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unauthorized attempt to like showcase"
                };
            }

            return await _projectShowcaseService.LikeShowcase(showcaseId).ConfigureAwait(false);
        }

        public async Task<Result> Publish(string showcaseId, int? listingId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> RateComment(int commentId, bool isUpvote)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> ReportComment(int commentId, string reasonText)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> ReportShowcase(string showcaseId, string reasonText)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> Unlink(string showcaseId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> Unpublish(string showcaseId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> VerifyCommentOwnership(int commentId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> VerifyOwnership(string showcaseId)
        {
            throw new NotImplementedException();
        }
    }
}

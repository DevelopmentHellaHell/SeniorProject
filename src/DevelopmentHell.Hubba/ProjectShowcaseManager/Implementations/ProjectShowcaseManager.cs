using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.ProjectShowcase.Manager.Abstractions;
using DevelopmentHell.Hubba.ProjectShowcase.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<Result<string>> CreateShowcase(int listingId, string title, string description, List<IFormFile> files)
        {
            try
            {
                var authResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
                if (!authResult.IsSuccessful)
                {
                    return new(Result.Failure("Unauthorized attempt to create a showcase",401));
                }

                var createResult = await _projectShowcaseService.CreateShowcase(listingId, title, description).ConfigureAwait(false);
                if (!createResult.IsSuccessful)
                {
                    if (!createResult.StatusCode.HasValue)
                    {
                        _logger.Warning(Category.BUSINESS, "Unable to Create Showcase.", "ShowcaseService");
                    }
                    return new(Result.Failure("Unable to Create Showcase."));
                }

                for (int i = 0; i < files.Count; i++)
                {
                    _fileService.UploadFile($"showcases/{createResult.Payload!}/0{i}_{files[i].Name}", files[i]);
                }

                return Result<string>.Success(createResult.Payload!);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, "Unable to Create Showcase.", "ShowcaseManager");
                return new(Result.Failure("Unable to Create showcase."));
            }
        }

        public async Task<Result> DeleteComment(int commentId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> DeleteShowcase(string showcaseId)
        {
            try
            {
                var checkResult = await IsAdminOrOwnerOf(showcaseId).ConfigureAwait(false);
                if (!checkResult.IsSuccessful || !checkResult.Payload)
                {
                    return new(checkResult);
                }

                var deleteResult = await _projectShowcaseService.DeleteShowcase(showcaseId).ConfigureAwait(false);
                if (!deleteResult.IsSuccessful)
                {
                    return Result.Failure($"Unable to Delete showcase: {deleteResult.ErrorMessage}");
                }

                return deleteResult;
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, "Unable to Delete Showcase.", "ShowcaseManager");
                return Result.Failure($"Unable to Delete showcase.");

            }
        }

        public async Task<Result> EditComment(int commentId, string commentText)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> EditShowcase(string showcaseId, int? listingId, string? title, string? description, List<IFormFile>? files)
        {
            try
            {
                var checkResult = await IsAdminOrOwnerOf(showcaseId).ConfigureAwait(false);
                if (!checkResult.IsSuccessful || !checkResult.Payload)
                {
                    return new(checkResult);
                }

                var editResult = await _projectShowcaseService.EditShowcase(showcaseId, listingId, title, description).ConfigureAwait(false);
                if (!editResult.IsSuccessful)
                {
                    return Result.Failure($"Unable to Edit showcase: {editResult.ErrorMessage}");
                }

                if (files != null && files.Count > 0)
                {
                    _fileService.DeleteDir($"showcases/{showcaseId}");
                    for (int i = 0; i < files.Count; i++)
                    {
                        _fileService.UploadFile($"showcases/{showcaseId}/0{i}_{files[i].Name}", files[i]);
                    }
                }

                return new()
                {
                    IsSuccessful = true
                };
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, "Unable to Edit Showcase.", "ShowcaseManager");
                return Result.Failure($"Unable to Edit showcase.");
            }
        }

        public async Task<Result<List<ShowcaseComment>>> GetComments(string showcaseId, int? commentCount, int? page)
        {
            try
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
                    if (!testResult.StatusCode.HasValue)
                    {
                        _logger.Warning(Category.BUSINESS, "Error getting showcase Details.", "ShowcaseManager");
                    }
                    return new(Result.Failure(testResult.ErrorMessage!));
                }
                if (!(bool)testResult.Payload!["IsPublished"])
                {
                    return new(Result.Failure("Unauthorized access.",401));
                }

                return await _projectShowcaseService.GetComments(showcaseId, (int)commentCount, (int)page).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, "Unable to Fetch Comments.", "ShowcaseManager");
                return new(Result.Failure("Error in fetching comments."));
            }
        }

        public async Task<Result<PackagedShowcase>> GetShowcase(string showcaseId)
        {
            try
            {
                var getShowcaseResult = await _projectShowcaseService.GetShowcase(showcaseId).ConfigureAwait(false);
                if (!getShowcaseResult.IsSuccessful)
                {
                    if (!getShowcaseResult.StatusCode.HasValue)
                    {
                        _logger.Warning(Category.BUSINESS, "Error getting Showcase.", "ShowcaseManager");
                    }
                    var errorMessage = $"Error in getting Showcase {showcaseId}: {getShowcaseResult.ErrorMessage}";
                    return new(Result.Failure(errorMessage));
                }

                Result<List<string>> getFilesResult = await _fileService.GetFolder($"showcases/{showcaseId}");
                if (!getFilesResult.IsSuccessful)
                {
                    var errorMessage = $"Error in getting files for Showcase {showcaseId}: {getFilesResult.ErrorMessage}";
                    if (!getFilesResult.StatusCode.HasValue)
                    {
                        _logger.Warning(Category.BUSINESS, $"Error in getting files for Showcase {showcaseId}: {getFilesResult.ErrorMessage}", "ShowcaseManager");
                    }
                    return new(Result.Failure(errorMessage));
                }

                var checkResult = await IsAdminOrOwnerOf(showcaseId).ConfigureAwait(false);
                if (!checkResult.IsSuccessful || !checkResult.Payload)
                {
                    return new(checkResult);
                }

                var getCommentsResult = await _projectShowcaseService.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
                if (!getCommentsResult.IsSuccessful)
                {
                    var errorMessage = $"Error in getting comments for Showcase {showcaseId}: {getFilesResult.ErrorMessage}";
                    if (!getCommentsResult.StatusCode.HasValue)
                    {
                        _logger.Warning(Category.BUSINESS, errorMessage);
                    }
                    return new(Result.Failure(errorMessage));
                }

                return Result<PackagedShowcase>.Success(new() { Comments = getCommentsResult.Payload, Showcase = getShowcaseResult.Payload, FilePaths = getFilesResult.Payload });
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, "Error in getting showcase", "ShowcaseManager");
                return new(Result.Failure("Error in getting showcase"));
            }
        }

        public async Task<Result<float>> LikeShowcase(string showcaseId)
        {
            try
            {
                var authResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
                if (!authResult.IsSuccessful)
                {
                    return new(Result.Failure("Unauthorized attempt to like showcase"));
                }

                return await _projectShowcaseService.LikeShowcase(showcaseId).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                _logger.Warning(Category.BUSINESS, "Error in liking showcase", "ShowcaseManager");
                return new(Result.Failure("Error in liking showcase"));
            }
        }

        public async Task<Result> Publish(string showcaseId, int? listingId)
        {
            try
            {
                var checkResult = await IsAdminOrOwnerOf(showcaseId).ConfigureAwait(false);
                if (!checkResult.IsSuccessful || !checkResult.Payload)
                {
                    return new(checkResult);
                }

                return await _projectShowcaseService.Publish(showcaseId, listingId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, "Error in publishing showcase", "ShowcaseManager");
                return new(Result.Failure("Error in publishing showcase"));
            }
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
            var authResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
            if(!authResult.IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = true,
                    ErrorMessage = "User is not logged in.",
                    Payload = false
                };
            }

            var detailResult = await _projectShowcaseService.GetDetails(showcaseId).ConfigureAwait(false);
            if (!detailResult.IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unable to get showcase details."
                };
            }

            if ((int)detailResult.Payload!["ShowcaseUserId"] != int.Parse((Thread.CurrentPrincipal as ClaimsPrincipal)?.FindFirstValue("sub")!))
            {
                return new()
                {
                    IsSuccessful = true,
                    ErrorMessage = "User is not owner.",
                    Payload = false
                };
            }

            return new()
            {
                IsSuccessful = true,
                Payload = true
            };
        }

        public async Task<Result<bool>> IsAdminOrOwnerOf(string showcaseId)
        {
            var authResult = _authorizationService.Authorize(new string[] { "AdminUser" });
            if (!authResult.IsSuccessful)
            {
                var verificationResult = await VerifyOwnership(showcaseId).ConfigureAwait(false);
                if (!verificationResult.IsSuccessful)
                {
                    if (!verificationResult.StatusCode.HasValue)
                    {
                        _logger.Warning(Category.BUSINESS, "Error verifying ownership.", "ShowcaseManager");
                    }
                    return Result<bool>.Failure("Unable to verify ownership");
                }
                else if (!verificationResult.Payload)
                {
                    return Result<bool>.Failure("Unable to verify ownership", 401);
                }
            }
            return Result<bool>.Success(true);
        }
    }
}

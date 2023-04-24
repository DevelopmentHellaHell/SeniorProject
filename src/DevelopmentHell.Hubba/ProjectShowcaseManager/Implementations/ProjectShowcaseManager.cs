using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.ProjectShowcase.Manager.Abstractions;
using DevelopmentHell.Hubba.ProjectShowcase.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
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

        public async Task<Result<List<string>>> GetShowcaseFiles(string showcaseId)
        {
            Result<List<string>> getFilesResult = await _fileService.GetFilesInDir($"ProjectShowcases/{showcaseId}");
            await _fileService.Disconnect();
            if (!getFilesResult.IsSuccessful)
            {
                var errorMessage = $"Error in getting files for Showcase {showcaseId}: {getFilesResult.ErrorMessage}";
                return new(Result.Failure(errorMessage));
            }
            return Result<List<string>>.Success(getFilesResult.Payload!);
        }
        public async Task<Result> AddComment(string showcaseId, string commentText)
        {

            try
            {
                var authResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
                if (!authResult.IsSuccessful)
                {
                    return new(Result.Failure("Unauthorized attempt to create a comment", 401));
                }

                var createdResult = await _projectShowcaseService.AddComment(showcaseId, commentText).ConfigureAwait(false);
                if (!createdResult.IsSuccessful)
                {
                    return Result.Failure("Unable to add comment.");
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Unable to Add Comment: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Unable to Add Comment."));
            }
        }

        public async Task<Result<string>> CreateShowcase(int listingId, string title, string description, List<Tuple<string,string>> files)
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
                    return new(Result.Failure("Unable to Create Showcase."));
                }

                for (int i = 0; i < files.Count; i++)
                {
                    byte[] bytes = Convert.FromBase64String(files[i].Item2);
                    var crateDir = await _fileService.CreateDir($"ProjectShowcases/{createResult.Payload!}");
                    var uploadResult = await _fileService.UploadFile($"ProjectShowcases/{createResult.Payload!}",$"0{i}_{files[i].Item1}", bytes);
                    if (!uploadResult.IsSuccessful)
                    {
                        await _fileService.Disconnect();
                        return new(Result.Failure("Unable to upload file."));
                    }
                }
                await _fileService.Disconnect();

                return Result<string>.Success(createResult.Payload!);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Unable to Create Showcase: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Unable to Create showcase."));
            }
        }

        public async Task<Result> DeleteComment(long commentId)
        {
            try
            {
                var checkResult = await IsAdminOrOwnerOfComment(commentId).ConfigureAwait(false);
                if (!checkResult.IsSuccessful || !checkResult.Payload)
                {
                    return new(checkResult);
                }

                var deleteResult = await _projectShowcaseService.DeleteComment(commentId).ConfigureAwait(false);
                if (!deleteResult.IsSuccessful)
                {
                    return Result.Failure($"Unable to Delete Showcase: {deleteResult.ErrorMessage}");
                }

                return deleteResult;
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Unable to Edit Comment: {ex.Message}", "ShowcaseManager");
                return Result.Failure($"Unable to Edit Comment.");
            }
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
                _logger.Warning(Category.BUSINESS, $"Unable to Delete Showcase: {ex.Message}", "ShowcaseManager");
                return Result.Failure($"Unable to Delete showcase.");
            }
        }

        public async Task<Result> EditComment(long commentId, string commentText)
        {
            try
            {
                var checkResult = await IsAdminOrOwnerOfComment(commentId).ConfigureAwait(false);
                if (!checkResult.IsSuccessful || !checkResult.Payload)
                {
                    return new(checkResult);
                }

                var editResult = await _projectShowcaseService.EditComment(commentId, commentText).ConfigureAwait(false);
                if (!editResult.IsSuccessful)
                {
                    return Result.Failure($"Unable to Delete Showcase: {editResult.ErrorMessage}");
                }

                return editResult;
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Unable to Edit Comment: {ex.Message}", "ShowcaseManager");
                return Result.Failure($"Unable to Edit Comment.");
            }
        }

        public async Task<Result> EditShowcase(string showcaseId, int? listingId, string? title, string? description, List<Tuple<string,string>>? files)
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
                    await _fileService.DeleteDir($"ProjectShowcases/{showcaseId}");
                    for (int i = 0; i < files.Count; i++)
                    {
                        byte[] bytes = files[i].Item2.Select(c => (byte)c).ToArray();
                        var uploadResult = await _fileService.UploadFile($"ProjectShowcases/{showcaseId}", $"0{i}_{files[i].Item1}", bytes);
                        await _fileService.Disconnect();
                        if (!uploadResult.IsSuccessful)
                        {
                            return Result.Failure("Unable to upload new showcase files");
                        }
                    }
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Unable to Edit Showcase: {ex.Message}", "ShowcaseManager");
                return Result.Failure($"Unable to Edit showcase.");
            }
        }

        public async Task<Result<List<ShowcaseComment>>> GetComments(string showcaseId, int? commentCount = 10, int? page = 1)
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
                    return new(Result.Failure(testResult.ErrorMessage!));
                }
                var checkResult = await VerifyOwnership(showcaseId);
                if (!checkResult.IsSuccessful && !(bool)testResult.Payload!["IsPublished"])
                {
                    return new(Result.Failure("Unauthorized access.",401));
                }

                return await _projectShowcaseService.GetComments(showcaseId, (int)commentCount, (int)page).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Unable to Fetch Comments: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in fetching comments."));
            }
        }

        public async Task<Result<ShowcaseComment>> GetComment(long commentId)
        {
            try
            {
                var checkResult = await IsAdminOrOwnerOfComment(commentId).ConfigureAwait(false);
                if (!checkResult.IsSuccessful)
                {
                    return new(Result.Failure("Unexpected error when getting comment"));
                }
                if (!checkResult.Payload!)
                {
                    return new(Result.Failure("Unauthorized attempt to get comment", 401));
                }

                return await _projectShowcaseService.GetComment(commentId);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Unable to Fetch Comment: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in fetching comment."));
            }
        }

        public async Task<Result<List<CommentReport>>> GetAllCommentReports()
        {
            try
            {
                var authResult = _authorizationService.Authorize(new string[] { "AdminUser" });
                if (!authResult.IsSuccessful)
                {
                    _logger.Warning(Category.BUSINESS, $"401 Error in getting comment reports", "ShowcaseManager");
                    return new(Result.Failure("Unauthorized access to comment reports", 401));
                }
                var result = await _projectShowcaseService.GetAllCommentReports().ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return new(Result.Failure("Unable to get all comment reports"));
                }
                return Result<List<CommentReport>>.Success(result.Payload!);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting comment reports: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in getting comment reports"));
            }
        }

        public async Task<Result<List<CommentReport>>> GetCommentReports(long commentId)
        {
            try
            {
                var authResult = _authorizationService.Authorize(new string[] { "AdminUser" });
                if (!authResult.IsSuccessful)
                {
                    _logger.Warning(Category.BUSINESS, $"401 Error in getting comment reports", "ShowcaseManager");
                    return new(Result.Failure("Unauthorized access to comment reports", 401));
                }
                return await _projectShowcaseService.GetCommentReports(commentId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting comment reports: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in getting comment reports"));
            }
        }

        public async Task<Result<PackagedShowcase>> GetShowcase(string showcaseId)
        {
            try
            {
                var getShowcaseResult = await _projectShowcaseService.GetShowcase(showcaseId).ConfigureAwait(false);
                if (!getShowcaseResult.IsSuccessful)
                {
                    var errorMessage = $"Error in getting Showcase {showcaseId}: {getShowcaseResult.ErrorMessage}";
                    return new(Result.Failure(errorMessage));
                }

                Result<List<string>> getFilesResult = await _fileService.GetFilesInDir($"ProjectShowcases/{showcaseId}");
                await _fileService.Disconnect();
                if (!getFilesResult.IsSuccessful)
                {
                    var errorMessage = $"Error in getting files for Showcase {showcaseId}: {getFilesResult.ErrorMessage}";
                    return new(Result.Failure(errorMessage));
                }

                if (!(bool)getShowcaseResult.Payload!.IsPublished!)
                {
                    var checkResult = await IsAdminOrOwnerOf(showcaseId).ConfigureAwait(false);
                    if (!checkResult.IsSuccessful || !checkResult.Payload)
                    {
                        return new(checkResult);
                    }
                }
                

                var getCommentsResult = await _projectShowcaseService.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
                if (!getCommentsResult.IsSuccessful)
                {
                    var errorMessage = $"Error in getting comments for Showcase {showcaseId}: {getFilesResult.ErrorMessage}";
                    return new(Result.Failure(errorMessage));
                }

                return Result<PackagedShowcase>.Success(new() { Comments = getCommentsResult.Payload, Showcase = getShowcaseResult.Payload, FilePaths = getFilesResult.Payload });
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting showcase: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in getting showcase"));
            }
        }

        public async Task<Result<List<Showcase>>> GetUserShowcases(int userId, bool includeDescription = true)
        {
            try
            {
                var authResult = _authorizationService.Authorize(new string[] { "AdminUser" });
                if (!authResult.IsSuccessful)
                {
                    int requestedId = int.Parse((Thread.CurrentPrincipal as ClaimsPrincipal)?.FindFirstValue("sub")!);
                    if (userId != requestedId)
                    {
                        return new(Result.Failure("Unauthorized attempt to get user showcases", 401));
                    }
                }
                return await _projectShowcaseService.GetUserShowcases(userId, includeDescription).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting user showcases: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in getting user showcases"));
            }
        }

        public async Task<Result<List<ShowcaseReport>>> GetAllShowcaseReports()
        {
            try
            {
                var authResult = _authorizationService.Authorize(new string[] { "AdminUser" });
                if (!authResult.IsSuccessful)
                {
                    return new(Result.Failure("Unauthorized attempt to get Showcase Reports", 401));
                }
                return await _projectShowcaseService.GetAllShowcaseReports().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting showcase reports: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in getting showcase reports"));
            }
        }

        public async Task<Result<List<ShowcaseReport>>> GetShowcaseReports(string showcaseId)
        {
            try
            {
                var authResult = _authorizationService.Authorize(new string[] { "AdminUser" });
                if (!authResult.IsSuccessful)
                {
                    return new(Result.Failure("Unauthorized attempt to get Showcase Reports", 401));
                }
                return await _projectShowcaseService.GetShowcaseReports(showcaseId);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting showcase reports: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in getting showcase reports"));
            }
        }

        public async Task<Result<double>> LikeShowcase(string showcaseId)
        {
            try
            {
                var authResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
                if (!authResult.IsSuccessful)
                {
                    return new(Result.Failure("Unauthorized attempt to like showcase", 401));
                }

                return await _projectShowcaseService.LikeShowcase(showcaseId).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in liking showcase: {ex.Message}", "ShowcaseManager");
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
                _logger.Warning(Category.BUSINESS, $"Error in publishing showcase: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in publishing showcase"));
            }
        }

        public async Task<Result<int>> RateComment(long commentId, bool isUpvote)
        {
            try
            {
                var authResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
                if (!authResult.IsSuccessful)
                {
                    return new(Result.Failure("Unauthorized attempt to like showcase", 401));
                }

                return await _projectShowcaseService.RateComment(commentId, isUpvote).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in rating showcase: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in rating showcase"));
            }
        }

        public async Task<Result> ReportComment(long commentId, string reasonText)
        {
            try
            {
                var authResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
                if (!authResult.IsSuccessful)
                {
                    return new(Result.Failure("Unauthorized attempt to Report comment",401));
                }

                return await _projectShowcaseService.ReportComment(commentId, reasonText).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in reporting comment: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in reporting comment"));
            }
        }

        public async Task<Result> ReportShowcase(string showcaseId, string reasonText)
        {
            try
            {
                var authResult = _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" });
                if (!authResult.IsSuccessful)
                {
                    return new(Result.Failure("Unauthorized attempt to Report showcase", 401));
                }
                return await _projectShowcaseService.ReportShowcase(showcaseId, reasonText).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in reporting showcase: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in reporting showcase"));
            }
        }

        public async Task<Result> Unlink(string showcaseId)
        {
            try
            {
                var checkResult = await IsAdminOrOwnerOf(showcaseId).ConfigureAwait(false);
                if (!checkResult.IsSuccessful || !checkResult.Payload)
                {
                    return checkResult;
                }

                var unpubResult = await _projectShowcaseService.Unpublish(showcaseId).ConfigureAwait(false);
                if (!unpubResult.IsSuccessful)
                {
                    return unpubResult;
                }
                return await _projectShowcaseService.Unlink(showcaseId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in unlinking showcase: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in unlinking showcase"));
            }
        }

        public async Task<Result> Unpublish(string showcaseId)
        {
            try
            {
                var checkResult = await IsAdminOrOwnerOf(showcaseId).ConfigureAwait(false);
                if (!checkResult.IsSuccessful || !checkResult.Payload)
                {
                    return new(checkResult);
                }

                return await _projectShowcaseService.Unpublish(showcaseId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in unpublishing showcase: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in unpublishing showcase"));
            }
        }

        public async Task<Result<bool>> VerifyCommentOwnership(long commentId)
        {
            var authResult = _authorizationService.Authorize(new string[] { "AdminUser" });
            if (authResult.IsSuccessful)
            {
                return Result<bool>.Success(false);
            }

            var detailResult = await _projectShowcaseService.GetCommentDetails(commentId).ConfigureAwait(false);
            if (!detailResult.IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unable to get comment details."
                };
            }

            if ((int)detailResult.Payload!["CommenterId"] != int.Parse((Thread.CurrentPrincipal as ClaimsPrincipal)?.FindFirstValue("sub")!))
            {
                return new(Result.Failure("User is not owner", 401));
            }

            return new()
            {
                IsSuccessful = true,
                Payload = true
            };
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
                return new(Result.Failure("Unable to get Showcase Details"));
            }

            if ((int)detailResult.Payload!["ShowcaseUserId"] != int.Parse((Thread.CurrentPrincipal as ClaimsPrincipal)?.FindFirstValue("sub")!))
            {
                return new(Result.Failure("User is not owner", 401));
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
                    return Result<bool>.Failure("Unable to verify showcase ownership");
                }
                else if (!verificationResult.Payload)
                {
                    return Result<bool>.Failure("Unable to verify showcase ownership", 401);
                }
            }
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> IsAdminOrOwnerOfComment(long commentId)
        {
            var authResult = _authorizationService.Authorize(new string[] { "AdminUser" });
            if (!authResult.IsSuccessful)
            {
                var verificationResult = await VerifyCommentOwnership(commentId).ConfigureAwait(false);
                if (!verificationResult.IsSuccessful)
                {
                    return Result<bool>.Failure("Unable to verify comment ownership");
                }
                else if (!verificationResult.Payload)
                {
                    return Result<bool>.Failure("Unable to verify comment ownership", 401);
                }
            }
            return Result<bool>.Success(true);
        }
    }
}

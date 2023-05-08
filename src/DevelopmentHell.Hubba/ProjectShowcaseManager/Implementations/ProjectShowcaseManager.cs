using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.ProjectShowcase.Manager.Abstractions;
using DevelopmentHell.Hubba.ProjectShowcase.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.ListingProfile.Service.Abstractions;
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
        private readonly IListingProfileService _listingProfileService;
        private readonly ILoggerService _logger;
        private readonly IAuthorizationService _authorizationService;
        public ProjectShowcaseManager(IProjectShowcaseService projectShowcaseService, IFileService fileService, IListingProfileService listingProfileService, ILoggerService loggerService, IAuthorizationService authorizationService) 
        {
            _projectShowcaseService = projectShowcaseService;
            _fileService = fileService;
            _listingProfileService = listingProfileService;
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
                _logger.Error(Category.BUSINESS, errorMessage, "ShowcaseManager");
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
                    _logger.Warning(Category.BUSINESS, $"Unauthorized attempt to create a comment", "ShowcaseManager");
                    return new(Result.Failure("Unauthorized attempt to create a comment", 401));
                }

                var createdResult = await _projectShowcaseService.AddComment(showcaseId, commentText).ConfigureAwait(false);
                if (!createdResult.IsSuccessful)
                {
                    _logger.Error(Category.BUSINESS, $"Unable to add comment: {createdResult.ErrorMessage}", "ShowcaseManager");
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
                    _logger.Warning(Category.BUSINESS, $"Unauthorized attempt to create a showcase", "AuthorizationService");
                    return new(Result.Failure("Unauthorized attempt to create a showcase",401));
                }

                if (listingId != 0)
                {
                    var checkResult = await _listingProfileService.CheckListingHistory(listingId, int.Parse((Thread.CurrentPrincipal as ClaimsPrincipal)?.FindFirstValue("sub")!)).ConfigureAwait(false);
                    if (!checkResult.IsSuccessful)
                    {
                        _logger.Error(Category.BUSINESS, $"Unable to verify user history with listing: {checkResult.ErrorMessage}", "ListingProfileService");
                        return new(Result.Failure("Unable to verify user history with listing"));
                    }
                    if (!checkResult.Payload!)
                    {
                        _logger.Warning(Category.BUSINESS, $"Unable to verify user history with listing", "ShowcaseManager");
                        return new(Result.Failure("Unable to verify user history with listing. A user can only showcase their work on a listing if they have a recorded history with it.", 400));
                    }
                }


                var createResult = await _projectShowcaseService.CreateShowcase(listingId, title, description).ConfigureAwait(false);
                if (!createResult.IsSuccessful)
                {
                    _logger.Error(Category.BUSINESS, $"Unable to Create Showcase: {createResult.ErrorMessage}", "ProjectShowcaseService");
                    return new(Result.Failure("Unable to Create Showcase."));
                }

                var createDir = await _fileService.CreateDir($"ProjectShowcases/{createResult.Payload!}");
                for (int i = 0; i < files.Count; i++)
                {
                    byte[] bytes = Convert.FromBase64String(files[i].Item2);
                    var uploadResult = await _fileService.UploadFile($"ProjectShowcases/{createResult.Payload!}",$"0{i}_{files[i].Item1}", bytes);
                    if (!uploadResult.IsSuccessful)
                    {
                        await _fileService.Disconnect();
                        _logger.Error(Category.BUSINESS, $"Unable to upload file: {uploadResult.ErrorMessage}", "FileService");
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
                    _logger.Error(Category.BUSINESS, $"Unable to Delete Comment: {deleteResult.ErrorMessage}", "ProjectShowcaseService");
                    return Result.Failure($"Unable to Delete Comment");
                }

                return deleteResult;
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Unable to Delete Comment: {ex.Message}", "ShowcaseManager");
                return Result.Failure($"Unable to Delete Comment.");
            }
        }

        public async Task<Result> DeleteShowcase(string showcaseId)
        {
            try
            {
                var checkResult = await IsAdminOrOwnerOf(showcaseId).ConfigureAwait(false);
                if (!checkResult.IsSuccessful || !checkResult.Payload)
                {
                    _logger.Warning(Category.BUSINESS, $"Unable to Delete Showcase: {checkResult.ErrorMessage}", "ProjectShowcaseManager");
                    return new(checkResult);
                }

                var deleteResult = await _projectShowcaseService.DeleteShowcase(showcaseId).ConfigureAwait(false);
                if (!deleteResult.IsSuccessful)
                {
                    _logger.Error(Category.BUSINESS, $"Unable to Delete Showcase: {deleteResult.ErrorMessage}", "ProjectShowcaseService");
                    return Result.Failure($"Unable to Delete showcase.");
                }

                var fileResult = await _fileService.DeleteDir($"ProjectShowcases /{showcaseId}");
                if (!fileResult.IsSuccessful)
                {
                    _logger.Error(Category.BUSINESS, $"Unable to delete showcase files: {fileResult.ErrorMessage}", "FileService");
                    return Result.Failure("Unabel to Delete Showcase");
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
                    _logger.Warning(Category.BUSINESS, $"Unable to edit comment: {checkResult.ErrorMessage}", "ProjectShowcaseManager");
                    return new(checkResult);
                }

                var editResult = await _projectShowcaseService.EditComment(commentId, commentText).ConfigureAwait(false);
                if (!editResult.IsSuccessful)
                {
                    _logger.Error(Category.BUSINESS, $"Unable to Edit Comment: {editResult.ErrorMessage}", "ProjectShowcaseService");
                    return Result.Failure($"Unable to Delete Showcase");
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
                    _logger.Warning(Category.BUSINESS, $"Unable to verify ownership of showcase: {checkResult.ErrorMessage}", "ProjectShowcaseManager");
                    return new(checkResult);
                }

                var editResult = await _projectShowcaseService.EditShowcase(showcaseId, listingId, title, description).ConfigureAwait(false);
                if (!editResult.IsSuccessful)
                {
                    _logger.Error(Category.BUSINESS, $"Unable to edit showcase: {editResult.ErrorMessage}", "ProjectShowcaseService");
                    return Result.Failure($"Unable to Edit showcase.");
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
                            _logger.Error(Category.BUSINESS, $"Unable to upload new showcase files: {uploadResult.ErrorMessage}", "FileService");
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
                    _logger.Error(Category.BUSINESS, $"Unable to get showcase Details: {testResult.ErrorMessage}", "ProjectShowcaseService");
                    return new(Result.Failure(testResult.ErrorMessage!));
                }
                var checkResult = await VerifyOwnership(showcaseId);
                if (!checkResult.IsSuccessful)
                {
                    _logger.Error(Category.BUSINESS, $"Unable to verify ownership of showcase: {checkResult.ErrorMessage}", "ProjectShowcaseManager");
                    return new(Result.Failure("Unable to verify ownership of showcase."));
                }
                if (!checkResult.Payload && !(bool)testResult.Payload!["IsPublished"])
                {
                    _logger.Warning(Category.BUSINESS, $"Unauthorized attempt to get comments: {checkResult.Payload}", "ProjectShowcaseManager");
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
                    _logger.Error(Category.BUSINESS, $"Unable to verify ownership of showcase: {checkResult.ErrorMessage}", "ProjectShowcaseManager");
                    return new(Result.Failure("Unexpected error when getting comment"));
                }
                if (!checkResult.Payload!)
                {
                    _logger.Warning(Category.BUSINESS, $"Unauthorized attempt to get comments", "ProjectShowcaseManager");
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
                    _logger.Error(Category.BUSINESS, $"Unable to get Showcase Comment Reports: {result.ErrorMessage}", "ProjectShowcaseService");
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

        public async Task<Result<PackagedShowcase>> GetPackagedShowcase(string showcaseId)
        {
            try
            {
                PackagedShowcase output = new();

                var getShowcaseResult = await _projectShowcaseService.GetShowcase(showcaseId).ConfigureAwait(false);
                if (!getShowcaseResult.IsSuccessful)
                {
                    var errorMessage = $"Error in getting Showcase {showcaseId}: {getShowcaseResult.ErrorMessage}";
                    _logger.Warning(Category.BUSINESS, errorMessage);
                    output.Showcase = new();
                }
                else
                {
                    output.Showcase = getShowcaseResult.Payload;
                }

                if (!(bool)getShowcaseResult.Payload!.IsPublished!)
                {
                    var checkResult = await IsAdminOrOwnerOf(showcaseId).ConfigureAwait(false);
                    if (!checkResult.IsSuccessful || !checkResult.Payload)
                    {
                        _logger.Warning(Category.BUSINESS, $"Unable to verify ownership of Showcase: {checkResult.ErrorMessage}");
                        return new(checkResult);
                    }
                }

                Result<List<string>> getFilesResult = await _fileService.GetFilesInDir($"ProjectShowcases/{showcaseId}");
                await _fileService.Disconnect();
                if (!getFilesResult.IsSuccessful)
                {
                    var errorMessage = $"Error in getting files for Showcase {showcaseId}: {getFilesResult.ErrorMessage}";
                    _logger.Warning(Category.BUSINESS, errorMessage);
                    output.FilePaths = new List<string>();
                }
                else
                {
                    output.FilePaths = getFilesResult.Payload;
                }
                

                var getCommentsResult = await _projectShowcaseService.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
                if (!getCommentsResult.IsSuccessful)
                {
                    var errorMessage = $"Error in getting comments for Showcase {showcaseId}: {getFilesResult.ErrorMessage}";
                    _logger.Warning(Category.BUSINESS, errorMessage);
                    output.Comments = new();
                }
                else
                {
                    output.Comments = getCommentsResult.Payload;
                }

                return Result<PackagedShowcase>.Success(output);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting showcase: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in getting showcase"));
            }
        }

        public async Task<Result<Showcase>> GetShowcase(string showcaseId)
        {
            try
            {
                var getShowcaseResult = await _projectShowcaseService.GetShowcase(showcaseId).ConfigureAwait(false);
                if (!getShowcaseResult.IsSuccessful)
                {
                    var errorMessage = $"Error in getting Showcase {showcaseId}: {getShowcaseResult.ErrorMessage}";
                    _logger.Error(Category.BUSINESS, errorMessage, "ProjectShowcaseService");
                    return new(Result.Failure(errorMessage));
                }

                if (!(bool)getShowcaseResult.Payload!.IsPublished!)
                {
                    var checkResult = await IsAdminOrOwnerOf(showcaseId).ConfigureAwait(false);
                    if (!checkResult.IsSuccessful || !checkResult.Payload)
                    {
                        _logger.Error(Category.BUSINESS, $"Unable to verify ownership of showcase: {checkResult.ErrorMessage}", "ProjectShowcaseManager");
                        return new(checkResult);
                    }
                }


                return Result<Showcase>.Success(getShowcaseResult.Payload);
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
                        _logger.Warning(Category.BUSINESS, $"Authorization failure: {authResult.ErrorMessage}", "ProjectShowcaseManager");
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

        public async Task<Result<List<Showcase>>> GetListingShowcases(int listingId)
        {
            try
            {
                return await _projectShowcaseService.GetListingShowcases(listingId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting listing showcases: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in getting listing showcases"));
            }
        }

        public async Task<Result<List<ShowcaseReport>>> GetAllShowcaseReports()
        {
            try
            {
                var authResult = _authorizationService.Authorize(new string[] { "AdminUser" });
                if (!authResult.IsSuccessful)
                {
                    _logger.Warning(Category.BUSINESS, $"Authorization failure: {authResult.ErrorMessage}", "ProjectShowcaseManager");
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
                    _logger.Warning(Category.BUSINESS, $"Authorization failure: {authResult.ErrorMessage}", "ProjectShowcaseManager");
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
                    _logger.Warning(Category.BUSINESS, $"Authorization failure: {authResult.ErrorMessage}", "ProjectShowcaseManager");
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
                    _logger.Error(Category.BUSINESS, $"Unable to verify ownership of showcase: {checkResult.ErrorMessage}", "ProjectShowcaseManager");
                    return new(checkResult);
                }

                var fileResult = await GetShowcaseFiles(showcaseId).ConfigureAwait(false);
                if (!fileResult.IsSuccessful)
                {
                    _logger.Error(Category.BUSINESS, $"Unable to get showcase files: {fileResult.ErrorMessage}", "FileService");
                    return new(Result.Failure("Error in getting showcase files"));
                }
                if (fileResult.Payload!.Count == 0)
                {
                    _logger.Warning(Category.BUSINESS, $"User attempted to upload improper number of files");
                    return new(Result.Failure("Showcase must have at least one file to be published"));
                }

                var detailResult = await _projectShowcaseService.GetDetails(showcaseId).ConfigureAwait(false);
                if (!detailResult.IsSuccessful)
                {
                    _logger.Error(Category.BUSINESS, $"Unabel to get showcase detials: {detailResult.ErrorMessage}", "ProjectShowcaseService");
                    return new(Result.Failure("Unable to get showcase details."));
                }
                if (!detailResult.Payload!.TryGetValue("ListingId", out _) || detailResult.Payload!["ListingId"].GetType() == typeof(DBNull))
                {
                    _logger.Warning(Category.BUSINESS, $"Showcase needs to be linked to listing to be published", "ProjectShowcaseManager");
                    return new(Result.Failure("Showcase needs to be linked to listing to be published"));
                }


                var histResult = await _listingProfileService.CheckListingHistory((int)detailResult.Payload!["ListingId"], int.Parse((Thread.CurrentPrincipal as ClaimsPrincipal)?.FindFirstValue("sub")!)).ConfigureAwait(false);
                if (!histResult.IsSuccessful)
                {
                    _logger.Error(Category.BUSINESS, $"Unable to check listing hsitory: {histResult.ErrorMessage}");
                    return new(Result.Failure("Unable to check listing history"));
                }
                if (!histResult.Payload)
                {
                    _logger.Warning(Category.BUSINESS, $"invalid attempt to publish", "ProjectShowcaseManager");
                    await Unlink(showcaseId).ConfigureAwait(false);
                    return new(Result.Failure("Listing no longer available, please refresh. If this seems to be incorrect, please try again later or contact an admin."));
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
                    _logger.Warning(Category.BUSINESS, $"Authorization failure: {authResult.ErrorMessage}", "ProjectShowcaseManager");
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
                    _logger.Warning(Category.BUSINESS, $"Authorization failure: {authResult.ErrorMessage}", "ProjectShowcaseManager");
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
                    _logger.Warning(Category.BUSINESS, $"Authorization failure: {authResult.ErrorMessage}", "ProjectShowcaseManager");
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
                    _logger.Error(Category.BUSINESS, $"Unable to verify ownership of showcase: {checkResult.ErrorMessage}", "ProjectShowcaseManager");
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

        public async Task<Result> Link(string showcaseId, int listingId)
        {
            try
            {
                var checkResult = await IsAdminOrOwnerOf(showcaseId).ConfigureAwait(false);
                if (!checkResult.IsSuccessful || !checkResult.Payload)
                {
                    _logger.Error(Category.BUSINESS, $"Unable to verify ownership of showcase: {checkResult.ErrorMessage}", "ProjectShowcaseManager");
                    return checkResult;
                }
                var checkResult2 = await _listingProfileService.CheckListingHistory(listingId, int.Parse((Thread.CurrentPrincipal as ClaimsPrincipal)?.FindFirstValue("sub")!)).ConfigureAwait(false);
                if (!checkResult2.IsSuccessful)
                {
                    _logger.Error(Category.BUSINESS, $"Unable to verify user history with listing: {checkResult2.ErrorMessage}", "ProjectShwocaseManager");
                    return new(Result.Failure("Unable to verify user history with listing"));
                }
                if (!checkResult2.Payload!)
                {
                    _logger.Warning(Category.BUSINESS, $"Unable to verify user history with listing.", "ProjectShowcaseManager");
                    return new(Result.Failure("Unable to verify user history with listing. A user can only showcase their work on a listing if they have a recorded history with it.", 400));
                }

                return await _projectShowcaseService.Link(showcaseId, listingId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in linking showcase: {ex.Message}", "ShowcaseManager");
                return new(Result.Failure("Error in linking showcase"));
            }
        }

        public async Task<Result> Unpublish(string showcaseId)
        {
            try
            {
                var checkResult = await IsAdminOrOwnerOf(showcaseId).ConfigureAwait(false);
                if (!checkResult.IsSuccessful || !checkResult.Payload)
                {
                    _logger.Error(Category.BUSINESS, $"Unable to verify ownership of showcase: {checkResult.ErrorMessage}", "ProjectShowcaseManager");
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

﻿using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.ProjectShowcase.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using Microsoft.Identity.Client;
using System.Drawing.Printing;
using DevelopmentHell.Hubba.SqlDataAccess;

namespace DevelopmentHell.Hubba.ProjectShowcase.Service.Implementations
{
    public class ProjectShowcaseService : IProjectShowcaseService
    {
        private readonly IProjectShowcaseDataAccess _projectShowcaseDataAccess;
        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IValidationService _validationService;
        private readonly ILoggerService _logger;

        public ProjectShowcaseService(IProjectShowcaseDataAccess projectShowcaseDataAccess, IUserAccountDataAccess userAccountDataAccess, IValidationService validationService, ILoggerService loggerService) 
        {
            _userAccountDataAccess = userAccountDataAccess;
            _projectShowcaseDataAccess = projectShowcaseDataAccess;
            _validationService = validationService;
            _logger = loggerService;
        }

        private T MapToType<T>(Dictionary<string,string> attToSqlVarMap, Dictionary<string, object> values) where T : new()
        {
            T outClass = new();
            foreach (var attToSqlVar in attToSqlVarMap)
            {
                if (values.TryGetValue(attToSqlVar.Value, out _))
                {
                    var prop = typeof(T).GetProperty(attToSqlVar.Key);
                    prop!.SetValue(outClass, Convert.ChangeType(values[attToSqlVar.Value], prop!.PropertyType));
                }
            }
            return outClass;
        }

        public async Task<Result> AddComment(string showcaseId, string commentText)
        {
            if (!_validationService.ValidateBodyText(commentText).IsSuccessful)
            {
                return new(Result.Failure("Invalid comment.", 400));
            }

            try
            {
                int accountId = int.Parse((Thread.CurrentPrincipal as ClaimsPrincipal)?.FindFirstValue("sub")!);
                var insertResult = await _projectShowcaseDataAccess.AddComment(showcaseId, accountId, commentText, DateTime.UtcNow).ConfigureAwait(false);
                if (!insertResult.IsSuccessful)
                {
                    return new(Result.Failure($"Error in adding comment: {insertResult.ErrorMessage}"));
                }
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in adding comment: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in adding comment"));
            }
        }

        public async Task<Result<string>> CreateShowcase(int listingId, string title, string description)
        {
            try
            {
                if (!_validationService.ValidateTitle(title).IsSuccessful)
                {
                    return new(Result.Failure("Invalid title.",400));
                }
                if (!_validationService.ValidateBodyText(description).IsSuccessful)
                {
                    return new(Result.Failure("Invalid body.",400));
                }

                int accountId = int.Parse((Thread.CurrentPrincipal as ClaimsPrincipal)?.FindFirstValue("sub")!);
                string generatedId = System.Convert.ToBase64String(Encoding.ASCII.GetBytes($"{accountId}:{DateTime.UtcNow}"));


                var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, generatedId, listingId, title, description, DateTime.UtcNow).ConfigureAwait(false);
                if (!insertResult.IsSuccessful)
                {
                    return new(Result.Failure(insertResult.ErrorMessage!));
                }

                return Result<string>.Success(generatedId);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in creating showcase: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in creating showcase"));
            }
        }

        public async Task<Result> DeleteComment(int commentId)
        {
            try
            {
                if (commentId < 0)
                {
                    return Result.Failure("commentId is out of range", 400);
                }

                var deleteResult = await _projectShowcaseDataAccess.DeleteComment(commentId).ConfigureAwait(false);
                if (!deleteResult.IsSuccessful)
                {
                    return Result.Failure(deleteResult.ErrorMessage!);
                }
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in deleting comment: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in creating showcase"));
            }
        }

        public async Task<Result> DeleteShowcase(string showcaseId)
        {
            try
            {
                if (showcaseId == "")
                {
                    return new(Result.Failure("Showcase cannot be empty", 400));
                }

                var deleteResult = await _projectShowcaseDataAccess.DeleteShowcase(showcaseId).ConfigureAwait(false);
                if (!deleteResult.IsSuccessful)
                {
                    return new(Result.Failure($"Error in Deleting showcase: {deleteResult.ErrorMessage}"));
                }
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in deleting showcase: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in deleting showcase"));
            }
        }

        public async Task<Result> EditComment(int commentId, string commentText)
        {
            try
            {
                if (commentId < 0)
                {
                    return Result.Failure("Comment Id is out of range", 400);
                }
                if (commentText == null || commentText.Length == 0)
                {
                    return Result.Failure("comment text cannot be empty", 400);
                }

                var editResult = await _projectShowcaseDataAccess.UpdateComment(commentId, commentText, DateTime.UtcNow).ConfigureAwait(false);
                if (!editResult.IsSuccessful)
                {
                    return Result.Failure("Error in editing comment.");
                }
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in editing showcase: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in editing showcase"));
            }
        }

        public async Task<Result> EditShowcase(string showcaseId, int? listingId = null, string? title = null, string? description = null)
        {
            try
            {
                if (title != null && !_validationService.ValidateTitle(title).IsSuccessful)
                {
                    return new(Result.Failure("Invalid title.", 400));
                }
                if (description != null && !_validationService.ValidateBodyText(description).IsSuccessful)
                {
                    return new(Result.Failure("Invalid body.", 400));
                }

                var editResult = await _projectShowcaseDataAccess.EditShowcase(showcaseId, title, description, DateTime.UtcNow).ConfigureAwait(false);
                if (!editResult.IsSuccessful)
                {
                    return Result.Failure(editResult.ErrorMessage!);
                }

                return Result.Success();    
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in editing showcase: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in editing showcase"));
            }
        }

        public async Task<Result<Dictionary<string, object>>> GetCommentDetails(int commentId)
        {
            try
            {
                var getResult = await _projectShowcaseDataAccess.GetCommentDetails(commentId).ConfigureAwait(false);
                if (!getResult.IsSuccessful)
                {
                    return new(Result.Failure($"Error in retrieving details from DAC: {getResult.ErrorMessage}"));
                }
                return getResult;
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting comment details: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in getting comment details"));
            }
        }

        public async Task<Result<List<ShowcaseComment>>> GetComments(string showcaseId, int commentCount, int page)
        {
            try
            {
                var getResult = await _projectShowcaseDataAccess.GetComments(showcaseId, commentCount, page);
                if (!getResult.IsSuccessful)
                {
                    return new(Result.Failure("Unable to get comments from DAC"));
                }

                List<ShowcaseComment> output = new();
                Dictionary<string, string> varSqlVarMap = new()
                {
                    { "CommenterId", "CommenterId" },
                    { "ShowcaseId", "ShowcaseId" },
                    { "Id", "Id" },
                    { "Text", "Text" },
                    { "Rating", "Rating" },
                    { "Timestamp", "Timestamp" },
                    { "EditTimestamp", "EditTimestamp" },
                };

                foreach (var commentDict in getResult.Payload!)
                {
                    ShowcaseComment nextComment = MapToType<ShowcaseComment>(varSqlVarMap, commentDict);
                    var userResult = await _userAccountDataAccess.GetUser((int)nextComment.Id!).ConfigureAwait(false);
                    if (!userResult.IsSuccessful)
                    {
                        return new(Result.Failure($"Unable to get user email: {userResult.ErrorMessage}"));
                    }
                    nextComment.CommenterEmail = userResult.Payload!.Email;
                    output.Add(nextComment);
                }

                return Result<List<ShowcaseComment>>.Success(output);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting comments: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in getting comments"));
            }
        }

        public async Task<Result<Dictionary<string, object>>> GetDetails(string showcaseId)
        {
            try
            {
                var getResult = await _projectShowcaseDataAccess.GetDetails(showcaseId).ConfigureAwait(false);
                if (!getResult.IsSuccessful)
                {
                    return new(Result.Failure($"Error in retrieving details from DAC: {getResult.ErrorMessage}"));
                }
                return getResult;
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting showcase details: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in getting showcase details"));
            }
        }

        public async Task<Result<Showcase>> GetShowcase(string showcaseId)
        {
            try
            {
                var getResult = await _projectShowcaseDataAccess.GetShowcase(showcaseId).ConfigureAwait(false);
                if (!getResult.IsSuccessful)
                {
                    return new(Result.Failure($"Error in getting Showcase from DAC: {getResult.ErrorMessage}"));
                }
                Showcase output = new();
                Dictionary<string, string> varSqlVarMap = new()
                {
                    { "Id", "Id" },
                    { "ShowcaseUserId", "ShowcaseUserId" },
                    { "ListingId", "ListingId" },
                    { "Title", "Title" },
                    { "Description", "Description" },
                    { "IsPublished", "IsPublished" },
                    { "Rating", "Rating" },
                    { "PublishTimestamp","PublishTimestamp" },
                    { "EditTimestamp", "EditTimestamp" }
                };
                foreach (var varSqlVar in varSqlVarMap)
                {
                    object? test = null;
                    if (!getResult.Payload!.TryGetValue(varSqlVar.Value, out test))
                    {
                        // Set all of the attributes in the output var to the ones in the dict from getResult.Payload
                        var prop = typeof(Showcase).GetProperty(varSqlVar.Key);
                        prop!.SetValue(output, Convert.ChangeType(getResult.Payload![varSqlVar.Value], prop!.PropertyType));
                    }
                }

                Showcase nextShowcase = MapToType<Showcase>(varSqlVarMap, getResult.Payload!);
                var userResult = await _userAccountDataAccess.GetUser((int)nextShowcase.ShowcaseUserId!).ConfigureAwait(false);
                if (!userResult.IsSuccessful)
                {
                    return new(Result.Failure($"Unable to get user email: {userResult.ErrorMessage}"));
                }
                nextShowcase.ShowcaseUserEmail = userResult.Payload!.Email;

                return Result<Showcase>.Success(nextShowcase);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting showcase: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in getting showcase"));
            }
        }


        public async Task<Result<List<Showcase>>> GetUserShowcases(int accountId, bool includeDetails = false)
        {
            try
                {
                var getResult = await _projectShowcaseDataAccess.GetUserShowcases(accountId).ConfigureAwait(false);
                if (!getResult.IsSuccessful)
                    {
                    return new(Result.Failure($"Error in getting showcases from DAC: {getResult.ErrorMessage}"));
                }
                List<Showcase> output = new();
                Dictionary<string, string> varSqlVarMap = new()
                {
                    { "Id", "Id" },
                    { "ShowcaseUserId", "ShowcaseUserId" },
                    { "ListingId", "ListingId" },
                    { "Title", "Title" },
                    { "Description", "Description" },
                    { "IsPublished", "IsPublished" },
                    { "Rating", "Rating" },
                    { "PublishTimestamp","PublishTimestamp" },
                    { "EditTimestamp", "EditTimestamp" }
                };
                foreach (var showcaseDict in getResult.Payload!)
                    
                {
                    Showcase nextShowcase = MapToType<Showcase>(varSqlVarMap, showcaseDict);
                    var userResult = await _userAccountDataAccess.GetUser((int)nextShowcase.ShowcaseUserId!).ConfigureAwait(false);
                    if (!userResult.IsSuccessful)
                    {
                        return new(Result.Failure($"Unable to get user email: {userResult.ErrorMessage}"));
                    }
                    nextShowcase.ShowcaseUserEmail = userResult.Payload!.Email;
                    output.Add(nextShowcase);
                }
                return Result<List<Showcase>>.Success(output);
            }
            catch (Exception ex)
                {
                _logger.Warning(Category.BUSINESS, $"Error in getting showcases: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in getting showcases"));
            }
        }

        public async Task<Result<float>> LikeShowcase(string showcaseId)
        {
            try
            {
                var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
                int accountId = int.Parse(claimsPrincipal?.FindFirstValue("sub")!);

                var userLikeResult = await _projectShowcaseDataAccess.RecordUserLike(accountId, showcaseId).ConfigureAwait(false);
                if (!userLikeResult.IsSuccessful)
                {
                    return new(Result.Failure($"Unable to like showcase: {userLikeResult.ErrorMessage}"));
                }

                var incrementLikeResult = await _projectShowcaseDataAccess.IncrementShowcaseLikes(showcaseId).ConfigureAwait(false);
                if (!incrementLikeResult.IsSuccessful)
                {
                    return new(Result.Failure($"Error in incrementing showcase likes: {incrementLikeResult.ErrorMessage}"));
                }

                return new()
                {
                    IsSuccessful = true,
                    Payload = incrementLikeResult.Payload!
                };
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in liking showcase: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in liking showcase"));
            }
        }

        public async Task<Result> Publish(string showcaseId, int? listingId)
        {
            try
            {
                if (listingId != null)
                {
                    var updateResult = await EditShowcase(showcaseId, listingId).ConfigureAwait(false);
                    if (!updateResult.IsSuccessful)
                    {
                        return Result.Failure("Unable to Edit showcase for publishing");
                    }
                }

                var pubResult = await _projectShowcaseDataAccess.ChangePublishStatus(showcaseId, true, DateTime.UtcNow).ConfigureAwait(false);
                if (!pubResult.IsSuccessful)
                {
                    return Result.Failure("Unable to upate publish status");
                }

                return new()
                {
                    IsSuccessful = true,
                };
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in publishing showcase: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in publishing showcase"));
            }
        }

        public async Task<Result<int>> RateComment(int commentId, bool isUpvote)
        {
            try
            {
                int accountId = int.Parse((Thread.CurrentPrincipal as ClaimsPrincipal)?.FindFirstValue("sub")!);
                var getResult = await _projectShowcaseDataAccess.GetCommentUserRating(commentId, accountId);
                if (!getResult.IsSuccessful)
                {
                    return new(getResult);
                }
                int updateAmount = 0;
                // if user has not rated yet
                if (getResult.Payload == null)
                {
                    var insertUserResult = await _projectShowcaseDataAccess.InsertUserCommentRating(commentId, accountId, isUpvote);
                    if (!insertUserResult.IsSuccessful)
                    {
                        return new(Result.Failure("Error inserting user's comment rating"));
                    }
                    updateAmount = 1;
                }
                else if (getResult.Payload != isUpvote)
                {
                    var updateUserResult = await _projectShowcaseDataAccess.UpdateUserCommentRating(commentId, accountId, isUpvote);
                    if (!updateUserResult.IsSuccessful)
                    {
                        return new(Result.Failure("Error updating user's comment rating"));
                    }
                    updateAmount = 2;
                }

                if (updateAmount > 0)
                {
                    var updateCommentResult = await _projectShowcaseDataAccess.UpdateCommentRating(commentId, (isUpvote ? updateAmount : -updateAmount));
                    if (!updateCommentResult.IsSuccessful)
                    {
                        return new(Result.Failure("Error updating comment rating"));
                    }
                    return Result<int>.Success(updateCommentResult.Payload);
                }
                return new(Result.Failure("Rating is unchanged.",400));
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in rating comment: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in rating comment"));
            }
        }

        public async Task<Result> ReportComment(int commentId, string reasonText)
        {
            try
            {
                int accountId = int.Parse((Thread.CurrentPrincipal as ClaimsPrincipal)?.FindFirstValue("sub")!);
                var reportResult = await _projectShowcaseDataAccess.AddCommentReport(commentId, accountId, reasonText, DateTime.Now);
                if (!reportResult.IsSuccessful)
                {
                    return Result.Failure("Error in reporting comment");
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in reporting comment: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in reporting comment"));
            }
        }

        public async Task<Result> ReportShowcase(string showcaseId, string reasonText)
        {
            try
            {
                int accountId = int.Parse((Thread.CurrentPrincipal as ClaimsPrincipal)?.FindFirstValue("sub")!);
                var reportResult = await _projectShowcaseDataAccess.AddShowcaseReport(showcaseId, accountId, reasonText, DateTime.Now);
                if (! reportResult.IsSuccessful)
                {
                    return Result.Failure("Error in reporting showcase");
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in reporting showcase: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in reporting showcase"));
            }
        }

        public async Task<Result> Unlink(string showcaseId)
        {
            try
            {
                var unlinkResult = await _projectShowcaseDataAccess.RemoveShowcaseListing(showcaseId);
                if (! unlinkResult.IsSuccessful)
                {
                    return unlinkResult;
                }
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in unlinking showcase: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in unlinking showcase"));
            }
        }

        public async Task<Result> Unpublish(string showcaseId)
        {
            try
            {
                var unpubResult = await _projectShowcaseDataAccess.ChangePublishStatus(showcaseId, false).ConfigureAwait(false);
                if (!unpubResult.IsSuccessful)
                {
                    return Result.Failure("Unable to upate publish status");
                }

                return new()
                {
                    IsSuccessful = true,
                };
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in unpublishing showcase: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in unpublishing showcase"));
            }
        }

        public async Task<Result<List<ShowcaseReport>>> GetAllShowcaseReports()
        {
            try
            {
                var getResult = await _projectShowcaseDataAccess.GetAllShowcaseReports().ConfigureAwait(false);
                if (!getResult.IsSuccessful)
                {
                    return new(Result.Failure($"Error in getting showcases from DAC: {getResult.ErrorMessage}"));
                }
                List<ShowcaseReport> output = new();
                Dictionary<string, string> varSqlVarMap = new()
                {
                    { "Timestamp", "Timestamp" },
                    { "Reason", "Reason" },
                    { "ShowcaseId", "ShowcaseId" },
                    { "ReporterId", "ReporterId" },
                    { "IsResolved", "IsResolved" }
                };
                foreach (var showcaseDict in getResult.Payload!)
                {
                    output.Add(MapToType<ShowcaseReport>(varSqlVarMap, showcaseDict));
                }
                return Result<List<ShowcaseReport>>.Success(output);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting showcases: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in getting showcases"));
            }
        }

        public async Task<Result<List<ShowcaseReport>>> GetShowcaseReports(string showcaseId)
        {
            try
            {
                var getResult = await _projectShowcaseDataAccess.GetShowcaseReports(showcaseId).ConfigureAwait(false);
                if (!getResult.IsSuccessful)
                {
                    return new(Result.Failure($"Error in getting showcases from DAC: {getResult.ErrorMessage}"));
                }
                List<ShowcaseReport> output = new();
                Dictionary<string, string> varSqlVarMap = new()
                {
                    { "Timestamp", "Timestamp" },
                    { "Reason", "Reason" },
                    { "ShowcaseId", "ShowcaseId" },
                    { "ReporterId", "ReporterId" },
                    { "IsResolved", "IsResolved" }
                };
                foreach (var showcaseDict in getResult.Payload!)
                {
                    output.Add(MapToType<ShowcaseReport>(varSqlVarMap, showcaseDict));
                }
                return Result<List<ShowcaseReport>>.Success(output);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting showcases: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in getting showcases"));
            }
        }

        public async Task<Result<List<CommentReport>>> GetAllCommentReports()
        {
            try
            {
                var getResult = await _projectShowcaseDataAccess.GetAllCommentReports().ConfigureAwait(false);
                if (!getResult.IsSuccessful)
                {
                    return new(Result.Failure($"Error in getting showcases from DAC: {getResult.ErrorMessage}"));
                }
                List<CommentReport> output = new();
                Dictionary<string, string> varSqlVarMap = new()
                {
                    { "Timestamp", "Timestamp" },
                    { "Reason", "Reason" },
                    { "CommentId", "CommentId" },
                    { "ReporterId", "ReporterId" },
                    { "IsResolved", "IsResolved" }
                };
                foreach (var showcaseDict in getResult.Payload!)
                {
                    output.Add(MapToType<CommentReport>(varSqlVarMap, showcaseDict));
                }
                return Result<List<CommentReport>>.Success(output);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting showcases: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in getting showcases"));
            }
        }

        public async Task<Result<List<CommentReport>>> GetCommentReports(int commentId)
        {
            try
            {
                var getResult = await _projectShowcaseDataAccess.GetCommentReports(commentId).ConfigureAwait(false);
                if (!getResult.IsSuccessful)
                {
                    return new(Result.Failure($"Error in getting showcases from DAC: {getResult.ErrorMessage}"));
                }
                List<CommentReport> output = new();
                Dictionary<string, string> varSqlVarMap = new()
                {
                    { "Timestamp", "Timestamp" },
                    { "Reason", "Reason" },
                    { "CommentId", "CommentId" },
                    { "ReporterId", "ReporterId" },
                    { "IsResolved", "IsResolved" }
                };
                foreach (var showcaseDict in getResult.Payload!)
                {
                    CommentReport showcase = new();
                    foreach (var varSqlVar in varSqlVarMap)
                    {
                        output.Add(MapToType<CommentReport>(varSqlVarMap, showcaseDict));
                    }
                    output.Add(showcase);
                }
                return Result<List<CommentReport>>.Success(output);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting showcases: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in getting showcases"));
            }
        }
    }
}

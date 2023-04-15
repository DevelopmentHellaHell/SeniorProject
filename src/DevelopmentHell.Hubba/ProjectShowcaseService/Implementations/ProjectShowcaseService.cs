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

namespace DevelopmentHell.Hubba.ProjectShowcase.Service.Implementations
{
    public class ProjectShowcaseService : IProjectShowcaseService
    {
        private readonly IProjectShowcaseDataAccess _projectShowcaseDataAccess;
        private readonly IValidationService _validationService;
        private readonly ILoggerService _logger;

        ProjectShowcaseService(IProjectShowcaseDataAccess projectShowcaseDataAccess, IValidationService validationService, ILoggerService loggerService) 
        { 
            _projectShowcaseDataAccess = projectShowcaseDataAccess;
            _validationService = validationService;
            _logger = loggerService;
        }

        public async Task<Result> AddComment(string showcaseId, string commentText)
        {
            throw new NotImplementedException();
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
                _logger.Warning(Category.BUSINESS, "Error in creating showcase", "ShowcaseService");
                return new(Result.Failure("Error in creating showcase"));
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
                _logger.Warning(Category.BUSINESS, "Error in deleting showcase", "ShowcaseService");
                return new(Result.Failure("Error in deleting showcase"));
            }
        }

        public async Task<Result> EditComment(int commentId, string commentText)
        {
            throw new NotImplementedException();
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
                _logger.Warning(Category.BUSINESS, "Error in editing showcase", "ShowcaseService");
                return new(Result.Failure("Error in editing showcase"));
            }
        }

        public async Task<Result<Dictionary<string, object>>> GetCommentDetails(string showcaseId)
        {
            throw new NotImplementedException();
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
                { "CommenterEmail", "Email" },
                { "ShowcaseId", "ShowcaseId" },
                { "Id", "ShowcaseComments.Id" },
                { "Text", "Text" },
                { "Rating", "Rating" },
                { "Timestamp", "Timestamp" },
                { "EditTimestamp", "EditTimestamp" },
            };

                foreach (var commentDict in getResult.Payload!)
                {
                    ShowcaseComment comment = new();
                    foreach (var varSqlVar in varSqlVarMap)
                    {
                        object? test = null;
                        if (!commentDict.TryGetValue(varSqlVar.Value, out test))
                        {
                            // Set all of the attributes in the output var to the ones in the dict from getResult.Payload
                            var prop = typeof(ShowcaseComment).GetProperty(varSqlVar.Key);
                            prop!.SetValue(comment, Convert.ChangeType(commentDict[varSqlVar.Value], prop!.PropertyType));
                        }
                    }
                    output.Add(comment);
                }

                return Result<List<ShowcaseComment>>.Success(output);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, "Error in getting comments", "ShowcaseService");
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
            catch
            {
                _logger.Warning(Category.BUSINESS, "Error in getting showcase details", "ShowcaseService");
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
                { "Id", "Showcases.Id" },
                { "ShowcaseUserEmail", "Email" },
                { "ShowcaseUserId", "UserAccounts.Id" },
                { "ListingId", "ListingId" },
                { "Title", "Title" },
                { "Description", "Description" },
                { "IsPublished", "IsPublished" },
                { "Rating", "Rating" },
                { "PublsihTimestamp","PublishTimestamp" },
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

                return Result<Showcase>.Success(output);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, "Error in getting showcase", "ShowcaseService");
                return new(Result.Failure("Error in getting showcase"));
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
                _logger.Warning(Category.BUSINESS, "Error in liking showcase", "ShowcaseService");
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
                _logger.Warning(Category.BUSINESS, "Error in publishing showcase", "ShowcaseService");
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
    }
}

using System.Security.Claims;
using System.Text;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.ProjectShowcase.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;

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


        private Dictionary<string, object> MapToAttDict(Dictionary<string, string> attToSqlVarMap, Dictionary<string, object> values)
        {
            Dictionary<string, object> nextDict = new Dictionary<string, object>();
            foreach (var attToSqlVar in attToSqlVarMap)
            {
                object? mappedVar;
                if (values.TryGetValue(attToSqlVar.Value, out mappedVar))
                {
                    nextDict[attToSqlVar.Key] = mappedVar;
                }
                else
                {
                    nextDict[attToSqlVar.Key] = null;
                }
            }
            return nextDict;
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
                    return new(Result.Failure("Invalid title.", 400));
                }
                if (!_validationService.ValidateBodyText(description).IsSuccessful)
                {
                    return new(Result.Failure("Invalid body.", 400));
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

        public async Task<Result> DeleteComment(long commentId)
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

        public async Task<Result> EditComment(long commentId, string commentText)
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

        public async Task<Result<Dictionary<string, object>>> GetCommentDetails(long commentId)
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
                    var valDict = MapToAttDict(varSqlVarMap, commentDict);
                    ShowcaseComment nextComment = new();
                    nextComment.CommenterId = valDict["CommenterId"].GetType() == typeof(DBNull) ? null : (int)valDict["CommenterId"];
                    nextComment.ShowcaseId = valDict["ShowcaseId"].GetType() == typeof(DBNull) ? null : (string)valDict["ShowcaseId"];
                    nextComment.Id = valDict["Id"].GetType() == typeof(DBNull) ? null : (long)valDict["Id"];
                    nextComment.Text = valDict["Text"].GetType() == typeof(DBNull) ? null : (string)valDict["Text"];
                    nextComment.Rating = valDict["Rating"].GetType() == typeof(DBNull) ? null : (int)valDict["Rating"];
                    nextComment.Timestamp = valDict["Timestamp"].GetType() == typeof(DBNull) ? null : (DateTime)valDict["Timestamp"];
                    nextComment.EditTimestamp = valDict["EditTimestamp"].GetType() == typeof(DBNull) ? null : (DateTime?)valDict["EditTimestamp"];


                    var userResult = await _userAccountDataAccess.GetUser((int)nextComment.CommenterId!).ConfigureAwait(false);
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

        public async Task<Result<ShowcaseComment>> GetComment(long commentId)
        {
            try
            {
                var getResult = await _projectShowcaseDataAccess.GetComment(commentId);
                if (!getResult.IsSuccessful)
                {
                    return new(Result.Failure("Unable to get comment from DAC"));
                }

                Dictionary<string, string> varSqlVarMap = new()
                {
                    { "CommenterId", "CommenterId" },
                    { "ShowcaseId", "ShowcaseId" },
                    { "Id", "Id" },
                    { "Text", "Text" },
                    { "Rating", "Rating" },
                    { "Timestamp", "Timestamp" },
                    { "EditTimestamp", "EditTimestamp" }
                };

                var valDict = MapToAttDict(varSqlVarMap, getResult.Payload!);
                ShowcaseComment nextComment = new();
                nextComment.CommenterId = valDict["CommenterId"].GetType() == typeof(DBNull) ? null : (int)valDict["CommenterId"];
                nextComment.ShowcaseId = valDict["ShowcaseId"].GetType() == typeof(DBNull) ? null : (string)valDict["ShowcaseId"];
                nextComment.Id = valDict["Id"].GetType() == typeof(DBNull) ? null : (long)valDict["Id"];
                nextComment.Text = valDict["Text"].GetType() == typeof(DBNull) ? null : (string)valDict["Text"];
                nextComment.Rating = valDict["Rating"].GetType() == typeof(DBNull) ? null : (int)valDict["Rating"];
                nextComment.Timestamp = valDict["Timestamp"].GetType() == typeof(DBNull) ? null : (DateTime)valDict["Timestamp"];
                nextComment.EditTimestamp = valDict["EditTimestamp"].GetType() == typeof(DBNull) ? null : (DateTime?)valDict["EditTimestamp"];

                var userResult = await _userAccountDataAccess.GetUser((int)nextComment.CommenterId!).ConfigureAwait(false);
                if (!userResult.IsSuccessful)
                {
                    return new(Result.Failure($"Unable to get user email: {userResult.ErrorMessage}"));
                }
                nextComment.CommenterEmail = userResult.Payload!.Email;

                return Result<ShowcaseComment>.Success(nextComment);
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

                var valDict = MapToAttDict(varSqlVarMap, getResult.Payload!);
                Showcase nextShowcase = new();
                nextShowcase.PublishTimestamp = valDict["PublishTimestamp"].GetType() == typeof(DBNull) ? null : (DateTime)valDict["PublishTimestamp"];
                nextShowcase.IsPublished = valDict["IsPublished"].GetType() == typeof(DBNull) ? null : (bool)valDict["IsPublished"];
                nextShowcase.EditTimestamp = valDict["EditTimestamp"].GetType() == typeof(DBNull) ? null : (DateTime)valDict["EditTimestamp"];
                nextShowcase.Title = valDict["Title"].GetType() == typeof(DBNull) ? null : (string)valDict["Title"];
                nextShowcase.Description = valDict["Description"].GetType() == typeof(DBNull) ? null : (string)valDict["Description"];
                nextShowcase.ListingId = valDict["ListingId"].GetType() == typeof(DBNull) ? null : (int)valDict["ListingId"];
                nextShowcase.Rating = valDict["Rating"].GetType() == typeof(DBNull) ? null : (double)valDict["Rating"];
                nextShowcase.Id = valDict["Id"].GetType() == typeof(DBNull) ? null : (string)valDict["Id"];
                nextShowcase.ShowcaseUserId = valDict["ShowcaseUserId"].GetType() == typeof(DBNull) ? null : (int)valDict["ShowcaseUserId"];

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
                    var valDict = MapToAttDict(varSqlVarMap, showcaseDict);
                    Showcase nextShowcase = new();
                    nextShowcase.PublishTimestamp = valDict["PublishTimestamp"].GetType() == typeof(DBNull) ? null : (DateTime)valDict["PublishTimestamp"];
                    nextShowcase.IsPublished = valDict["IsPublished"].GetType() == typeof(DBNull) ? null : (bool)valDict["IsPublished"];
                    nextShowcase.EditTimestamp = valDict["EditTimestamp"].GetType() == typeof(DBNull) ? null : (DateTime)valDict["EditTimestamp"];
                    nextShowcase.Title = valDict["Title"].GetType() == typeof(DBNull) ? null : (string)valDict["Title"];
                    nextShowcase.Description = valDict["Description"].GetType() == typeof(DBNull) ? null : (string)valDict["Description"];
                    nextShowcase.ListingId = valDict["ListingId"].GetType() == typeof(DBNull) ? null : (int)valDict["ListingId"];
                    nextShowcase.Rating = valDict["Rating"].GetType() == typeof(DBNull) ? null : (double)valDict["Rating"];
                    nextShowcase.Id = valDict["Id"].GetType() == typeof(DBNull) ? null : (string)valDict["Id"];
                    nextShowcase.ShowcaseUserId = valDict["ShowcaseUserId"].GetType() == typeof(DBNull) ? null : (int)valDict["ShowcaseUserId"];

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

        public async Task<Result<List<Showcase>>> GetListingShowcases(int listingId)
        {
            try
            {
                var getResult = await _projectShowcaseDataAccess.GetListingShowcases(listingId).ConfigureAwait(false);
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
                    var valDict = MapToAttDict(varSqlVarMap, showcaseDict);
                    Showcase nextShowcase = new();
                    nextShowcase.PublishTimestamp = valDict["PublishTimestamp"].GetType() == typeof(DBNull) ? null : (DateTime)valDict["PublishTimestamp"];
                    nextShowcase.IsPublished = valDict["IsPublished"].GetType() == typeof(DBNull) ? null : (bool)valDict["IsPublished"];
                    nextShowcase.EditTimestamp = valDict["EditTimestamp"].GetType() == typeof(DBNull) ? null : (DateTime)valDict["EditTimestamp"];
                    nextShowcase.Title = valDict["Title"].GetType() == typeof(DBNull) ? null : (string)valDict["Title"];
                    nextShowcase.ListingId = valDict["ListingId"].GetType() == typeof(DBNull) ? null : (int)valDict["ListingId"];
                    nextShowcase.Rating = valDict["Rating"].GetType() == typeof(DBNull) ? null : (double)valDict["Rating"];
                    nextShowcase.Id = valDict["Id"].GetType() == typeof(DBNull) ? null : (string)valDict["Id"];
                    nextShowcase.ShowcaseUserId = valDict["ShowcaseUserId"].GetType() == typeof(DBNull) ? null : (int)valDict["ShowcaseUserId"];

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

        public async Task<Result<double>> LikeShowcase(string showcaseId)
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
                var selectResult = await _projectShowcaseDataAccess.GetShowcase(showcaseId);
                if (!selectResult.IsSuccessful || selectResult.Payload == null)
                {
                    return Result.Failure("Unable to check if showcase is in state to be published.");
                }

                if (selectResult.Payload != null)
                {
                    if (selectResult.Payload["Description"].ToString()!.Length < 250)
                    {
                        return Result.Failure("Description needs to be at least 250 characters", 400);
                    }
                    if (selectResult.Payload["Title"].ToString()!.Length < 5)
                    {
                        return Result.Failure("Title needs to be at least 5 characters", 400);
                    }
                    if (selectResult.Payload!["ListingId"] == DBNull.Value)
                    {
                        return Result.Failure("Showcase needs to be linked to listing to be published", 400);
                    }
                }

                var pubResult = await _projectShowcaseDataAccess.ChangePublishStatus(showcaseId, true, DateTime.UtcNow).ConfigureAwait(false);
                if (!pubResult.IsSuccessful)
                {
                    return Result.Failure("Unable to update publish status");
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

        public async Task<Result<int>> RateComment(long commentId, bool isUpvote)
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
                return new(Result.Failure("Rating is unchanged.", 400));
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in rating comment: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in rating comment"));
            }
        }

        public async Task<Result> ReportComment(long commentId, string reasonText)
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
                if (!reportResult.IsSuccessful)
                {
                    return Result.Failure("Error in reporting showcase", 400);
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
                if (!unlinkResult.IsSuccessful)
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

        public async Task<Result> Link(string showcaseId, int listingId)
        {
            try
            {
                var linkResult = await _projectShowcaseDataAccess.LinkShowcaseListing(showcaseId, listingId);
                if (!linkResult.IsSuccessful)
                {
                    return linkResult;
                }
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in linking showcase: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in linking showcase"));
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
                    var valDict = MapToAttDict(varSqlVarMap, showcaseDict);
                    ShowcaseReport nextReport = new();
                    nextReport.Timestamp = valDict["Timestamp"].GetType() == typeof(DBNull) ? null : (DateTime)valDict["Timestamp"];
                    nextReport.Reason = valDict["Reason"].GetType() == typeof(DBNull) ? null : (string)valDict["Reason"];
                    nextReport.ShowcaseId = valDict["ShowcaseId"].GetType() == typeof(DBNull) ? null : (string)valDict["ShowcaseId"];
                    nextReport.ReporterId = valDict["ReporterId"].GetType() == typeof(DBNull) ? null : (int)valDict["ReporterId"];
                    nextReport.IsResolved = valDict["IsResolved"].GetType() == typeof(DBNull) ? null : (bool)valDict["IsResolved"];
                    output.Add(nextReport);
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
                    var valDict = MapToAttDict(varSqlVarMap, showcaseDict);
                    ShowcaseReport nextReport = new();
                    nextReport.Timestamp = valDict["Timestamp"].GetType() == typeof(DBNull) ? null : (DateTime)valDict["Timestamp"];
                    nextReport.Reason = valDict["Reason"].GetType() == typeof(DBNull) ? null : (string)valDict["Reason"];
                    nextReport.ShowcaseId = valDict["ShowcaseId"].GetType() == typeof(DBNull) ? null : (string)valDict["ShowcaseId"];
                    nextReport.ReporterId = valDict["ReporterId"].GetType() == typeof(DBNull) ? null : (int)valDict["ReporterId"];
                    nextReport.IsResolved = valDict["IsResolved"].GetType() == typeof(DBNull) ? null : (bool)valDict["IsResolved"];
                    output.Add(nextReport);
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
                foreach (var commentDict in getResult.Payload!)
                {
                    var valDict = MapToAttDict(varSqlVarMap, commentDict);
                    CommentReport nextReport = new();
                    nextReport.Timestamp = valDict["Timestamp"].GetType() == typeof(DBNull) ? null : (DateTime)valDict["Timestamp"];
                    nextReport.Reason = valDict["Reason"].GetType() == typeof(DBNull) ? null : (string)valDict["Reason"];
                    nextReport.CommentId = valDict["CommentId"].GetType() == typeof(DBNull) ? null : (long)valDict["CommentId"];
                    nextReport.ReporterId = valDict["ReporterId"].GetType() == typeof(DBNull) ? null : (int)valDict["ReporterId"];
                    nextReport.IsResolved = valDict["IsResolved"].GetType() == typeof(DBNull) ? null : (bool)valDict["IsResolved"];
                    output.Add(nextReport);
                }
                return Result<List<CommentReport>>.Success(output);
            }
            catch (Exception ex)
            {
                _logger.Warning(Category.BUSINESS, $"Error in getting showcases: {ex.Message}", "ShowcaseService");
                return new(Result.Failure("Error in getting showcases"));
            }
        }

        public async Task<Result<List<CommentReport>>> GetCommentReports(long commentId)
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
                foreach (var commentDict in getResult.Payload!)
                {
                    var valDict = MapToAttDict(varSqlVarMap, commentDict);
                    CommentReport nextReport = new();
                    nextReport.Timestamp = valDict["Timestamp"].GetType() == typeof(DBNull) ? null : (DateTime)valDict["Timestamp"];
                    nextReport.Reason = valDict["Reason"].GetType() == typeof(DBNull) ? null : (string)valDict["Reason"];
                    nextReport.CommentId = valDict["CommentId"].GetType() == typeof(DBNull) ? null : (long)valDict["CommentId"];
                    nextReport.ReporterId = valDict["ReporterId"].GetType() == typeof(DBNull) ? null : (int)valDict["ReporterId"];
                    nextReport.IsResolved = valDict["IsResolved"].GetType() == typeof(DBNull) ? null : (bool)valDict["IsResolved"];
                    output.Add(nextReport);
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

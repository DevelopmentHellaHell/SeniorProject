using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.ProjectShowcase.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.ProjectShowcase.Service.Implementations
{
    public class ProjectShowcaseService : IProjectShowcaseService
    {
        private readonly IProjectShowcaseDataAccess _projectShowcaseDataAccess;
        ProjectShowcaseService(IProjectShowcaseDataAccess projectShowcaseDataAccess) 
        { 
            _projectShowcaseDataAccess = projectShowcaseDataAccess;
        }

        public async Task<Result> AddComment(string showcaseId, string commentText)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> CreateShowcase(int listingId, string title, string description, List<HttpPostedFileBase> uploadedFiles)
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

        public async Task<Result> EditShowcase(int listingId, string title, string description, List<HttpPostedFileBase> uploadedFiles)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<Dictionary<string, object>>> GetCommentDetails(string showcaseId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<ShowcaseComment>>> GetComments(string showcaseId, int commentCount, int page)
        {
            var getResult = await _projectShowcaseDataAccess.GetComments(showcaseId, commentCount, page);
            List<ShowcaseComment> output = new();
            Dictionary<string,string> varSqlVarMap = new()
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

            return new()
            {
                IsSuccessful = true,
                Payload = output
            };
        }

        public async Task<Result<Dictionary<string, object>>> GetDetails(string showcaseId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<Showcase>> GetShowcase(string showcaseId)
        {
            var getResult = await _projectShowcaseDataAccess.GetShowcase(showcaseId).ConfigureAwait(false);
            if(!getResult.IsSuccessful)
            {
                return new()
                {
                    ErrorMessage = $"Error in getting Showcase from DAC: {getResult.ErrorMessage}",
                    IsSuccessful = false
                };
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

            return new()
            {
                IsSuccessful = true,
                Payload = output
            };
        }

        public async Task<Result<float>> LikeShowcase(string showcaseId)
        {
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            int accountId = int.Parse(claimsPrincipal?.FindFirstValue("sub")!);

            var userLikeResult = await _projectShowcaseDataAccess.RecordUserLike(accountId, showcaseId).ConfigureAwait(false);
            if (!userLikeResult.IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = $"Unable to like showcase: {userLikeResult.ErrorMessage}"
                };
            }

            var incrementLikeResult = await _projectShowcaseDataAccess.IncrementShowcaseLikes(showcaseId).ConfigureAwait(false);
            if (!incrementLikeResult.IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = $"Error in incrementing showcase likes: {incrementLikeResult.ErrorMessage}"
                };
            }

            return new()
            {
                IsSuccessful = true,
                Payload = incrementLikeResult.Payload!
            };
        }

        public async Task<Result> Publish(string showcaseId)
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
    }
}

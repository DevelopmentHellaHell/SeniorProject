using Microsoft.AspNetCore.Mvc;
using DevelopmentHell.Hubba.ProjectShowcase.Manager.Abstractions;
using Microsoft.AspNetCore.Authentication;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    public struct ShowcaseDTO
    {
        public int? ListingId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
    public struct CommentDTO
    {
        public string? CommentText { get; set; }
    }

    public struct ReportDTO
    {
        public string? ReasonText { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class ShowcasesController : Controller
    {
        private readonly IProjectShowcaseManager _projectShowcaseManager;
        private readonly ILoggerService _logger;
        ShowcasesController(IProjectShowcaseManager projectShowcaseManager, ILoggerService loggerService)
        {
            _projectShowcaseManager = projectShowcaseManager;
            _logger = loggerService;
        }

        private Func<object,IActionResult> GetFuncCode(int in_code)
        {
            switch (in_code)
            {
                case 202:
                    return Accepted;
                case 404:
                    return NotFound;
                case 400:
                    return BadRequest;
                case 200:
                    return Ok;
                //internal server error
                case 500:
                    return (object error) => StatusCode(500, error);
                default:
                    return NotFound;
            }
        }

        [HttpGet]
        [Route("view")]
        public async Task<IActionResult> GetShowcase([FromQuery(Name = "s")] string? showcaseId)
        {
            try
            {
                if (showcaseId == null)
                {
                    return BadRequest("Invalid request.");
                }
                var showcaseResult = await _projectShowcaseManager.GetShowcase(showcaseId);
                if (!showcaseResult.IsSuccessful)
                {
                    return GetFuncCode((int)showcaseResult.StatusCode!)(showcaseResult.ErrorMessage!);
                }
                return Ok(showcaseResult.Payload);

            } catch (Exception ex)
            {
                _logger.Warning(Models.Category.SERVER, $"Error in GetShowcase: {ex.Message}", "ShowcaseController");
                return StatusCode(500, "Unknown exception occured when attempting to complete your request");
            }
        }

        [HttpGet]
        [Route("comments")]
        public async Task<IActionResult> GetComments([FromQuery(Name = "s")] string? showcaseId, [FromQuery(Name = "c")] int? commentCount, [FromQuery(Name = "p")] int? page)
        {
            try
            {
                if (showcaseId == null)
                {
                    return BadRequest("Showcase Id missing from request");
                }

                var commentResult = await _projectShowcaseManager.GetComments(showcaseId, commentCount, page);
                if (!commentResult.IsSuccessful)
                {
                    return GetFuncCode((int)commentResult.StatusCode!)(commentResult.ErrorMessage!);
                }

                return Ok(commentResult.Payload);
            }
            catch (Exception ex)
            {
                _logger.Warning(Models.Category.SERVER, $"Error in GetComments: {ex.Message}", "ShowcaseController");
                return StatusCode(500, "Unknown exception occured when attempting to complete your request");
            }
        }

        [HttpPost]
        [Route("like")]
        public async Task<IActionResult> LikeShowcase([FromQuery(Name = "s")] string? showcaseId)
        {
            try
            {
                if (showcaseId == null)
                {
                    return BadRequest("Showcase Id missing from request");
                }

                var likeResult = await _projectShowcaseManager.LikeShowcase(showcaseId);
                if (!likeResult.IsSuccessful)
                {
                    if (likeResult.StatusCode != 500)
                    {
                        return GetFuncCode((int)likeResult.StatusCode!)(likeResult.ErrorMessage!);
                    }
                    likeResult = await _projectShowcaseManager.LikeShowcase(showcaseId);
                    if (!likeResult.IsSuccessful)
                    {
                        return GetFuncCode((int)likeResult.StatusCode!)(likeResult.ErrorMessage!);
                    }
                }

                return Ok(likeResult.Payload);
            }
            catch (Exception ex)
            {
                _logger.Warning(Models.Category.SERVER, $"Error in LikeShowcase: {ex.Message}", "ShowcaseController");
                return StatusCode(500, "Unknown exception occured when attempting to complete your request");
            }
        }
        [HttpPost]
        [Route("new")]
        public async Task<IActionResult> CreateShowcase(ShowcaseDTO uploadedShowcase)
        {
            try
            {
                if (uploadedShowcase.Title == null || uploadedShowcase.Title == "" ||
                    uploadedShowcase.Description == null || uploadedShowcase.Description == "" || 
                    uploadedShowcase.ListingId == null || 
                    uploadedShowcase.Files == null || uploadedShowcase.Files.Count == 0)
                {
                    return BadRequest("Showcase Id missing from request");
                }

                var createResult = await _projectShowcaseManager.CreateShowcase((int)uploadedShowcase.ListingId, uploadedShowcase.Title, uploadedShowcase.Description, uploadedShowcase.Files);
                if (!createResult.IsSuccessful)
                {
                    if (createResult.StatusCode != 500)
                    {
                        return GetFuncCode((int)createResult.StatusCode!)(createResult.ErrorMessage!);
                    }
                    createResult = await _projectShowcaseManager.CreateShowcase((int)uploadedShowcase.ListingId, uploadedShowcase.Title, uploadedShowcase.Description, uploadedShowcase.Files);
                    if (!createResult.IsSuccessful)
                    {
                        return GetFuncCode((int)createResult.StatusCode!)(createResult.ErrorMessage!);
                    }
                }

                return Ok(createResult.Payload);
            }
            catch (Exception ex)
            {
                _logger.Warning(Models.Category.SERVER, $"Error in CreateShowcase: {ex.Message}", "ShowcaseController");
                return StatusCode(500, "Unknown exception occured when attempting to complete your request");
            }
        }
        [HttpPost]
        [Route("edit")]
        public async Task<IActionResult> EditShowcase([FromQuery(Name = "s")] string? showcaseId, ShowcaseDTO editedShowcase)
        {
            try
            {
                if (showcaseId == null)
                {
                    return BadRequest("Showcase Id missing from request");
                }

                var editResult = await _projectShowcaseManager.EditShowcase(showcaseId, editedShowcase.ListingId, editedShowcase.Title, editedShowcase.Description, editedShowcase.Files);
                if (!editResult.IsSuccessful)
                {
                    if (editResult.StatusCode != 500)
                    {
                        return GetFuncCode((int)editResult.StatusCode!)(editResult.ErrorMessage!);
                    }
                    editResult = await _projectShowcaseManager.EditShowcase(showcaseId, editedShowcase.ListingId, editedShowcase.Title, editedShowcase.Description, editedShowcase.Files);
                    if (!editResult.IsSuccessful)
                    {
                        return GetFuncCode((int)editResult!.StatusCode!)(editResult.ErrorMessage!);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Warning(Models.Category.SERVER, $"Error in EditShowcase: {ex.Message}", "ShowcaseController");
                return StatusCode(500, "Unknown exception occured when attempting to complete your request");
            }
        }
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> DeleteShowcase([FromQuery(Name = "s")] string? showcaseId)
        {
            try
            {
                if (showcaseId == null)
                {
                    return BadRequest("Showcase Id missing from request");
                }

                var deleteResult = await _projectShowcaseManager.DeleteShowcase(showcaseId);
                if (!deleteResult.IsSuccessful)
                {
                    if (deleteResult.StatusCode != 500)
                    {
                        return GetFuncCode((int)deleteResult.StatusCode!)(deleteResult.ErrorMessage!);
                    }
                    deleteResult = await _projectShowcaseManager.DeleteShowcase(showcaseId);
                    if (!deleteResult.IsSuccessful)
                    {
                        return GetFuncCode((int)deleteResult!.StatusCode!)(deleteResult.ErrorMessage!);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Warning(Models.Category.SERVER, $"Error in DeleteShowcase: {ex.Message}", "ShowcaseController");
                return StatusCode(500, "Unknown exception occured when attempting to complete your request");
            }
        }
        [HttpPost]
        [Route("publish")]
        public async Task<IActionResult> PublishShowcase([FromQuery(Name = "s")] string? showcaseId, [FromQuery(Name = "l")] int? listingId)
        {
            try
            {
                if(showcaseId == null)
                {
                    return BadRequest("Showcase Id missing from request");
                }

                var publishResult = await _projectShowcaseManager.Publish(showcaseId, listingId);
                if (!publishResult.IsSuccessful)
                {
                    if (publishResult.StatusCode != 500)
                    {
                        return GetFuncCode((int)publishResult.StatusCode!)(publishResult.ErrorMessage!);
                    }
                    publishResult = await _projectShowcaseManager.Publish(showcaseId, listingId);
                    if (!publishResult.IsSuccessful)
                    {
                        return GetFuncCode((int)publishResult!.StatusCode!)(publishResult.ErrorMessage!);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Warning(Models.Category.SERVER, $"Error in PublishShowcase: {ex.Message}", "ShowcaseController");
                return StatusCode(500, "Unknown exception occured when attempting to complete your request");
            }
        }
        [HttpPost]
        [Route("unpublish")]
        public async Task<IActionResult> UnpublishShowcase([FromQuery(Name = "s")] string? showcaseId)
        {
            try
            {
                if (showcaseId == null)
                {
                    return BadRequest("Showcase Id missing from request");
                }

                var unpublishResult = await _projectShowcaseManager.Unpublish(showcaseId);
                if (!unpublishResult.IsSuccessful)
                {
                    if(unpublishResult.StatusCode != 500)
                    {
                        return GetFuncCode((int)unpublishResult.StatusCode!)(unpublishResult.ErrorMessage!);
                    }
                    unpublishResult = await _projectShowcaseManager.Unpublish(showcaseId);
                    if (!unpublishResult.IsSuccessful)
                    {
                        return GetFuncCode((int)unpublishResult!.StatusCode!)(unpublishResult.ErrorMessage!);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Warning(Models.Category.SERVER, $"Error in UnpublishShowcase: {ex.Message}", "ShowcaseController");
                return StatusCode(500, "Unknown exception occured when attempting to complete your request");
            }
        }
        [HttpPost]
        [Route("comments")]
        public async Task<IActionResult> AddComment([FromQuery(Name = "s")] string? showcaseId, CommentDTO comment)
        {
            try
            {
                if (showcaseId == null)
                {
                    return BadRequest("Showcase Id missing from request");
                }
                if (comment.CommentText == null)
                {
                    return BadRequest("Comment text missing from request");
                }

                var addResult = await _projectShowcaseManager.AddComment(showcaseId, comment.CommentText!).ConfigureAwait(false);
                if (!addResult.IsSuccessful)
                {
                    if(addResult.StatusCode != 500)
                    {
                        return GetFuncCode((int)addResult.StatusCode!)(addResult.ErrorMessage!);
                    }
                    addResult = await _projectShowcaseManager.AddComment(showcaseId,comment.CommentText!).ConfigureAwait(false);
                    if (!addResult.IsSuccessful)
                    {
                        return GetFuncCode((int)addResult!.StatusCode!)(addResult.ErrorMessage!);
                    }
                }

                return Ok();
            }
            catch(Exception ex)
            {
                _logger.Warning(Models.Category.SERVER, $"Error in AddComment: {ex.Message}", "ShowcaseController");
                return StatusCode(500, "Unknown exception occured when attempting to complete your request");
            }
        }
        [HttpPost]
        [Route("comments/edit")]
        public async Task<IActionResult> EditComment([FromQuery(Name = "cid")] int? commentId, CommentDTO comment)
        {
            try
            {
                if (commentId == null)
                {
                    return BadRequest("Comment Id missing from request");
                }
                if (comment.CommentText == null)
                {
                    return BadRequest("Comment text missing from request");
                }
                var editResult = await _projectShowcaseManager.EditComment((int)commentId!, comment.CommentText);
                if (!editResult.IsSuccessful)
                {
                    if(editResult.StatusCode != 500)
                    {
                        return GetFuncCode((int)editResult.StatusCode!)(editResult.ErrorMessage!);
                    }
                    editResult = await _projectShowcaseManager.EditComment((int)commentId!, comment.CommentText);
                    if (!editResult.IsSuccessful)
                    {
                        return GetFuncCode((int)editResult.StatusCode!)(editResult.ErrorMessage!);
                    }
                }

                return Ok();
            }
            catch(Exception ex)
            {
                _logger.Warning(Models.Category.SERVER, $"Error in EditComment: {ex.Message}", "ShowcaseController");
                return StatusCode(500, "Unknown exception occured when attempting to complete your request");
            }
        }
        [HttpPost]
        [Route("comments/delete")]
        public async Task<IActionResult> DeleteComment([FromQuery(Name = "cid")] int? commentId)
        {
            try
            {
                if (commentId == null)
                {
                    return BadRequest("Comment Id missing from request");
                }

                var deleteResult = await _projectShowcaseManager.DeleteComment((int)commentId!);
                if (!deleteResult.IsSuccessful)
                {
                    if(deleteResult.StatusCode != 500)
                    {
                        return GetFuncCode((int)deleteResult.StatusCode!)(deleteResult.ErrorMessage!);
                    }
                    deleteResult = await _projectShowcaseManager.DeleteComment((int)commentId!);
                    if (!deleteResult.IsSuccessful)
                    {
                        return GetFuncCode((int)deleteResult.StatusCode!)(deleteResult.ErrorMessage!);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Warning(Models.Category.SERVER, $"Error in DeleteComment: {ex.Message}", "ShowcaseController");
                return StatusCode(500, "Unknown exception occured when attempting to complete your request");
            }
        }
        [HttpPost]
        [Route("comments/rate")]
        public async Task<IActionResult> RateComment([FromQuery(Name = "cid")] int? commentId, [FromQuery(Name = "r")] bool? isUpvote)
        {
            try
            {
                if (commentId == null)
                {
                    return BadRequest("Comment Id missing from request");
                }
                if (isUpvote == null)
                {
                    return BadRequest("Rating type missing from request");
                }

                var rateResult = await _projectShowcaseManager.RateComment((int)commentId!, (bool)isUpvote!);
                if (!rateResult.IsSuccessful)
                {
                    if (rateResult.StatusCode != 500)
                    {
                        return GetFuncCode((int)rateResult.StatusCode!)(rateResult.ErrorMessage!);
                    }
                    rateResult = await _projectShowcaseManager.RateComment((int)commentId, (bool)isUpvote!);
                    if (!rateResult.IsSuccessful)
                    {
                        return GetFuncCode((int)rateResult.StatusCode!)(rateResult.ErrorMessage!);
                    }
                }

                return Ok(rateResult.Payload!);
            }
            catch (Exception ex)
            {
                _logger.Warning(Models.Category.SERVER, $"Error in RateComment: {ex.Message}", "ShowcaseController");
                return StatusCode(500, "Unknown exception occured when attempting to complete your request");
            }
        }
        [HttpPost]
        [Route("comments/report")]
        public async Task<IActionResult> ReportComment([FromQuery(Name = "cid")] int? commentId, ReportDTO reason)
        {
            try
            {
                if (commentId == null)
                {
                    return BadRequest("Comment Id missing from request");
                }
                if (reason.ReasonText == null ||  reason.ReasonText.Length == 0)
                {
                    return BadRequest("Report reason missing from request");
                }

                var reportResult = await _projectShowcaseManager.ReportComment((int)commentId, reason.ReasonText);
                if (!reportResult.IsSuccessful)
                {
                    if (reportResult.StatusCode != 500)
                    {
                        return GetFuncCode((int)reportResult.StatusCode!)(reportResult.ErrorMessage!);
                    }
                    reportResult = await _projectShowcaseManager.ReportComment((int)commentId, reason.ReasonText);
                    if (!reportResult.IsSuccessful)
                    {
                        return GetFuncCode((int)reportResult?.StatusCode!)(reportResult.ErrorMessage!);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Warning(Models.Category.SERVER, $"Error in ReportComment: {ex.Message}", "ShowcaseController");
                return StatusCode(500, "Unknown exception occured when attempting to complete your request");
            }
        }
        [HttpPost]
        [Route("report")]
        public async Task<IActionResult> ReportShowcase([FromQuery(Name = "s")] string? showcaseId, ReportDTO reason)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("unlink")]
        public async Task<IActionResult> UnlinkShowcase([FromQuery(Name = "s")] string? showcaseId)
        {
            throw new NotImplementedException();
        }

    }
}

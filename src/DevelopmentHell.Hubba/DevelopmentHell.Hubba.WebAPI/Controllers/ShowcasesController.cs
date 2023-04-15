using Microsoft.AspNetCore.Mvc;
using DevelopmentHell.Hubba.ProjectShowcase.Manager.Abstractions;
using Microsoft.AspNetCore.Authentication;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    public struct ShowcaseDTO
    {
        public int? ListingId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<IFormFile>? Files { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class ShowcasesController : Controller
    {
        private readonly IProjectShowcaseManager _projectShowcaseManager;
        ShowcasesController(IProjectShowcaseManager projectShowcaseManager)
        {
            _projectShowcaseManager = projectShowcaseManager;
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
                    var showcaseResultAgain = await _projectShowcaseManager.GetShowcase(showcaseId);
                    if (!showcaseResultAgain.IsSuccessful)
                    {
                        throw new IOException("Unable to Get showcase");
                    }
                }
                return Ok(showcaseResult.Payload);

            } catch (Exception ex)
            {
                return BadRequest("Invalid request.");
                
            }
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("comments")]
        public async Task<IActionResult> GetComments([FromQuery(Name = "s")] string? showcaseId, [FromQuery(Name = "c")] int? commentCount, [FromQuery(Name = "p")] int? page)
        {
            try
            {
                if (showcaseId == null)
                {
                    return BadRequest("Invalid request");
                }

                var commentResult = await _projectShowcaseManager.GetComments(showcaseId, commentCount, page);
                if (!commentResult.IsSuccessful)
                {
                    throw new IOException("Unable to get comments");
                }

                return Ok(commentResult.Payload);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid request.");
                //TODO: log failure
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
                    return BadRequest("Invalid request");
                }

                var likeResult = await _projectShowcaseManager.LikeShowcase(showcaseId);
                if (!likeResult.IsSuccessful)
                {
                    var likeResult2 = await _projectShowcaseManager.LikeShowcase(showcaseId);
                    if (!likeResult2.IsSuccessful)
                    {
                        throw new IOException("Unable to like Showcase");
                    }
                }

                return Ok(likeResult.Payload);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid request.");
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
                    return BadRequest("Invalid request");
                }

                var createResult = await _projectShowcaseManager.CreateShowcase((int)uploadedShowcase.ListingId, uploadedShowcase.Title, uploadedShowcase.Description, uploadedShowcase.Files);
                if (!createResult.IsSuccessful)
                {
                    var createResult2 = await _projectShowcaseManager.CreateShowcase((int)uploadedShowcase.ListingId, uploadedShowcase.Title, uploadedShowcase.Description, uploadedShowcase.Files);
                    if (!createResult2.IsSuccessful)
                    {
                        throw new IOException("Unable to Create Showcase");
                    }
                }

                return Ok(createResult.Payload);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid request");
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
                    return BadRequest("Invalid request");
                }

                var editResult = await _projectShowcaseManager.EditShowcase(showcaseId, editedShowcase.ListingId, editedShowcase.Title, editedShowcase.Description, editedShowcase.Files);
                if (!editResult.IsSuccessful)
                {
                    var editResult2 = await _projectShowcaseManager.EditShowcase(showcaseId, editedShowcase.ListingId, editedShowcase.Title, editedShowcase.Description, editedShowcase.Files);
                    if (!editResult2.IsSuccessful)
                    {
                        throw new IOException("Unable to Edit Shwocase");
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid request");
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
                    return BadRequest("Invalid request");
                }

                var deleteResult = await _projectShowcaseManager.DeleteShowcase(showcaseId);
                if (!deleteResult.IsSuccessful)
                {
                    var deleteResult2 = await _projectShowcaseManager.DeleteShowcase(showcaseId);
                    if (!deleteResult2.IsSuccessful)
                    {
                        throw new IOException("Unable to delete showcase");
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid request");
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
                    return BadRequest("Invalid request");
                }

                var publishResult = await _projectShowcaseManager.Publish(showcaseId, listingId);
                if (!publishResult.IsSuccessful)
                {
                    var publishResult2 = await _projectShowcaseManager.Publish(showcaseId, listingId);
                    if (!publishResult2.IsSuccessful)
                    {
                        throw new IOException("Unable to Publish showcase");
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid request");
            }
        }
        [HttpPost]
        [Route("unpublish")]
        public async Task<IActionResult> UnpublishShowcase([FromQuery(Name = "s")] string? showcaseId)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("comments")]
        public async Task<IActionResult> AddComment([FromQuery(Name = "s")] string? showcaseId, CommentDTO comment)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("comments/edit")]
        public async Task<IActionResult> EditComment([FromQuery(Name = "cid")] int? commentId, CommentDTO comment)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("comments/delete")]
        public async Task<IActionResult> DeleteComment([FromQuery(Name = "cid")] int? commentId)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("comments/rate")]
        public async Task<IActionResult> RateComment([FromQuery(Name = "cid")] int? commentId, [FromQuery(Name = "r")] bool? isUpvote)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("comments/report")]
        public async Task<IActionResult> ReportComment([FromQuery(Name = "cid")] int? commentId, ReportDTO reason)
        {
            throw new NotImplementedException();
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

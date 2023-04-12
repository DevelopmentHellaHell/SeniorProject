using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShowcasesController : Controller
    {
        [HttpGet]
        [Route("view")]
        public async Task<IActionResult> GetShowcase([FromQuery(Name = "s")] string? showcaseId)
        {
            throw new NotImplementedException();
        }
        [HttpGet]
        [Route("comments")]
        public async Task<IActionResult> GetComments([FromQuery(Name = "s")] string? showcaseId, [FromQuery(Name = "c")] int? commentCount, [FromQuery(Name = "p")] int? page)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("like")]
        public async Task<IActionResult> LikeShowcase([FromQuery(Name = "s")] string? showcaseId)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("new")]
        public async Task<IActionResult> CreateShowcase(ShowcaseDTO uploadedShowcase)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("edit")]
        public async Task<IActionResult> EditShowcase(ShowcaseDTO editedShowcase)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> DeleteShowcase([FromQuery(Name = "s")] string? showcaseId)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("publish")]
        public async Task<IActionResult> PublishShowcase([FromQuery(Name = "s")] string? showcaseId, [FromQuery(Name = "l")] int? listingId)
        {
            throw new NotImplementedException();
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

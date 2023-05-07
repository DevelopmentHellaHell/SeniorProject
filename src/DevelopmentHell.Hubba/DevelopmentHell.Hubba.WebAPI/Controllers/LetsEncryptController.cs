using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    [ApiController]
    [Route(".well-known")]
    public class LetsEncryptController : ControllerBase
    {
        [HttpGet]
        [Route("acme-challenge/{filename}")]
        public IActionResult GetChallenge(string filename)
        {
            Console.WriteLine("endpoint hit");
            var filePath = $"./.well-known/acme-challenge/{filename}";
            if (System.IO.File.Exists(filePath))
            {
                Console.WriteLine("found");
                var fileContent = System.IO.File.ReadAllText(filePath);
                return new ContentResult
                {
                    ContentType = "text/plain",
                    StatusCode = 200,
                    Content = fileContent
                };
            }
            else
            {
                Console.WriteLine("not found");
                return NotFound();
            }
        }
    }
}

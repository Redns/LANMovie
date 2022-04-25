using Microsoft.AspNetCore.Mvc;

namespace LANMovie.Controllers
{
    [Route("api/video")]
    [ApiController]
    public class VideoController : Controller
    {
        [HttpGet("{videoId}")]
        public IActionResult Get(string videoId)
        {
            return PhysicalFile($"{Directory.GetCurrentDirectory()}/Data/Videos/{videoId}", "video/mp4");
        }
    }
}

using LANMovie.Common;
using LANMovie.Data.Access;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LANMovie.Controllers
{
    [Route("api/image")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        public const string MovieRootPath = "Data/Videos/Movies";
        public const string TeleplayRootPath = "Data/Videos/Teleplays";
        public const string ShortVideoRootPath = "Data/Videos/ShortVideos";

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetMovieCover(string movieId)
        {
            using(var context = new OurDbContext())
            {
                var sqlMovieData = new SqlMovieData(context);

                var movie = await sqlMovieData.GetAsync(movieId);
                var movieCover = $"{MovieRootPath}/{movie?.Id}/{movie?.Cover}";

                if ((movie != null) && System.IO.File.Exists(movieCover))
                {
                    return File(System.IO.File.ReadAllBytes(movieCover), $"image/{FileHelper.GetExtension(movieCover)}");
                }
                else
                {
                    return File(System.IO.File.ReadAllBytes($"Data/Videos/imageNotFound.png"), "image/png");
                }
            }
        }
    }
}

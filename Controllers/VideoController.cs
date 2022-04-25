using LANMovie.Common;
using LANMovie.Data.Access;
using LANMovie.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LANMovie.Controllers
{
    [Route("api/video")]
    [ApiController]
    public class VideoController : Controller
    {
        /// <summary>
        /// 获取电影视频
        /// </summary>
        /// <param name="movieId">电影ID</param>
        /// <returns></returns>
        [HttpGet("{movieId}")]
        public async Task<IActionResult> GetMovie(string movieId)
        {
            using (var context = new OurDbContext())
            {
                var sqlMovieData = new SqlMovieData(context);
                var movie = await sqlMovieData.GetAsync(movieId);
                if((movie != null) && (!string.IsNullOrEmpty(movie.VideoPath)))
                {
                    movie.RequestCount++;
                    await sqlMovieData.UpdateAsync(movie);

                    return PhysicalFile($"{Directory.GetCurrentDirectory()}/Data/Videos/{movie.Id}/{movie.VideoPath}", $"video/{FileHelper.GetExtension(movie.VideoPath)}");
                }
                return PhysicalFile($"{Directory.GetCurrentDirectory()}/Data/Videos/videoNotFound.mp4", "video/mp4");
            }
        }


        /// <summary>
        /// 获取电影信息
        /// </summary>
        /// <param name="movieId">电影ID</param>
        /// <returns></returns>
        [HttpGet("{movieId}/i")]
        public async Task<ApiResult<MovieEntity?>> GetMovieInfo(string movieId)
        {
            using(var context = new OurDbContext())
            {
                var movie = await new SqlMovieData(context).GetAsync(movieId);
                if(movie != null)
                {
                    return new ApiResult<MovieEntity?>(ApiResultCode.Success, $"Get {movie.Name}'s information success", movie);
                }
                else
                {
                    return new ApiResult<MovieEntity?>(ApiResultCode.ResourceNotFound, "Video not exist", null);
                }
            }
        }


        /// <summary>
        /// 删除电影
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [HttpDelete("{movieId}")]
        public async Task<ApiResult<MovieEntity?>> RemoveMovie(string movieId)
        {
            using(var context = new OurDbContext())
            {
                var sqlMovieData = new SqlMovieData(context);
                var movie = await sqlMovieData.GetAsync(movieId);
                if(movie != null)
                {
                    if (sqlMovieData.Remove(movie))
                    {
                        // 先删除数据库数据, 再删除本地文件
                        // 避免删除数据库数据失败后, 无法找回原视频的Bug
                        if (Directory.Exists($"Data/Videos/Movies/{movie.Id}"))
                        {
                            Directory.Delete($"Data/Videos/Movies/{movie.Id}");
                        }
                        return new ApiResult<MovieEntity?>(ApiResultCode.Success, $"Remove {movie.Name} success", movie);
                    }
                    else
                    {
                        return new ApiResult<MovieEntity?>(ApiResultCode.ServerError, $"Remove {movie.Name} failed", movie);
                    }
                }
                return new ApiResult<MovieEntity?>(ApiResultCode.ResourceNotFound, "Video not exist", null);
            }
        }


        /// <summary>
        /// 获取电视剧视频
        /// </summary>
        /// <param name="teleId">电视剧ID</param>
        /// <param name="telePage">电视剧集数</param>
        /// <returns></returns>
        [HttpGet("{teleId}/{telePage}")]
        public IActionResult GetTeleplay(string teleId, string telePage)
        {
            return PhysicalFile($"{Directory.GetCurrentDirectory()}/Data/Videos/{teleId}/{telePage}", "video/mp4");
        }


        /// <summary>
        /// 获取短视频
        /// </summary>
        /// <param name="shortVideoId"></param>
        /// <returns></returns>
        [HttpGet("{shortVideoId}/sv")]
        public IActionResult GetShortVideo(string shortVideoId)
        {
            return PhysicalFile($"{Directory.GetCurrentDirectory()}/Data/Videos/{shortVideoId}", "video/mp4");
        }
    }
}

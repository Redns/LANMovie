using LANMovie.Common;
using LANMovie.Data.Access;
using LANMovie.Data.Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LANMovie.Controllers
{
    [Route("api/video")]
    [ApiController]
    public class VideoController : Controller
    {
        public const int HttpRangeMaxSize = 1024 * 1024;


        /// <summary>
        /// 获取电影视频
        /// </summary>
        /// <param name="movieId">电影ID</param>
        /// <returns></returns>
        [HttpGet("movie/{movieId}")]
        public async Task GetMovie(string movieId)
        {
            FileStream videoReadStream;
            HttpResponse resp = HttpContext.Response;
            
            // 检索数据库中的电影
            using (var context = new OurDbContext())
            {
                var sqlMovieData = new SqlMovieData(context);
                var movie = sqlMovieData.Get(movieId);
                if ((movie != null) && (!string.IsNullOrEmpty(movie.VideoPath)))
                {
                    videoReadStream = System.IO.File.OpenRead($"Data/Videos/Movies/{movie.Id}/{movie.VideoPath}");
                }
                else
                {
                    videoReadStream = System.IO.File.OpenRead($"Data/Videos/videoNotFound.mp4");
                }

                // 根据Range返回视频片段
                long rangeStart, rangeEnd, rangeSize;
                var range = Request.Headers.Range.ToString().Trim().ToLower();
                if (range.StartsWith("bytes=") && range.Contains('-'))
                {
                    var ranges = range[6..].Split('-');

                    // 计算Range范围
                    rangeStart = int.Parse(ranges[0]);
                    if (!string.IsNullOrEmpty(ranges[1]))
                    {
                        rangeEnd = int.Parse(ranges[1]);
                    }
                    else
                    {
                        rangeEnd = videoReadStream.Length - 1;
                    }

                    // 判断Range尺寸是否超出限制
                    rangeSize = rangeEnd - rangeStart + 1;
                    if (rangeSize > HttpRangeMaxSize)
                    {
                        rangeEnd = rangeStart + HttpRangeMaxSize - 1;
                        rangeSize = HttpRangeMaxSize;
                    }

                    // 修改Header中的Range
                    if (!resp.HasStarted)
                    {
                        resp.Headers.Add("Content-Range", $"bytes {rangeStart}-{rangeEnd}/{videoReadStream.Length}");
                        resp.Headers.Add("Content-Length", rangeSize.ToString());
                        resp.Headers.ContentType = "video/mp4";

                        // 若返回整个文件则响应200, 否则响应206
                        if ((rangeStart == 0) && (rangeEnd + 1) >= videoReadStream.Length)
                        {
                            resp.StatusCode = 200;
                        }
                        else
                        {
                            resp.StatusCode = 206;
                        }
                    }

                    // 读取数据流至数组
                    byte[] videoBuffer = new byte[HttpRangeMaxSize];
                    videoReadStream.Seek(rangeStart, SeekOrigin.Begin);
                    int readLen = videoReadStream.Read(videoBuffer, 0, videoBuffer.Length);

                    // 写入响应
                    var syncIOFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
                    if (syncIOFeature != null)
                    {
                        syncIOFeature.AllowSynchronousIO = true;
                    }
                    await resp.Body.WriteAsync(videoBuffer.AsMemory(0, readLen));
                    await videoReadStream.FlushAsync();
                    await videoReadStream.DisposeAsync();
                }
            }
        }


        /// <summary>
        /// 下载电影
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [HttpGet("movie/{movieId}/d")]
        public IActionResult DownloadMovie(string movieId)
        {
            using (var context = new OurDbContext())
            {
                var sqlMovieData = new SqlMovieData(context);
                var movie = sqlMovieData.Get(movieId);
                if ((movie != null) && (!string.IsNullOrEmpty(movie.VideoPath)))
                {
                    return new PhysicalFileResult($"{Directory.GetCurrentDirectory()}/Data/Videos/Movies/{movie.Id}/{movie.VideoPath}", $"video/{FileHelper.GetExtension(movie.VideoPath)}");
                }
                else
                {
                    return new PhysicalFileResult($"{Directory.GetCurrentDirectory()}/Data/Videos/videoNotFound.mp4", $"video/mp4");
                }
            }
        }


        /// <summary>
        /// 获取电影信息
        /// </summary>
        /// <param name="movieId">电影ID</param>
        /// <returns></returns>
        [HttpGet("movie/{movieId}/i")]
        public async Task<ApiResult<MovieEntity?>> GetMovieInfo(string movieId)
        {
            using(var context = new OurDbContext())
            {
                var movie = await new SqlMovieData(context).GetAsync(movieId);
                if(movie != null)
                {
                    return new ApiResult<MovieEntity?>(ApiResultCode.Success, $"Get {movie.Name}'s information success", movie);
                }
                return new ApiResult<MovieEntity?>(ApiResultCode.ResourceNotFound, "Video not exist", null);
            }
        }


        /// <summary>
        /// 删除电影
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [HttpDelete("movie/{movieId}")]
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
                            Directory.Delete($"Data/Videos/Movies/{movie.Id}", true);
                        }
                        return new ApiResult<MovieEntity?>(ApiResultCode.Success, $"Remove {movie.Name} success", movie);
                    }
                    return new ApiResult<MovieEntity?>(ApiResultCode.ServerError, $"Remove {movie.Name} failed", movie);
                }
                return new ApiResult<MovieEntity?>(ApiResultCode.ResourceNotFound, "Video not exist", null);
            }
        }


        /// <summary>
        /// 修改电影信息
        /// </summary>
        /// <param name="movieId">待修改电影的ID</param>
        /// <returns></returns>
        [HttpPost("movie/{movieId}/i")]
        public async Task<ApiResult<MovieEntity?>> UpdateMovieInfo(string movieId, [FromBody]MovieEntity newMovie)
        {
            using(var context = new OurDbContext())
            {
                var sqlMovieData = new SqlMovieData(context);
                var oldMovie = await sqlMovieData.GetAsync(movieId);
                if(oldMovie != null)
                {
                    oldMovie.Name = newMovie.Name;
                    oldMovie.Description = newMovie.Description;
                    oldMovie.PublishTime = newMovie.PublishTime;
                    oldMovie.Director = newMovie.Director;
                    oldMovie.Category = newMovie.Category;
                    oldMovie.Area = newMovie.Area;

                    if(await sqlMovieData.UpdateAsync(oldMovie))
                    {
                        return new ApiResult<MovieEntity?>(ApiResultCode.Success, "Update movie's information success", oldMovie);
                    }
                    return new ApiResult<MovieEntity?>(ApiResultCode.ServerError, "Update movie's information failed", null);
                }
                return new ApiResult<MovieEntity?>(ApiResultCode.ResourceNotFound, "Movie not exist", null);
            }
        }


        /// <summary>
        /// 获取电视剧视频
        /// </summary>
        /// <param name="teleId">电视剧ID</param>
        /// <param name="telePage">电视剧集数</param>
        /// <returns></returns>
        [HttpGet("tele/{teleId}/{telePage}")]
        public IActionResult GetTeleplay(string teleId, string telePage)
        {
            return PhysicalFile($"{Directory.GetCurrentDirectory()}/Data/Videos/{teleId}/{telePage}", "video/mp4");
        }


        /// <summary>
        /// 获取短视频
        /// </summary>
        /// <param name="shortVideoId"></param>
        /// <returns></returns>
        [HttpGet("sv/{shortVideoId}")]
        public IActionResult GetShortVideo(string shortVideoId)
        {
            return PhysicalFile($"{Directory.GetCurrentDirectory()}/Data/Videos/{shortVideoId}", "video/mp4");
        }
    }
}

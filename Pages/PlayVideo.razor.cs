using LANMovie.Common;
using LANMovie.Data.Access;
using LANMovie.Data.Entities;
using Microsoft.JSInterop;

namespace LANMovie.Pages
{
    partial class PlayVideo
    {
        MovieEntity? movie;
        bool visiable_movieEditModal = false;

        TeleplayEntity? teleplay;                           
        ShortVideoEntity? shortvideo;                       
        VideoCategory videoCategory = VideoCategory.None;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (!string.IsNullOrEmpty(VideoId))
            {
                // 分割页面URL
                // 电影：movie/{movieId}
                // 电视剧：teleplay/{teleId}/{telePage}
                // 短视频：shortvideo/{svId}
                var videoUrls = VideoId.Split("/"); 

                videoCategory = VideoEntity.ParseVideoCategory(videoUrls[0]);
                if(videoCategory == VideoCategory.Movie)
                {
                    using (var context = new OurDbContext())
                    {
                        var sqlMovieData = new SqlMovieData(context);
                        movie = await sqlMovieData.GetAsync(videoUrls[1]);
                    }

                    if(movie != null)
                    {
                        string host = NavigationManager.Uri;
                        string QRCodeImage = $"Data/Videos/Movies/{movie.Id}/QRCode.png";
                        if (!host.Contains("localhost") && !host.Contains("127.0.0.1"))
                        {
                            if (File.Exists(QRCodeImage) && (QRCoderHelper.QRDecoder(QRCodeImage) != host))
                            {
                                File.Delete(QRCodeImage);
                                QRCoderHelper.QRCoder(host, QRCodeImage, "", 5);
                            }
                            else
                            {
                                QRCoderHelper.QRCoder(host, QRCodeImage, "", 5);
                            }
                        }
                        else
                        {
                            if (!File.Exists(QRCodeImage))
                            {
                                QRCoderHelper.QRCoder("当使用 localhost/127.0.0.1 访问时，系统不会生成连接二维码!", QRCodeImage, "", 5);
                            }
                        }
                    }
                }
                else if(videoCategory == VideoCategory.Teleplay)
                {

                }
                else if(videoCategory == VideoCategory.ShortVideo)
                {

                }
            }
        }


        /// <summary>
        /// 保持电影修改
        /// </summary>
        /// <returns></returns>
        async Task SaveMovieEdit()
        {
            if(movie != null)
            {
                using (var context = new OurDbContext())
                {
                    var sqlMovieData = new SqlMovieData(context);
                    if (await sqlMovieData.UpdateAsync(movie))
                    {
                        _ = _message.Success("修改成功 !", 1.5);
                    }
                    else
                    {
                        _ = _message.Error("修改失败 !", 1.5);
                    }
                }
            }
            visiable_movieEditModal = false;
        }


        /// <summary>
        /// 下载电影
        /// </summary>
        async Task DownloadMovie()
        {
            string movieDownloadName = $"{movie?.Name} {movie?.PublishTime}.{FileHelper.GetExtension(movie?.VideoPath ?? "png")}";
            string movieDownloadUrl = $"{NavigationManager.BaseUri}api/video/movie/{movie?.Id}/d"; 
            await JS.InvokeVoidAsync("downloadFileFromStream", movieDownloadName, movieDownloadUrl);
        }


        /// <summary>
        /// 删除电影
        /// </summary>
        /// <param name="movieId">电影ID</param>
        /// <returns></returns>
        async Task RemoveMovie()
        {
            if(movie != null)
            {
                using (var context = new OurDbContext())
                {
                    var sqlMovieData = new SqlMovieData(context);
                    if (await sqlMovieData.RemoveAsync(movie))
                    {
                        if (Directory.Exists($"Data/Videos/Movies/{movie.Id}"))
                        {
                            Directory.Delete($"Data/Videos/Movies/{movie.Id}", true);
                        }
                        NavigationManager.NavigateTo("Movies");
                    }
                    else
                    {
                        await _message.Error($"{movie.Name} {movie.PublishTime} 删除失败!", 2);
                    }
                }
            }
        }
    }
}

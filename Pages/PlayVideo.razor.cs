using LANMovie.Common;
using LANMovie.Data.Access;
using LANMovie.Data.Entities;

namespace LANMovie.Pages
{
    partial class PlayVideo
    {
        static MovieEntity? movie;                                 
        static TeleplayEntity? teleplay;                           
        static ShortVideoEntity? shortvideo;                       
        VideoCategory videoCategory = VideoCategory.None;

        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(VideoId))
            {
                // 分割页面URL
                // 电影：movie/{movieId}
                // 电视剧：teleplay/{teleId}/{telePage}
                // 短视频：shortvideo/{svId}
                var videoUrls = VideoId.Split("/"); 

                videoCategory = CheckVideoCategory(videoUrls[0]);
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
            await base.OnInitializedAsync();
        }


        VideoCategory CheckVideoCategory(string videoCategory)
        {
            var temp = videoCategory.ToLower();
            if(temp == "movie") { return VideoCategory.Movie; }
            else if(temp == "teleplay") { return VideoCategory.Teleplay; }
            else if(temp == "shortvideo") { return VideoCategory.ShortVideo; }
            else { return VideoCategory.None; }
        }
    }
}

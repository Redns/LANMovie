using AntDesign;
using LANMovie.Common;
using LANMovie.Data.Access;
using LANMovie.Data.Entities;
using Microsoft.AspNetCore.Components.Forms;
using System;

namespace LANMovie.Pages
{
    partial class Index
    {
        bool emptyShown = true;

        int uploadStepCurrent = 0;
        VideoCategory uploadVideoCategory;
        
        MovieEntity movie = new();
        DateTime publishDate = DateTime.Now;

        bool progress_showInfo;
        int progress_stroke_width;
        int _ProgressPercent;                                  
        int ProgressPercent
        {
            get { return _ProgressPercent; }
            set
            {
                if (value == 0)
                {
                    progress_stroke_width = 0;
                    progress_showInfo = false;
                }
                else
                {
                    progress_stroke_width = 10;
                    progress_showInfo = true;
                }
                _ProgressPercent = value;
            }
        }

        IBrowserFile? uploadMovie;
        IBrowserFile? uploadMovieCover;

        readonly MovieConfig movieConfig = GlobalValues.AppConfig?.Data.Video.Movie ?? new MovieConfig(10*1024*1024, 1*1024*1024*1024);


        /// <summary>
        /// 上传电影
        /// </summary>
        /// <returns></returns>
        async Task UploadMovie()
        {
            // 创建上传文件夹
            var uploadDir = FileHelper.GenerateUnitDirName("Data/Videos/Movies", 8, true);
            var uploadMovieExt = FileHelper.GetExtension(uploadMovie?.Name ?? "");
            var uploadMovieCoverExt = FileHelper.GetExtension(uploadMovieCover?.Name ?? "");

            // 上传封面(路径为 Data/Videos/Movies/{uploadDir}/{cover.xxx})
            using (var movieCoverWriter = new FileStream($"Data/Videos/Movies/{uploadDir}/cover.{uploadMovieCoverExt}", FileMode.Create))
            {
                using (var movieCoverReader = uploadMovieCover?.OpenReadStream(movieConfig.CoverMaxSize))
                {
                    if(movieCoverReader != null)
                    {
                        await movieCoverReader.CopyToAsync(movieCoverWriter);
                    }
                }
                await movieCoverWriter.FlushAsync();
            }

            // 上传视频(路径为 Data/Videos/Movies/{uploadDir}/{src.xxx})
            using (var movieWriter = new FileStream($"Data/Videos/Movies/{uploadDir}/src.{uploadMovieExt}", FileMode.Create))
            {
                using(var movieReader = uploadMovie?.OpenReadStream(movieConfig.MaxSize))
                {
                    if(movieReader != null)
                    {
                        int readLen;
                        long readLenTotal = 0;
                        long lenTotal = movieReader.Length;
                        byte[] buffer = new byte[1024 * 1024];

                        while ((readLen = await movieReader.ReadAsync(buffer)) > 0)
                        {
                            // 写入文件
                            readLenTotal += readLen;
                            await movieWriter.WriteAsync(buffer.AsMemory(0, readLen));

                            // 更新进度条
                            ProgressPercent = (int)(readLenTotal * 100.0 / lenTotal);
                            await InvokeAsync(() => StateHasChanged());
                        }
                    }
                }
                await movieWriter.FlushAsync();
            }

            // 添加至数据库
            using(var context = new OurDbContext())
            {
                var sqlMovieData = new SqlMovieData(context);

                movie.Id = uploadDir;
                movie.VideoPath = $"src.{uploadMovieExt}";
                movie.UploadTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                movie.PublishTime = publishDate.Year.ToString();
                movie.Cover = $"cover.{uploadMovieCoverExt}";
                movie.Size = FileHelper.RebuildFileSize(uploadMovie?.Size ?? 0);
                await sqlMovieData.AddAsync(movie);
            }

            uploadStepCurrent++;
        }
    }

    enum VideoCategory
    {
        Movie = 0,
        Teleplay,
        ShortVideo,
        None
    }
}

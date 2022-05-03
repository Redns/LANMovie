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
                InvokeAsync(() => StateHasChanged());
            }
        }

        IBrowserFile? uploadMovie;
        IBrowserFile? uploadMovieCover;

        readonly MovieConfig movieConfig = GlobalValues.AppConfig?.Data.Video.Movie ?? new MovieConfig(10*1024*1024, 1*1024*1024*1024);
        readonly string movieRoot = "Data/Videos/Movies";

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            string host = NavigationManager.BaseUri;
            if (!host.Contains("localhost") && !host.Contains("127.0.0.1"))
            {
                if (File.Exists("wwwroot/connect.png") && (QRCoderHelper.QRDecoder("wwwroot/connect.png") != host))
                {
                    File.Delete("wwwroot/connect.png");
                    QRCoderHelper.QRCoder(host, "wwwroot/connect.png", "", 5);
                }
                else
                {
                    QRCoderHelper.QRCoder(host, "wwwroot/connect.png", "", 5);
                }
            }
            else
            {
                if (!File.Exists("wwwroot/connect.png"))
                {
                    QRCoderHelper.QRCoder("当使用 localhost/127.0.0.1 访问时，系统不会生成连接二维码!", "wwwroot/connect.png", "", 5);
                }
            }
        }


        /// <summary>
        /// 上传电影
        /// </summary>
        /// <returns></returns>
        async Task UploadMovie()
        {
            // 创建上传文件夹
            var uploadDir = FileHelper.GenerateUnitDirName(movieRoot, 8, true);
            var uploadMovieExt = FileHelper.GetExtension(uploadMovie?.Name ?? "");
            var uploadMovieCoverExt = FileHelper.GetExtension(uploadMovieCover?.Name ?? "");

            if (string.IsNullOrEmpty(movie.Name))
            {
                _ = _message.Error("电影名称不能为空 !", 1.5);
                uploadStepCurrent = 0;
                return;
            }

            try
            {
                // 上传封面(Data/Videos/Movies/{uploadDir}/{cover.xxx})
                using (var movieCoverReader = uploadMovieCover?.OpenReadStream(movieConfig.CoverMaxSize))
                {
                    if (movieCoverReader != null)
                    {
                        using (var movieCoverWriter = new FileStream($"Data/Videos/Movies/{uploadDir}/cover_temp.{uploadMovieCoverExt}", FileMode.Create))
                        {
                            await movieCoverReader.CopyToAsync(movieCoverWriter);
                            await movieCoverWriter.FlushAsync();
                        }

                        // 裁剪图片尺寸至 1920*1080，之后添加水印
                        using (var resizeCoverImage = ImageHelper.ImageCut($"Data/Videos/Movies/{uploadDir}/cover_temp.{uploadMovieCoverExt}"))
                        {
                            if (resizeCoverImage != null)
                            {
                                ImageHelper.ImageWaterMark(resizeCoverImage, $"{movie.Name} {movie.PublishTime}", leftEdge: 50, bottomEdge: 140, fontSize: 80)
                                           .WriteToFile($"Data/Videos/Movies/{uploadDir}/cover.{uploadMovieCoverExt}");
                            }
                        }

                        File.Delete($"Data/Videos/Movies/{uploadDir}/cover_temp.{uploadMovieCoverExt}");
                    }
                    else
                    {
                        if (Directory.Exists($"{movieRoot}/{movie.Id}"))
                        {
                            Directory.Delete($"{movieRoot}/{movie.Id}", true);
                        }

                        _ = _message.Error($"请选择封面 !", 1.5);
                        return;
                    }
                }

                // 上传视频(Data/Videos/Movies/{uploadDir}/{src.xxx})
                using (var movieReader = uploadMovie?.OpenReadStream(movieConfig.MaxSize))
                {
                    if (movieReader != null)
                    {
                        using (var movieWriter = new FileStream($"Data/Videos/Movies/{uploadDir}/src.{uploadMovieExt}", FileMode.Create))
                        {
                            int readLen;
                            long readLenTotal = 0;
                            long lenTotal = movieReader.Length;
                            byte[] buffer = new byte[1024 * 1024];

                            while ((readLen = await movieReader.ReadAsync(buffer)) > 0)
                            {
                                readLenTotal += readLen;
                                await movieWriter.WriteAsync(buffer.AsMemory(0, readLen));

                                ProgressPercent = (int)(readLenTotal * 100.0 / lenTotal);
                            }
                            await movieWriter.FlushAsync();
                        }
                    }
                    else
                    {
                        if (Directory.Exists($"{movieRoot}/{movie.Id}"))
                        {
                            Directory.Delete($"{movieRoot}/{movie.Id}", true);
                        }

                        _ = _message.Error($"请选择要上传的视频 !", 1.5);
                        return;
                    }
                }

                // 添加至数据库
                using (var context = new OurDbContext())
                {
                    var sqlMovieData = new SqlMovieData(context);

                    movie.Id = uploadDir;
                    movie.VideoPath = $"src.{uploadMovieExt}";
                    movie.UploadTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    movie.Cover = $"cover.{uploadMovieCoverExt}";
                    movie.Size = FileHelper.RebuildFileSize(uploadMovie?.Size ?? 0);
                    await sqlMovieData.AddAsync(movie);
                }
                uploadStepCurrent++;
            }
            catch (Exception ex)
            {
                var exceptType = ex.GetType();
                if (exceptType == typeof(IOException))
                {
                    if (!File.Exists($"{movieRoot}/{movie.Id}/cover.{uploadMovieCoverExt}"))
                    {
                        _ = _message.Error($"封面大小超过{FileHelper.RebuildFileSize(movieConfig.CoverMaxSize)} !", 1.5);
                    }
                    else if (!File.Exists($"{movieRoot}/{movie.Id}/src.{uploadMovieExt}"))
                    {
                        _ = _message.Error($"视频大小超过{FileHelper.RebuildFileSize(movieConfig.MaxSize)} !", 1.5);
                    }
                    else
                    {
                        _ = _message.Error($"上传失败, {ex.Message} !", 1.5);
                    }
                }
                else if (exceptType == typeof(NetVips.VipsException))
                {
                    _ = _message.Error($"封面处理上传失败, {ex.Message} !", 1.5);
                }
                else
                {
                    _ = _message.Error($"上传失败, {ex.Message} !", 1.5);
                }

                if (Directory.Exists($"{movieRoot}/{movie.Id}"))
                {
                    Directory.Delete($"{movieRoot}/{movie.Id}", true);
                }
            }
        }
    }
}

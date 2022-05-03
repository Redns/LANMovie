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
            movie.Id = FileHelper.GenerateUnitDirName(movieRoot, 8, true);

            string uploadMovieName = $"src.{FileHelper.GetExtension(uploadMovie?.Name ?? "")}";
            string uploadCoverName = $"cover.{FileHelper.GetExtension(uploadMovieCover?.Name ?? "")}";
            string uploadMoviePath = $"{movieRoot}/{movie.Id}/{uploadMovieName}";
            string uploadCoverPath = $"{movieRoot}/{movie.Id}/{uploadCoverName}";
            string uploadCoverTempPath = $"{movieRoot}/{movie.Id}/cover_temp.{FileHelper.GetExtension(uploadMovieCover?.Name ?? "")}";

            try
            {
                if (string.IsNullOrEmpty(movie.Name))
                {
                    uploadStepCurrent = 0;
                    throw new ArgumentNullException("电影名称不能为空");
                }

                using (var movieCoverReader = uploadMovieCover?.OpenReadStream(movieConfig.CoverMaxSize))
                {
                    if(movieCoverReader != null)
                    {
                        using (var movieCoverWriter = new FileStream(uploadCoverTempPath, FileMode.Create))
                        {
                            await movieCoverReader.CopyToAsync(movieCoverWriter);
                            await movieCoverWriter.FlushAsync();
                        }

                        // 裁剪图片尺寸至 1920*1080，之后添加水印
                        using (var resizeCoverImage = ImageHelper.ImageCut(uploadCoverTempPath))
                        {
                            if (resizeCoverImage != null)
                            {
                                ImageHelper.ImageWaterMark(resizeCoverImage, $"{movie.Name} {movie.PublishTime}", leftEdge: 50, bottomEdge: 140, fontSize: 80)
                                           .WriteToFile(uploadCoverPath);
                            }
                        }

                        File.Delete(uploadCoverTempPath);
                    }
                    else
                    {
                        throw new ArgumentNullException("请选择封面");
                    }
                }

                // 分段上传视频
                using(var movieReader = uploadMovie?.OpenReadStream(movieConfig.MaxSize))
                {
                    if(movieReader != null)
                    {
                        using (var movieWriter = new FileStream(uploadMoviePath, FileMode.Create))
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
                        throw new ArgumentNullException("请选择视频文件");
                    }
                }

                // 添加至数据库
                using (var context = new OurDbContext())
                {
                    var sqlMovieData = new SqlMovieData(context);

                    movie.VideoPath = uploadMovieName;
                    movie.UploadTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    movie.Cover = uploadCoverName;
                    movie.Size = FileHelper.RebuildFileSize(uploadMovie?.Size ?? 0);

                    _ = sqlMovieData.AddAsync(movie);
                }

                uploadStepCurrent = 2;
            }
            catch(Exception ex)
            {
                var exceptType = ex.GetType();
                var errorMessage = $"上传失败, 具体原因请查看日志文件 !";

                if (exceptType == typeof(IOException))
                {
                    if (!string.IsNullOrEmpty(uploadCoverPath) && !File.Exists(uploadCoverPath))
                    {
                        errorMessage = $"封面大小超过{FileHelper.RebuildFileSize(movieConfig.CoverMaxSize)} !";
                    }
                    else if (!string.IsNullOrEmpty(uploadMoviePath) && !File.Exists(uploadMoviePath))
                    {
                        errorMessage = $"视频大小超过{FileHelper.RebuildFileSize(movieConfig.MaxSize)} !";
                    }
                }
                else if(exceptType == typeof(NetVips.VipsException))
                {
                    errorMessage = $"封面图片处理失败, {ex.Message} !";
                }
                else if(exceptType == typeof(ArgumentNullException))
                {
                    errorMessage = $"上传失败, {(ex as ArgumentNullException)?.ParamName} !";
                }

                _ = _message.Error(errorMessage, 1.5);

                if (!string.IsNullOrEmpty(movie.Id) && Directory.Exists($"{movieRoot}/{movie.Id}"))
                {
                    Directory.Delete($"{movieRoot}/{movie.Id}", true);
                }
            }
        }
    }
}

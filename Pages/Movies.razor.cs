using AntDesign;
using LANMovie.Common;
using LANMovie.Data.Access;
using LANMovie.Data.Entities;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace LANMovie.Pages
{
    partial class Movies
    {
        // 卡片视图时相关参数
        readonly ListGridType grid = new()
        {
            Gutter = 16,    // 栅格间距
            Xs = 1,         // < 576px 展示的列数
            Lg = 2,         // ≥ 992px 展示的列数
            Xl = 3,         // ≥ 1200px 展示的列数
            Xxl = 4,        // ≥ 1600px 展示的列数 
        };

        MovieEntity[] movies = Array.Empty<MovieEntity>();      


        /// <summary>
        /// 初始化页面
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            using(var context = new OurDbContext())
            {
                var sqlMovieData = new SqlMovieData(context);
                movies = (MovieEntity[])await sqlMovieData.GetAsync();
            }
            await base.OnInitializedAsync();
        }


        /// <summary>
        /// 调用 JS 下载文件
        /// </summary>
        /// <param name="url">文件url</param>
        /// <param name="name">下载时的文件名称</param>
        async Task DownloadFile(string url, string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                await JS.InvokeVoidAsync("downloadFileFromStream", "default-imageName", url);
            }
            else
            {
                await JS.InvokeVoidAsync("downloadFileFromStream", name, url);
            }
        }


        /// <summary>
        /// 删除电影
        /// </summary>
        /// <param name="movieId">电影ID</param>
        /// <returns></returns>
        async Task RemoveMovie(MovieEntity movie)
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
                    movies = movies.Remove(movie);
                    await _message.Success($"{movie.Name} {movie.PublishTime} 已删除!", 2);
                }
                else
                {
                    await _message.Error($"{movie.Name} {movie.PublishTime} 删除失败!", 2);
                }
            }
        }
    }
}

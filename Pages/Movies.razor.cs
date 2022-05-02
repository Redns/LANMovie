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
    }
}

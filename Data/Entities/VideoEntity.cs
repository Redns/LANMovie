using System.ComponentModel;

namespace LANMovie.Data.Entities
{
    public class VideoEntity
    {
        // 8位随机字符串
        public string Id { get; set; } = string.Empty;

        [DisplayName("名 称")]
        public string Name { get; set; } = string.Empty;

        [DisplayName("简 介")]
        public string Description { get; set; } = string.Empty;

        // 用于计算视频的路径
        // 电影：Data/Videos/Movies/{Id}/{VideoPath}
        // 电视剧：Data/Videos/Movies/{Id}/{Page}/{VideoPath.Split(",")[Page - 1]} (Page从1开始)
        // 短视频：Data/Videos/ShortVideos/{Id}/{VideoPath}
        public string VideoPath { get; set; } = string.Empty;

        [DisplayName("发布时间")]
        public string PublishTime { get; set; } = $"{DateTime.Now.Year}";

        // 封面图片相对路径为 {rootpath}/{Id}/{Cover}
        [DisplayName("封 面")]
        public string Cover { get; set; } = string.Empty;

        [DisplayName("地 区")]
        public string Area { get; set; } = string.Empty;


        /// <summary>
        /// 解析影片类型
        /// </summary>
        /// <param name="videoCategory">影片类型字符串</param>
        /// <returns></returns>
        public static VideoCategory ParseVideoCategory(string videoCategory)
        {
            var temp = videoCategory.ToLower();
            if (temp == "movie") { return VideoCategory.Movie; }
            else if (temp == "teleplay") { return VideoCategory.Teleplay; }
            else if (temp == "shortvideo") { return VideoCategory.ShortVideo; }
            else { return VideoCategory.None; }
        }
    }


    /// <summary>
    /// 影片类型
    /// </summary>
    public enum VideoCategory
    {
        Movie = 0,      // 电影
        Teleplay,       // 电视剧
        ShortVideo,     // 短视频
        None
    }
}
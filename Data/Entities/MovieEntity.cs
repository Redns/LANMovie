using System.ComponentModel;

namespace LANMovie.Data.Entities
{
    public class MovieEntity : VideoEntity
    {
        [DisplayName("上传时间")]
        public string UploadTime { get; set; } = $"{DateTime.Now.Year}";

        [DisplayName("导演")]
        public string Director { get; set; } = string.Empty;

        [DisplayName("类 型")]
        public MovieCategory Category { get; set; } = MovieCategory.Other;

        [DisplayName("大 小")]
        public string Size { get; set; } = string.Empty;

        public MovieEntity() { }
        public MovieEntity(string id, string name, string description, string videoPath, string uploadTime, string publishTime, string cover, string director, MovieCategory category, string area, string size)
        {
            Id = id;
            Name = name;
            Description = description;
            VideoPath = videoPath;
            UploadTime = uploadTime;
            PublishTime = publishTime;
            Cover = cover;
            Director = director;
            Category = category;
            Area = area;
            Size = size;
        }
    }


    /// <summary>
    /// 电影分类
    /// </summary>
    public enum MovieCategory
    {
        Drama = 0,                  // 情节片
        Comedy,                     // 喜剧片
        Romantic,                   // 爱情片
        Action,                     // 动作片
        MartialArts,                // 武侠片
        KungFu,                     // 功夫片
        War,                        // 战争片
        Thrill,                     // 悬疑片
        Horror,                     // 恐怖片
        Fantasy,                    // 神幻片
        Scifi,                      // 科幻片
        Ghost,                      // 鬼片
        NewYearCelebration,         // 贺岁片
        Other                       // 其它
    }
}

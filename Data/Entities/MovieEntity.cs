using System.ComponentModel;

namespace LANMovie.Data.Entities
{
    public class MovieEntity
    {
        // 8位随机字符串
        public string Id { get; set; } = string.Empty;

        [DisplayName("名 称")]
        public string Name { get; set; } = string.Empty;

        [DisplayName("简 介")]
        public string Description { get; set; } = string.Empty;

        [DisplayName("上传时间")]
        public string UploadTime { get; set; } = string.Empty;

        [DisplayName("发布时间")]
        public string PublishTime { get; set; } = string.Empty;

        [DisplayName("封 面")]
        public string Cover { get; set; } = string.Empty;

        [DisplayName("导演")]
        public string Director { get; set; } = string.Empty;

        [DisplayName("类 型")]
        public MovieCategory Category { get; set; } = MovieCategory.Other;

        [DisplayName("地 区")]
        public string Area { get; set; } = string.Empty;

        [DisplayName("时 长")]
        public string Duration { get; set; } = string.Empty;

        [DisplayName("大 小")]
        public string Size { get; set; } = string.Empty;

        public MovieEntity() { }
        public MovieEntity(string id, string name, string description, string uploadTime, string publishTime, string cover, string director, MovieCategory category, string area, string duration, string size)
        {
            Id = id;
            Name = name;
            Description = description;
            UploadTime = uploadTime;
            PublishTime = publishTime;
            Cover = cover;
            Director = director;
            Category = category;
            Area = area;
            Duration = duration;
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

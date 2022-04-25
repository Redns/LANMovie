﻿using System.ComponentModel;

namespace LANMovie.Data.Entities
{
    public class MovieEntity
    {
        // 8位随机字符串
        public string Id { get; set; }

        [DisplayName("名 称")]
        public string Name { get; set; }

        [DisplayName("简 介")]
        public string Description { get; set; }

        // 视频相对路径为 Data/Videos/Movies/{Id}/{VideoPath}
        public string VideoPath { get; set; }

        [DisplayName("上传时间")]
        public string UploadTime { get; set; }

        [DisplayName("发布时间")]
        public string PublishTime { get; set; }

        // 封面图片相对路径为 Data/Videos/Movies/{Id}/{Cover}
        [DisplayName("封 面")]
        public string Cover { get; set; }

        public int RequestCount { get; set; }

        [DisplayName("导演")]
        public string Director { get; set; }

        [DisplayName("类 型")]
        public MovieCategory Category { get; set; }

        [DisplayName("地 区")]
        public string Area { get; set; }

        [DisplayName("时 长")]
        public string Duration { get; set; }

        [DisplayName("大 小")]
        public string Size { get; set; }

        public MovieEntity(string id, string name, string description, string videoPath, string uploadTime, string publishTime, string cover, int requestCount, string director, MovieCategory category, string area, string duration, string size)
        {
            Id = id;
            Name = name;
            Description = description;
            VideoPath = videoPath;
            UploadTime = uploadTime;
            PublishTime = publishTime;
            Cover = cover;
            RequestCount = requestCount;
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

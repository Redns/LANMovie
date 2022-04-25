using System.ComponentModel;

namespace LANMovie.Data.Entities
{
    public class ShortVideoEntity
    {
        public string Id { get; set; } = string.Empty;

        [DisplayName("名 称")]
        public string Name { get; set; } = string.Empty;

        [DisplayName("简 介")]
        public string Description { get; set; } = string.Empty;

        [DisplayName("上传时间")]
        public string UploadTime { get; set; } = string.Empty;

        [DisplayName("封 面")]
        public string Cover { get; set; } = string.Empty;

        [DisplayName("类 型")]
        public ShortVideoCategory Category { get; set; } = ShortVideoCategory.Other;

        [DisplayName("访问次数")]
        public int Visits { get; set; } = 0;

        [DisplayName("时 长")]
        public string Duration { get; set; } = string.Empty;

        [DisplayName("大 小")]
        public string Size { get; set; } = string.Empty;

        public ShortVideoEntity() { }
        public ShortVideoEntity(string id, string name, string description, string uploadTime, string cover, ShortVideoCategory category, int visits, string duration, string size)
        {
            Id = id;
            Name = name;
            Description = description;
            UploadTime = uploadTime;
            Cover = cover;
            Category = category;
            Visits = visits;
            Duration = duration;
            Size = size;
        }
    }


    public enum ShortVideoCategory
    {
        Cartoon = 0,            // 动画
        Demon,                  // 鬼畜
        Dance,                  // 舞蹈
        Recreation,             // 娱乐
        Technology,             // 科技
        Cate,                   // 美食
        Car,                    // 汽车
        Sport,                  // 运动
        Game,                   // 游戏
        Music,                  // 音乐
        Knowledge,              // 知识
        News,                   // 咨询
        Life,                   // 生活
        Fashion,                // 时尚
        Other                   // 其它
    }
}

using System.ComponentModel;

namespace LANMovie.Data.Entities
{
    public class TeleplayEntity
    {
        // 影视剧ID, 格式为{影视ID}/{集数}
        public string Id => $"{FatherId}/{Page}";
        public string FatherId { get; set; } = string.Empty;
        public int Page { get; set; } = 1;

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
        public TeleplayCategory Category { get; set; }

        [DisplayName("地 区")]
        public string Area { get; set; } = string.Empty;

        [DisplayName("时 长")]
        public string Duration { get; set; } = string.Empty;

        [DisplayName("大 小")]
        public string Size { get; set; } = string.Empty;
    }


    // TODO 完善电视剧类别
    public enum TeleplayCategory
    {

    }
}

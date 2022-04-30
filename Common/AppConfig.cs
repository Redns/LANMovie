using Newtonsoft.Json;

namespace LANMovie.Common
{
    public class AppConfig
    {
        public const string appConfigPath = "appsettings.json";


        public DataConfig Data { get; set; }

        public AppConfig(DataConfig data)
        {
            Data = data;
        }


        /// <summary>
        /// 解析本地配置文件
        /// </summary>
        /// <returns>计解析成功返回AppConfig对象, 失败返回null</returns>
        public static AppConfig? Parse()
        {
            try
            {
                return JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(appConfigPath));
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="appConfig">待保存的设置对象</param>
        /// <returns>保存成功返回true, 否则返回false</returns>
        public static bool Save(AppConfig appConfig)
        {
            if(appConfig != null)
            {
                try
                {
                    File.WriteAllText(appConfigPath, JsonConvert.SerializeObject(appConfig));
                    return true;
                }
                catch { }
            }
            return false;
        }
    }

    public class DataConfig
    {
        public DatabaseConfig Database { get; set; }
        public VideoConfig Video { get; set; }

        public DataConfig(DatabaseConfig database, VideoConfig video)
        {
            Database = database;
            Video = video;
        }
    }

    public class DatabaseConfig
    {
        public string ConnStr { get; set; }

        public DatabaseConfig(string connStr)
        {
            ConnStr = connStr;
        }
    }

    public class VideoConfig
    {
        public MovieConfig Movie { get; set; }

        public VideoConfig(MovieConfig movie)
        {
            Movie = movie;
        }
    }

    public class MovieConfig
    {
        public long CoverMaxSize { get; set; }      // 封面最大尺寸(单位：Byte)
        public long MaxSize { get; set; }           // 电影视频最大尺寸(单位：Byte)

        public MovieConfig(long coverMaxSize, long maxSize)
        {
            CoverMaxSize = coverMaxSize;
            MaxSize = maxSize;
        }
    }
}

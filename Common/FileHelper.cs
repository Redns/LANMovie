namespace LANMovie.Common
{
    public class FileHelper
    {
        public const long KB = 1024;
        public const long MB = 1024 * 1024;
        public const long GB = 1024 * 1024 * 1024;


        /// <summary>
        /// 获取文件拓展名(不包括.)
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <returns></returns>
        public static string GetExtension(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                return $"{filename.Split(".").Last()}";
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="len">字符串长度</param>
        /// <returns></returns>
        public static string GererateRandomString(int len)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks;
            Random random = new(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> len)));
            for (int i = 0; i < len; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str += ch.ToString();
            }
            return str;
        }


        /// <summary>
        /// 生成唯一文件夹名称
        /// </summary>
        /// <param name="parDir">父文件夹路径</param>
        /// <param name="len">文件夹名称长度</param>
        /// <param name="createDir">是否创建文件夹</param>
        /// <returns></returns>
        public static string GenerateUnitDirName(string parDir, int len, bool createDir = false)
        {
            var dirName = GererateRandomString(len);
            while (Directory.Exists($"{parDir}/{dirName}"))
            {
                dirName = GererateRandomString(len);
            }
            if (createDir)
            {
                Directory.CreateDirectory($"{parDir}/{dirName}");
            }
            return dirName;
        }


        /// <summary>
        /// 生成唯一文件名称
        /// </summary>
        /// <param name="dir">文件所处的文件夹路径</param>
        /// <param name="extension">文件后缀</param>
        /// <param name="len">文件名长度</param>
        /// <returns></returns>
        public static string GenerateUnitFilename(string dir, string extension, int len)
        {
            var filename = $"{GererateRandomString(len)}.{extension}";
            while (File.Exists($"{dir}/{filename}"))
            {
                filename = $"{GererateRandomString(len)}.{extension}";
            }
            return filename;
        }


        /// <summary>
        /// 格式化文件大小
        /// </summary>
        /// <param name="len">文件大小(单位:Byte)</param>
        /// <returns></returns>
        public static string RebuildFileSize(long len)
        {
            if (len < 1 * KB)
            {
                return $"{len}B";
            }
            else if (len < 1 * MB)
            {
                return $"{len / (1 * KB): #.##}KB";
            }
            else if (len < 1 * GB)
            {
                return $"{len / (1 * MB):#.##}MB";
            }
            else if(len < 10 * GB)
            {
                return $"{len / (1 * GB):#.##}GB";
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 解析文件大小字符串(B、KB、MB)
        /// </summary>
        /// <param name="size">格式化后的文件大小</param>
        /// <returns>文件大小(以KB计)</returns>
        public static double ParseFileSize(string size)
        {
            if (size.Contains("MB"))
            {
                return double.Parse(size[0..^2]) * 1024;
            }
            else if (size.Contains("KB"))
            {
                return double.Parse(size[0..^2]);
            }
            else
            {
                return double.Parse(size[0..^2]) / 1024.0;
            }
        }
    }
}

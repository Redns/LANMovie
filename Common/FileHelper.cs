namespace LANMovie.Common
{
    public class FileHelper
    {
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
        /// <returns></returns>
        public static string GenerateUnitDirName(string parDir, int len)
        {
            var dirName = GererateRandomString(len);
            while (Directory.Exists($"{parDir}/{dirName}"))
            {
                dirName = GererateRandomString(len);
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
    }
}

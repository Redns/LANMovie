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
    }
}

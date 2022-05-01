using NetVips;

namespace LANMovie.Common
{
    public class ImageHelper
    {
        /// <summary>
        /// 以中心为原点裁剪图片，目标尺寸为 1920*1440
        /// </summary>
        /// <param name="imageSrcPath">图片源地址</param>
        /// <param name="imageDstPath">图片目的地址</param>
        /// <param name="delSrcImage">是否删除源图片(默认保留)</param>
        /// <returns>裁剪成功返回true, 否则返回false</returns>
        public static Image? ImageCut(string imageSrcPath, bool delSrcImage = false)
        {
            float ratio = 1.5F;
            try
            {
                if (string.IsNullOrEmpty(imageSrcPath) || !File.Exists(imageSrcPath))
                {
                    return null;
                }
                else
                {
                    // 加载原图片
                    var imageSrc = Image.NewFromFile(imageSrcPath);

                    // 删除源图片
                    if (delSrcImage) { File.Delete(imageSrcPath); }

                    var imageDstWidth = 0.0;
                    var imageDstHeight = 0.0;
                    if ((imageSrc.Width == 1620) && (imageSrc.Height == 1080))
                    {
                        return imageSrc;
                    }
                    else
                    {
                        var ratioSrc = imageSrc.Width * 1.0 / imageSrc.Height;
                        if (ratioSrc > ratio)
                        {
                            imageDstHeight = imageSrc.Height;
                            imageDstWidth = imageSrc.Height * ratio;
                        }
                        else if (ratioSrc < ratio)
                        {
                            imageDstWidth = imageSrc.Width;
                            imageDstHeight = imageDstWidth / ratio;
                        }

                        // 计算裁剪边缘距离
                        var imageLeftEdge = (imageSrc.Width - imageDstWidth) / 2;
                        var imageTopEdge = (imageSrc.Height - imageDstHeight) / 2;

                        // 裁剪、放缩图片
                        return imageSrc.Crop((int)imageLeftEdge, (int)imageTopEdge, (int)imageDstWidth, (int)imageDstHeight)
                                        .Resize(1620.0 / imageDstWidth);
                    } 
                }
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 为图片添加水印
        /// </summary>
        /// <param name="image">原图片</param>
        /// <param name="text">水印</param>
        /// <param name="fontSize">水印文字大小(单位：px)</param>
        /// <param name="leftEdge">水印距左侧边缘距离</param>
        /// <param name="bottomEdge">水印距右侧边缘距离</param>
        /// <returns></returns>
        public static Image ImageWaterMark(Image image, string markText, int fontSize = 36, int leftEdge = 0, int bottomEdge = 0)
        {
            using (var textImage = Image.Text(markText, $"Arial {fontSize}px", width: image.Width - 100))
            {
                using (var overlay = textImage.NewFromImage(255, 255, 255)
                                              .Copy(interpretation: Enums.Interpretation.Srgb)
                                              .Bandjoin(textImage))
                {
                    return image.Composite(overlay, Enums.BlendMode.Over, leftEdge, image.Height - bottomEdge);
                }
            }
        }
    }
}

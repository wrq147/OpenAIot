using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;

namespace IoTAIService
{
    public static class ImageExtends
    {
        public static Image<Rgb24> CropByBox(this Image<Rgb24> sourceImage, float x1, float x2, float y1, float y2)
        {
            if (sourceImage == null)
                throw new ArgumentNullException(nameof(sourceImage), "原始图像不能为空");

            // 验证坐标合法性（x2 必须 >= x1，y2 必须 >= y1）
            if (x2 < x1)
                throw new ArgumentException($"x2（{x2}）不能小于 x1（{x1}）");
            if (y2 < y1)
                throw new ArgumentException($"y2（{y2}）不能小于 y1（{y1}）");

            // 计算宽高并构建截取区域
            float width = x2 - x1;
            float height = y2 - y1;
            var cropArea = new Rectangle((int)x1, (int)y1, (int)width, (int)height);

            // 验证区域是否在图像范围内
            if (!IsAreaValid(sourceImage, cropArea))
            {
                throw new ArgumentOutOfRangeException(
                    "截取区域超出图像范围",
                    $"图像尺寸：{sourceImage.Width}x{sourceImage.Height}，截取区域：({x1},{y1}) 至 ({x2},{y2})"
                );
            }

            // 截取区域并返回新图像
            return sourceImage.Clone(ctx => ctx.Crop(cropArea));
        }
        /// <summary>
        /// 验证区域是否在图像范围内
        /// </summary>
        private static bool IsAreaValid(Image<Rgb24> image, Rectangle area)
        {
            return area.X >= 0
                && area.Y >= 0
                && area.Right <= image.Width  // 右下角 X 坐标不超过图像宽度
                && area.Bottom <= image.Height; // 右下角 Y 坐标不超过图像高度
        }
    }
}

using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;

namespace IoTAIService
{
    public static class ImageExtends
    {
        public static Image<Rgb24> ToImage(this Tensor<float> tensor)
        {
            // 1. 验证张量维度（确保单张图片、3通道）
            if (tensor.Dimensions.Length != 4)
                throw new ArgumentException("张量必须是4维 (批次, 通道, 高度, 宽度)");

            int batchSize = tensor.Dimensions[0];
            int channels = tensor.Dimensions[1];
            int height = tensor.Dimensions[2];
            int width = tensor.Dimensions[3];

            if (batchSize != 1 || channels != 3)
                throw new ArgumentException("仅支持单张3通道(RGB)张量");

            // 2. 创建目标图像（宽度×高度）
            var image = new Image<Rgb24>(width, height);

            // 3. 遍历像素并转换
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // 3.1 从张量获取 RGB 通道值（注意通道顺序：若为 BGR 需反转）
                    float r = tensor[0, 0, y, x]; // 第0通道：R
                    float g = tensor[0, 1, y, x]; // 第1通道：G
                    float b = tensor[0, 2, y, x]; // 第2通道：B
                    r = r * 0.229f + 0.485f;
                    g = g * 0.224f + 0.456f;
                    b = b * 0.225f + 0.406f;

                    // 3.2 转换为 byte（根据实际范围调整映射公式）
                    byte rByte = ClampToByte(r * 255f);
                    byte gByte = ClampToByte(g * 255f);
                    byte bByte = ClampToByte(b * 255f);

                    // 3.3 设置图像像素
                    image[x, y] = new Rgb24(rByte, gByte, bByte);
                }
            }
            return image;
        }
        private static byte ClampToByte(float value)
        {
            return (byte)Math.Clamp(Math.Round(value), byte.MinValue, byte.MaxValue);
        }
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

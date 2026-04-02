using ChannelUtility.Message;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.AICode
{
    /// <summary>
    /// ReID特征提取接口（标准化对接OSNet）
    /// </summary>
    public interface IReIDExtractor
    {
        /// <summary>
        /// 从图像ROI中提取ReID特征
        /// </summary>
        /// <param name="image">原始图像</param>
        /// <param name="roi">目标检测框（x1,y1,x2,y2）</param>
        /// <returns>归一化的特征向量（OSNet默认512维）</returns>
        float[] ExtractFeature(Image<Rgb24> image, BoxItem roi);

        /// <summary>
        /// 计算两个特征的余弦相似度
        /// </summary>
        /// <param name="feat1">特征1</param>
        /// <param name="feat2">特征2</param>
        /// <returns>相似度（0-1）</returns>
        float CosineSimilarity(float[] feat1, float[] feat2);
    }

}

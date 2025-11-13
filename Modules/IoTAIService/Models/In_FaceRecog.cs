using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.Models
{
    public class In_FaceRecog
    {
        /// <summary>
        /// Base64图片
        /// </summary>
        public string ImageBase64 { get; set; }
        /// <summary>
        /// 图片的 Url
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 是否启用空间变换
        /// </summary>
        public bool EnableSTN { get; set; } = true;
        /// <summary>
        /// 阈值
        /// </summary>
        public float Threshold { get; set; } = 0.8f;
        /// <summary>
        /// 排除交并比阈值
        /// </summary>
        public float IOU_Threshold { get; set; } = 0.2f;
        /// <summary>
        /// 最小相似度得分
        /// </summary>
        public float SimiScore { get; set; } = 0.8f;
        /// <summary>
        /// 过滤的人脸库
        /// </summary>
        public string HouseId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.AICode
{
    public class YoloDetectionResult
    {
        public string Label { get; set; }    // 类别标签
        public float Confidence { get; set; } // 置信度
        public float X1 { get; set; }        // 检测框左上角X（原图坐标）
        public float Y1 { get; set; }        // 检测框左上角Y（原图坐标）
        public float X2 { get; set; }        // 检测框右下角X（原图坐标）
        public float Y2 { get; set; }        // 检测框右下角Y（原图坐标）
    }
}

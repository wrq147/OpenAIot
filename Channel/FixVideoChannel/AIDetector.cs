using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixVideoChannel
{
    public class AIDetector : IDisposable
    {
        // 模拟检测结果
        public class DetectionBox
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public string Label { get; set; }
            public float Confidence { get; set; }
        }

        // 模拟AI检测
        public Task Detect(byte[] rgbFrame, int width, int height, RtspParam rtspParam)
        {
            //AI检测

            //绘制检测框
            if (rtspParam.EnableDrawDetection)
            {

            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // 释放AI模型资源
        }
    }
}

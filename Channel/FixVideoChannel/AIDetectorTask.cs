using System;
namespace FixVideoChannel
{
    public class AIDetectorTask
    {
        /// <summary>
        /// 检测类型
        /// </summary>
        public string DetectType { get; set; }
        // AI检测
        public virtual Task Detect(byte[] rgbFrame, int width, int height)
        {
            return Task.CompletedTask;
        }
    }
}

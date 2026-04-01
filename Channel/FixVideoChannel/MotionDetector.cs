using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ZLMediaKit.Autogen;
namespace FixVideoChannel
{
    /// <summary>
    /// 画面变化检测
    /// </summary>
    public class MotionDetector
    {
        public int DiffThreshold { get; set; } = 25;
        public float MotionBlockRatioThreshold { get; set; } = 0.015f;
        public int MinMotionBlocks { get; set; } = 6;
        public int BlockSize { get; set; } = 32;
        public int CoolDownMs { get; set; } = 200;

        private byte[] _lastY;
        private long _lastTriggerTime;

        public (bool isMotion, float ratio) IsMotionKeyframe(byte[] rgb24, int width, int height)
        {
            long now = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            if (now - _lastTriggerTime < CoolDownMs)
                return (false, 0);

            if (_lastY == null || _lastY.Length != width * height)
            {
                _lastY = new byte[width * height];
                LibConvert.ExtractY(rgb24, _lastY, width, height);
                return (false, 0);
            }

            float ratio = 0;
            int ret = LibConvert.CheckBlockMotion(
                _lastY,
                rgb24,
                width,
                height,
                BlockSize,
                DiffThreshold,
                MotionBlockRatioThreshold,
                MinMotionBlocks,
                ref ratio
            );

            LibConvert.ExtractY(rgb24, _lastY, width, height);

            bool isMotion = ret == 1;
            if (isMotion)
                _lastTriggerTime = now;

            return (isMotion, ratio);
        }


        public void ResetContext()
        {
            _lastY = null;
            _lastTriggerTime = 0;
        }
    }
}

using System;
using ZLMediaKit.Autogen;


namespace GB28181Channel
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

        private byte[] _lastY;


        public (bool isMotion, float ratio) IsMotionKeyframe(byte[] rgb24, int width, int height)
        {
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

            return (ret == 1, ratio);
        }


        public void ResetContext()
        {
            _lastY = null;
        }
    }
}

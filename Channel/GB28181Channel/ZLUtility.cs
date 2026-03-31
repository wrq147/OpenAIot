using System;
using System.Numerics;
using ZLMediaKit.Autogen;

namespace GB28181Channel
{
    public static class ZLUtility
    {
        /// <summary>
        /// RGB24转换到目标YUV格式（C原生速度版）
        /// </summary>
        public static bool ConvertRgb24ToTargetYuv(byte[] rgb24Data, int width, int height, int alignedLinesize, AVPixelFormat targetFmt, out byte[] yuvData, out int[] yuvLineSizes)
        {
            // 参数校验
            if (rgb24Data == null || rgb24Data.Length < alignedLinesize * height || width <= 0 || height <= 0 || (width & 1) != 0 || (height & 1) != 0)
            {
                yuvData = null;
                yuvLineSizes = null;
                return false;
            }

            yuvData = null;
            yuvLineSizes = null;

            switch (targetFmt)
            {
                case AVPixelFormat.AV_PIX_FMT_YUV420P:
                    yuvData = ConvertRgb24ToYuv420P_Native(rgb24Data, width, height, alignedLinesize);
                    yuvLineSizes = new int[3] { width, width / 2, width / 2 };
                    return true;

                case AVPixelFormat.AV_PIX_FMT_YUV422P:
                    yuvData = ConvertRgb24ToYuv422P_Native(rgb24Data, width, height, alignedLinesize);
                    yuvLineSizes = new int[3] { width, width / 2, width / 2 };
                    return true;

                case AVPixelFormat.AV_PIX_FMT_YUV444P:
                    yuvData = ConvertRgb24ToYuv444P_Native(rgb24Data, width, height, alignedLinesize);
                    yuvLineSizes = new int[3] { width, width, width };
                    return true;

                case AVPixelFormat.AV_PIX_FMT_NV12:
                    yuvData = ConvertRgb24ToNV12_Native(rgb24Data, width, height, alignedLinesize);
                    yuvLineSizes = new int[2] { width, width };
                    return true;
                case AVPixelFormat.AV_PIX_FMT_YUVJ420P:
                    yuvData = ConvertRgb24ToYuv420P_Native(rgb24Data, width, height, alignedLinesize);
                    yuvLineSizes = new int[3] { width, width / 2, width / 2 };
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// C原生速度 - RGB24转YUV420P
        /// </summary>
        public unsafe static byte[] ConvertRgb24ToYuv420P_Native(byte[] rgb24Data, int width, int height, int alignedLinesize)
        {
            int ySize = width * height;
            int uvSize = ySize / 4;
            byte[] yuv420p = new byte[ySize + uvSize * 2];

            fixed (byte* pRgb = rgb24Data)
            fixed (byte* pYuv = yuv420p)
            {
                Rgb24ToYUV.rgb24_to_yuv420p(pRgb, pYuv, width, height, alignedLinesize);
            }

            return yuv420p;
        }

        /// <summary>
        /// C原生速度 - RGB24转YUV422P
        /// </summary>
        public unsafe static byte[] ConvertRgb24ToYuv422P_Native(byte[] rgb24Data, int width, int height, int alignedLinesize)
        {
            int ySize = width * height;
            int uvSize = ySize / 2;
            byte[] yuv422p = new byte[ySize + uvSize * 2];

            fixed (byte* pRgb = rgb24Data)
            fixed (byte* pYuv = yuv422p)
            {
                Rgb24ToYUV.rgb24_to_yuv422p(pRgb, pYuv, width, height, alignedLinesize);
            }

            return yuv422p;
        }

        /// <summary>
        /// C原生速度 - RGB24转YUV444P
        /// </summary>
        public unsafe static byte[] ConvertRgb24ToYuv444P_Native(byte[] rgb24Data, int width, int height, int alignedLinesize)
        {
            int ySize = width * height;
            byte[] yuv444p = new byte[ySize * 3];

            fixed (byte* pRgb = rgb24Data)
            fixed (byte* pYuv = yuv444p)
            {
                Rgb24ToYUV.rgb24_to_yuv444p(pRgb, pYuv, width, height, alignedLinesize);
            }

            return yuv444p;
        }

        /// <summary>
        /// C原生速度 - RGB24转NV12
        /// </summary>
        public unsafe static byte[] ConvertRgb24ToNV12_Native(byte[] rgb24Data, int width, int height, int alignedLinesize)
        {
            int ySize = width * height;
            byte[] nv12 = new byte[ySize + ySize / 2];

            fixed (byte* pRgb = rgb24Data)
            fixed (byte* pNv12 = nv12)
            {
                Rgb24ToYUV.rgb24_to_nv12(pRgb, pNv12, width, height, alignedLinesize);
            }

            return nv12;
        }
    }

    /// <summary>
    /// 像素格式枚举（兼容FFmpeg）
    /// </summary>
    public enum AVPixelFormat
    {
        AV_PIX_FMT_NONE = -1,
        AV_PIX_FMT_YUV420P,   // 0
        AV_PIX_FMT_YUYV422,   // 1
        AV_PIX_FMT_RGB24,     // 2
        AV_PIX_FMT_BGR24,     // 3
        AV_PIX_FMT_YUV422P,   // 4
        AV_PIX_FMT_YUV444P,   // 5
        AV_PIX_FMT_YUV410P,   // 6
        AV_PIX_FMT_YUV411P,   // 7
        AV_PIX_FMT_GRAY8,     // 8
        AV_PIX_FMT_MONOWHITE, // 9
        AV_PIX_FMT_MONOBLACK, //10
        AV_PIX_FMT_PAL8,      //11
        AV_PIX_FMT_YUVJ420P,  //12
        AV_PIX_FMT_NV12 = 23,
    }
}

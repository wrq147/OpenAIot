using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel
{
    public static class ZLUtility
    {
        public static string SsrcToStreamId(string ssrc)
        {
            return int.Parse(ssrc.Substring(1)).ToString("x");
        }
        /// <summary>
        /// RGB24转换到目标YUV格式
        /// </summary>
        /// <param name="rgb24Data">输入RGB24数据</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="alignedLinesize">对齐后的行大小</param>
        /// <param name="targetFmt">目标YUV格式</param>
        /// <param name="yuvData">输出YUV数据</param>
        /// <param name="yuvLineSizes">输出YUV行大小数组</param>
        /// <returns>是否转换成功</returns>
        public static bool ConvertRgb24ToTargetYuv(byte[] rgb24Data, int width, int height, int alignedLinesize, AVPixelFormat targetFmt, out byte[] yuvData, out int[] yuvLineSizes)
        {
            yuvData = null;
            yuvLineSizes = null;
            int ySize = width * height;

            switch (targetFmt)
            {
                case AVPixelFormat.AV_PIX_FMT_YUV420P:
                    yuvData = ConvertRgb24ToYuv420P(rgb24Data, width, height, alignedLinesize);
                    // Y:width, U:width/2, V:width/2
                    yuvLineSizes = new int[3] { width, width / 2, width / 2 };
                    return true;

                case AVPixelFormat.AV_PIX_FMT_YUV422P:
                    yuvData = ConvertRgb24ToYuv422P(rgb24Data, width, height, alignedLinesize);
                    // Y:width, U:width/2, V:width/2
                    yuvLineSizes = new int[3] { width, width / 2, width / 2 };
                    return true;

                case AVPixelFormat.AV_PIX_FMT_YUV444P:
                    yuvData = ConvertRgb24ToYuv444P(rgb24Data, width, height, alignedLinesize);
                    // Y/U/V均为width
                    yuvLineSizes = new int[3] { width, width, width };
                    return true;

                case AVPixelFormat.AV_PIX_FMT_NV12:
                    yuvData = ConvertRgb24ToNV12(rgb24Data, width, height, alignedLinesize);
                    // Y:width, UV:width
                    yuvLineSizes = new int[2] { width, width };
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// RGB24转YUV420P
        /// </summary>
        private static byte[] ConvertRgb24ToYuv420P(byte[] rgb24Data, int width, int height, int alignedLinesize)
        {
            int ySize = width * height;
            int uvSize = ySize / 4;
            byte[] yuv420p = new byte[ySize + uvSize * 2];

            unsafe
            {
                fixed (byte* pRgb = rgb24Data)
                fixed (byte* pYuv = yuv420p)
                {
                    byte* pY = pYuv;
                    byte* pU = pYuv + ySize;
                    byte* pV = pYuv + ySize + uvSize;

                    for (int y = 0; y < height; y++)
                    {
                        byte* pRgbRow = pRgb + y * alignedLinesize;
                        for (int x = 0; x < width; x++)
                        {
                            // RGB24格式：每个像素按 R → G → B 顺序存储
                            byte r = pRgbRow[x * 3];
                            byte g = pRgbRow[x * 3 + 1];
                            byte b = pRgbRow[x * 3 + 2];

                            // Y分量计算
                            pY[y * width + x] = (byte)Math.Round(0.299 * r + 0.587 * g + 0.114 * b);

                            // 4:2:0 采样：仅偶数行偶数列计算UV
                            if (y % 2 == 0 && x % 2 == 0)
                            {
                                int uvIdx = (y / 2) * (width / 2) + (x / 2);
                                pU[uvIdx] = (byte)Math.Round(-0.14713 * r - 0.28886 * g + 0.436 * b + 128);
                                pV[uvIdx] = (byte)Math.Round(0.615 * r - 0.51499 * g - 0.10001 * b + 128);
                            }
                        }
                    }
                }
            }
            return yuv420p;
        }

        /// <summary>
        /// RGB24转YUV422P
        /// </summary>
        private static byte[] ConvertRgb24ToYuv422P(byte[] rgb24Data, int width, int height, int alignedLinesize)
        {
            int ySize = width * height;
            int uvSize = ySize / 2;
            byte[] yuv422p = new byte[ySize + uvSize * 2];

            unsafe
            {
                fixed (byte* pRgb = rgb24Data)
                fixed (byte* pYuv = yuv422p)
                {
                    byte* pY = pYuv;
                    byte* pU = pYuv + ySize;
                    byte* pV = pYuv + ySize + uvSize;

                    for (int y = 0; y < height; y++)
                    {
                        byte* pRgbRow = pRgb + y * alignedLinesize;
                        for (int x = 0; x < width; x++)
                        {
                            // RGB24格式：每个像素按 R → G → B 顺序存储
                            byte r = pRgbRow[x * 3];
                            byte g = pRgbRow[x * 3 + 1];
                            byte b = pRgbRow[x * 3 + 2];

                            pY[y * width + x] = (byte)Math.Round(0.299 * r + 0.587 * g + 0.114 * b);

                            // 4:2:2 采样：每2列计算一次UV
                            if (x % 2 == 0)
                            {
                                int uvIdx = (y * width / 2) + (x / 2);
                                pU[uvIdx] = (byte)Math.Round(-0.14713 * r - 0.28886 * g + 0.436 * b + 128);
                                pV[uvIdx] = (byte)Math.Round(0.615 * r - 0.51499 * g - 0.10001 * b + 128);
                            }
                        }
                    }
                }
            }
            return yuv422p;
        }

        /// <summary>
        /// RGB24转YUV444P
        /// </summary>
        private static byte[] ConvertRgb24ToYuv444P(byte[] rgb24Data, int width, int height, int alignedLinesize)
        {
            int ySize = width * height;
            byte[] yuv444p = new byte[ySize * 3];

            unsafe
            {
                fixed (byte* pRgb = rgb24Data)
                fixed (byte* pYuv = yuv444p)
                {
                    byte* pY = pYuv;
                    byte* pU = pYuv + ySize;
                    byte* pV = pYuv + ySize * 2;

                    for (int y = 0; y < height; y++)
                    {
                        byte* pRgbRow = pRgb + y * alignedLinesize;
                        for (int x = 0; x < width; x++)
                        {
                            // RGB24格式：每个像素按 R → G → B 顺序存储
                            byte r = pRgbRow[x * 3];
                            byte g = pRgbRow[x * 3 + 1];
                            byte b = pRgbRow[x * 3 + 2];

                            int idx = y * width + x;
                            pY[idx] = (byte)Math.Round(0.299 * r + 0.587 * g + 0.114 * b);
                            pU[idx] = (byte)Math.Round(-0.14713 * r - 0.28886 * g + 0.436 * b + 128);
                            pV[idx] = (byte)Math.Round(0.615 * r - 0.51499 * g - 0.10001 * b + 128);
                        }
                    }
                }
            }
            return yuv444p;
        }

        /// <summary>
        /// RGB24转NV12（Y + UV交织）
        /// </summary>
        private static byte[] ConvertRgb24ToNV12(byte[] rgb24Data, int width, int height, int alignedLinesize)
        {
            int ySize = width * height;
            int uvSize = ySize / 2;
            byte[] nv12 = new byte[ySize + uvSize];

            unsafe
            {
                fixed (byte* pRgb = rgb24Data)
                fixed (byte* pNv12 = nv12)
                {
                    byte* pY = pNv12;
                    byte* pUV = pNv12 + ySize;

                    for (int y = 0; y < height; y++)
                    {
                        byte* pRgbRow = pRgb + y * alignedLinesize;
                        for (int x = 0; x < width; x++)
                        {
                            // RGB24格式：每个像素按 R → G → B 顺序存储
                            byte r = pRgbRow[x * 3];
                            byte g = pRgbRow[x * 3 + 1];
                            byte b = pRgbRow[x * 3 + 2];

                            pY[y * width + x] = (byte)Math.Round(0.299 * r + 0.587 * g + 0.114 * b);

                            // NV12采样：偶数行偶数列计算UV，UV交织存储
                            if (y % 2 == 0 && x % 2 == 0)
                            {
                                int uvIdx = ((y / 2) * width + x) / 2 * 2;
                                byte u = (byte)Math.Round(-0.14713 * r - 0.28886 * g + 0.436 * b + 128);
                                byte v = (byte)Math.Round(0.615 * r - 0.51499 * g - 0.10001 * b + 128);
                                pUV[uvIdx] = u;
                                pUV[uvIdx + 1] = v;
                            }
                        }
                    }
                }
            }
            return nv12;
        }
    }

    public enum AVPixelFormat
    {
        // 未定义/无效格式
        AV_PIX_FMT_NONE = -1,
        // YUV 系列
        AV_PIX_FMT_YUV420P = 0,     // YUV420P（最常用）
        AV_PIX_FMT_YUV422P = 4,     // YUV422P
        AV_PIX_FMT_YUV444P = 5,     // YUV444P
        AV_PIX_FMT_NV12 = 23,       // NV12（Y + UV 交错）
                                    // RGB/BGR 系列
        AV_PIX_FMT_RGB24 = 2,       // RGB24（R→G→B）
        AV_PIX_FMT_BGR24 = 3        // BGR24（B→G→R，ZLMediaKit 默认）
    }
}

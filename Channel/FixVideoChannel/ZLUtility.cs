using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FixVideoChannel
{
    public static class ZLUtility
    {
        // <summary>
        /// BGR24转换到目标YUV格式
        /// </summary>
        /// <param name="bgr24Data">输入BGR24数据</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="alignedLinesize">对齐后的行大小</param>
        /// <param name="targetFmt">目标YUV格式</param>
        /// <param name="yuvData">输出YUV数据</param>
        /// <param name="yuvLineSizes">输出YUV行大小数组</param>
        /// <returns>是否转换成功</returns>
        public static bool ConvertBgr24ToTargetYuv(byte[] bgr24Data, int width, int height, int alignedLinesize, AVPixelFormat targetFmt, out byte[] yuvData, out int[] yuvLineSizes)
        {
            yuvData = null;
            yuvLineSizes = null;
            int ySize = width * height;

            switch (targetFmt)
            {
                case AVPixelFormat.AV_PIX_FMT_YUV420P:
                    yuvData = ConvertBgr24ToYuv420P(bgr24Data, width, height, alignedLinesize);
                    // Y:width, U:width/2, V:width/2
                    yuvLineSizes = new int[3] { width, width / 2, width / 2 };
                    return true;

                case AVPixelFormat.AV_PIX_FMT_YUV422P:
                    yuvData = ConvertBgr24ToYuv422P(bgr24Data, width, height, alignedLinesize);
                    // Y:width, U:width/2, V:width/2
                    yuvLineSizes = new int[3] { width, width / 2, width / 2 };
                    return true;

                case AVPixelFormat.AV_PIX_FMT_YUV444P:
                    yuvData = ConvertBgr24ToYuv444P(bgr24Data, width, height, alignedLinesize);
                    // Y/U/V均为width
                    yuvLineSizes = new int[3] { width, width, width };
                    return true;

                case AVPixelFormat.AV_PIX_FMT_NV12:
                    yuvData = ConvertBgr24ToNV12(bgr24Data, width, height, alignedLinesize);
                    // Y:width, UV:width
                    yuvLineSizes = new int[2] { width, width };
                    return true;

                default:
                    return false;
            }
        }
        /// <summary>
        /// BGR24转YUV420P
        /// </summary>
        private static byte[] ConvertBgr24ToYuv420P(byte[] bgr24Data, int width, int height, int alignedLinesize)
        {
            int ySize = width * height;
            int uvSize = ySize / 4;
            byte[] yuv420p = new byte[ySize + uvSize * 2];

            unsafe
            {
                fixed (byte* pBgr = bgr24Data)
                fixed (byte* pYuv = yuv420p)
                {
                    byte* pY = pYuv;
                    byte* pU = pYuv + ySize;
                    byte* pV = pYuv + ySize + uvSize;

                    for (int y = 0; y < height; y++)
                    {
                        byte* pBgrRow = pBgr + y * alignedLinesize;
                        for (int x = 0; x < width; x++)
                        {
                            byte b = pBgrRow[x * 3];
                            byte g = pBgrRow[x * 3 + 1];
                            byte r = pBgrRow[x * 3 + 2];

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
        /// BGR24转YUV422P
        /// </summary>
        private static byte[] ConvertBgr24ToYuv422P(byte[] bgr24Data, int width, int height, int alignedLinesize)
        {
            int ySize = width * height;
            int uvSize = ySize / 2;
            byte[] yuv422p = new byte[ySize + uvSize * 2];

            unsafe
            {
                fixed (byte* pBgr = bgr24Data)
                fixed (byte* pYuv = yuv422p)
                {
                    byte* pY = pYuv;
                    byte* pU = pYuv + ySize;
                    byte* pV = pYuv + ySize + uvSize;

                    for (int y = 0; y < height; y++)
                    {
                        byte* pBgrRow = pBgr + y * alignedLinesize;
                        for (int x = 0; x < width; x++)
                        {
                            byte b = pBgrRow[x * 3];
                            byte g = pBgrRow[x * 3 + 1];
                            byte r = pBgrRow[x * 3 + 2];

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
        /// BGR24转YUV444P
        /// </summary>
        private static byte[] ConvertBgr24ToYuv444P(byte[] bgr24Data, int width, int height, int alignedLinesize)
        {
            int ySize = width * height;
            byte[] yuv444p = new byte[ySize * 3];

            unsafe
            {
                fixed (byte* pBgr = bgr24Data)
                fixed (byte* pYuv = yuv444p)
                {
                    byte* pY = pYuv;
                    byte* pU = pYuv + ySize;
                    byte* pV = pYuv + ySize * 2;

                    for (int y = 0; y < height; y++)
                    {
                        byte* pBgrRow = pBgr + y * alignedLinesize;
                        for (int x = 0; x < width; x++)
                        {
                            byte b = pBgrRow[x * 3];
                            byte g = pBgrRow[x * 3 + 1];
                            byte r = pBgrRow[x * 3 + 2];

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
        /// BGR24转NV12（Y + UV交织）
        /// </summary>
        private static byte[] ConvertBgr24ToNV12(byte[] bgr24Data, int width, int height, int alignedLinesize)
        {
            int ySize = width * height;
            int uvSize = ySize / 2;
            byte[] nv12 = new byte[ySize + uvSize];

            unsafe
            {
                fixed (byte* pBgr = bgr24Data)
                fixed (byte* pNv12 = nv12)
                {
                    byte* pY = pNv12;
                    byte* pUV = pNv12 + ySize;

                    for (int y = 0; y < height; y++)
                    {
                        byte* pBgrRow = pBgr + y * alignedLinesize;
                        for (int x = 0; x < width; x++)
                        {
                            byte b = pBgrRow[x * 3];
                            byte g = pBgrRow[x * 3 + 1];
                            byte r = pBgrRow[x * 3 + 2];

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
        /// <summary>
        /// 拆分YUV数据为平面指针数组（适配ZLMediaKit的string[]参数）
        /// </summary>
        public static string[] SplitYuvPlanes(byte[] yuvData, int width, int height, AVPixelFormat fmt)
        {
            List<string> planes = new List<string>();
            int ySize = width * height;

            unsafe
            {
                fixed (byte* pYuv = yuvData)
                {
                    switch (fmt)
                    {
                        case AVPixelFormat.AV_PIX_FMT_YUV420P:
                            // Y平面 | U平面(ySize) | V平面(ySize*5/4)
                            planes.Add(Marshal.PtrToStringAnsi((IntPtr)pYuv));
                            planes.Add(Marshal.PtrToStringAnsi((IntPtr)(pYuv + ySize)));
                            planes.Add(Marshal.PtrToStringAnsi((IntPtr)(pYuv + ySize * 5 / 4)));
                            break;

                        case AVPixelFormat.AV_PIX_FMT_YUV422P:
                            // Y平面 | U平面(ySize) | V平面(ySize*3/2)
                            planes.Add(Marshal.PtrToStringAnsi((IntPtr)pYuv));
                            planes.Add(Marshal.PtrToStringAnsi((IntPtr)(pYuv + ySize)));
                            planes.Add(Marshal.PtrToStringAnsi((IntPtr)(pYuv + ySize * 3 / 2)));
                            break;

                        case AVPixelFormat.AV_PIX_FMT_YUV444P:
                            // Y平面 | U平面(ySize) | V平面(ySize*2)
                            planes.Add(Marshal.PtrToStringAnsi((IntPtr)pYuv));
                            planes.Add(Marshal.PtrToStringAnsi((IntPtr)(pYuv + ySize)));
                            planes.Add(Marshal.PtrToStringAnsi((IntPtr)(pYuv + ySize * 2)));
                            break;

                        case AVPixelFormat.AV_PIX_FMT_NV12:
                            // Y平面 | UV平面(ySize)
                            planes.Add(Marshal.PtrToStringAnsi((IntPtr)pYuv));
                            planes.Add(Marshal.PtrToStringAnsi((IntPtr)(pYuv + ySize)));
                            break;
                    }
                }
            }

            return planes.ToArray();
        }
    }
}

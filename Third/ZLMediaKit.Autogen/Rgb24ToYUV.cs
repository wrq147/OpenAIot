using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ZLMediaKit.Autogen
{
    public unsafe static class Rgb24ToYUV
    {

        // RGB24转NV12
        [DllImport("convert", CallingConvention = CallingConvention.Cdecl)]
        public static extern void rgb24_to_nv12(
            byte* rgb24,
            byte* nv12,
            int width,
            int height,
            int rgb_stride);

        // RGB24转YUV420P
        [DllImport("convert", CallingConvention = CallingConvention.Cdecl)]
        public static extern void rgb24_to_yuv420p(
            byte* rgb24,
            byte* yuv420p,
            int width,
            int height,
            int rgb_stride);

        // RGB24转YUV422P
        [DllImport("convert", CallingConvention = CallingConvention.Cdecl)]
        public static extern void rgb24_to_yuv422p(
            byte* rgb24,
            byte* yuv422p,
            int width,
            int height,
            int rgb_stride);

        // RGB24转YUV444P
        [DllImport("convert", CallingConvention = CallingConvention.Cdecl)]
        public static extern void rgb24_to_yuv444p(
            byte* rgb24,
            byte* yuv444p,
            int width,
            int height,
            int rgb_stride);
    }
}

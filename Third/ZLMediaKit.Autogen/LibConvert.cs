using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ZLMediaKit.Autogen
{
    public unsafe static class LibConvert
    {
        private static readonly string NativeBasePath = Path.Combine("ZLMediaLibs");
        static LibConvert()
        {

            string os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win" : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" : "osx";

            string arch = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X86 => "x86",
                Architecture.X64 => "x64",
                Architecture.Arm => "arm",
                Architecture.Arm64 => "arm64",
                _ => "x64"
            };
            string nativeDir = Path.Combine(NativeBasePath, $"{os}-{arch}");

            // 3. 把目录加入 PATH（系统会自动搜索这里）
            string path = Environment.GetEnvironmentVariable("PATH") ?? "";
            if (!path.Contains(nativeDir))
            {
                Environment.SetEnvironmentVariable("PATH", $"{nativeDir};{path}");
            }
        }

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


        [DllImport("convert", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CheckBlockMotion(
    byte[] last_y,
    byte[] curr_rgb24,
    int width,
    int height,
    int block_size,
    int diff_thresh,
    float ratio_thresh,
    int min_blocks,
    ref float out_ratio
);

        [DllImport("convert", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ExtractY(byte[] rgb24, byte[] out_y, int width, int height);

        [DllImport("convert", CallingConvention = CallingConvention.Cdecl)]
        public static extern void yuv_render(
            IntPtr data,        // YUV 数据指针数组 [Y, U, V]
            IntPtr yuvLineSizes,   // 行宽数组 [Y行宽, U行宽, V行宽]
            int w,                // 画面宽度
            int h,                // 画面高度
            int pix_fmt,          // 像素格式（用上面常量）
            int x1, int y1,       // 矩形左上角
            int x2, int y2,       // 矩形右下角
            byte r, byte g, byte b,// 框颜色 RGB
            [MarshalAs(UnmanagedType.LPStr)] string label // 中文标签
        );


        [DllImport("ff_h264_encoder", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ff_h264_encoder_create(int width, int height, int fps, int bitrate);

        [DllImport("ff_h264_encoder", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ff_h264_encode_frame(IntPtr handle, IntPtr[] yuv, int[] linesize, long pts, int pix_fmt, out IntPtr out_data, out int out_len);

        [DllImport("ff_h264_encoder", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ff_h264_free(IntPtr data);

        [DllImport("ff_h264_encoder", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ff_h264_encoder_destroy(IntPtr handle);
    }
}

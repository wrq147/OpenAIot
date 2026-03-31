using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ZLMediaKit.Autogen
{
    public unsafe static class CheckMotion
    {
        [DllImport("motion", CallingConvention = CallingConvention.Cdecl)]
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

        [DllImport("motion", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ExtractY(byte[] rgb24, byte[] out_y, int width, int height);
    }
}

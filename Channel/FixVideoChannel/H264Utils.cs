using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FixVideoChannel
{
    public static class H264Utils
    {
        // 检测是否包含 Annex-B 起始码
        public static bool HasAnnexBStartCode(byte[] data)
        {
            if (data.Length >= 3)
            {
                if (data[0] == 0 && data[1] == 0 && data[2] == 1) return true;
            }
            if (data.Length >= 4)
            {
                if (data[0] == 0 && data[1] == 0 && data[2] == 0 && data[3] == 1) return true;
            }
            return false;
        }
        /// <summary>
        /// 严格去除NALU中的Annex-B起始码（00 00 00 01 或 00 00 01）
        /// </summary>
        public static byte[] RemoveAnnexBStartCode(byte[] nalu)
        {
            if (nalu == null || nalu.Length < 3)
                return nalu;

            // 优先检查4字节起始码（00 00 00 01）
            if (nalu.Length >= 4 && nalu[0] == 0x00 && nalu[1] == 0x00 && nalu[2] == 0x00 && nalu[3] == 0x01)
            {
                byte[] pure = new byte[nalu.Length - 4];
                Array.Copy(nalu, 4, pure, 0, pure.Length);
                return pure;
            }

            // 检查3字节起始码（00 00 01）
            if (nalu[0] == 0x00 && nalu[1] == 0x00 && nalu[2] == 0x01)
            {
                byte[] pure = new byte[nalu.Length - 3];
                Array.Copy(nalu, 3, pure, 0, pure.Length);
                return pure;
            }

            // 无起始码，直接返回
            return nalu;
        }
        /// <summary>
        /// 从 FFmpeg 的 AVCodecContext 中提取 H.264 的 SPS 和 PPS 列表（支持多 SPS/PPS）
        /// </summary>
        /// <param name="videoCodecCtx">视频解码器上下文</param>
        /// <returns>(SPS 列表, PPS 列表)，若解析失败则返回空列表</returns>
        public static unsafe (List<byte[]> spsList, List<byte[]> ppsList) ExtractSpsPpsListFromStream(AVCodecContext* videoCodecCtx)
        {
            // 初始化空列表
            var spsList = new List<byte[]>();
            var ppsList = new List<byte[]>();

            // 1. 校验入参和 extradata 有效性
            if (videoCodecCtx == null || videoCodecCtx->extradata == null || videoCodecCtx->extradata_size < 8)
            {
                Console.WriteLine("Extradata 为空或长度不足，无法解析 SPS/PPS");
                return (spsList, ppsList);
            }

            try
            {
                // 2. 将非托管的 extradata 复制到托管字节数组
                byte[] avccData = new byte[videoCodecCtx->extradata_size];
                Marshal.Copy((IntPtr)videoCodecCtx->extradata, avccData, 0, videoCodecCtx->extradata_size);

                // 3. 解析 AVCC 格式头部字段
                int currentOffset = 0;

                // 跳过 configurationVersion (1字节)、AVCProfileIndication (1)、profile_compatibility (1)、AVCLevelIndication (1)
                currentOffset += 4;

                // 字节4：高6位保留位，低2位是 nalu_length_size - 1（暂存，可用于后续 NALU 解析）
                byte lengthSizeMinusOne = (byte)(avccData[currentOffset++] & 0x03);

                // 字节5：高3位保留位（0xE0），低5位是 SPS 的数量
                int spsCount = avccData[currentOffset++] & 0x1F;
                if (spsCount <= 0)
                {
                    Console.WriteLine("SPS 数量为0，无法解析");
                    return (spsList, ppsList);
                }

                // 4. 解析所有 SPS
                for (int i = 0; i < spsCount; i++)
                {
                    // 检查偏移是否越界（SPS 长度占2字节）
                    if (currentOffset + 2 > avccData.Length)
                    {
                        Console.WriteLine("解析 SPS 时偏移越界，终止解析");
                        return (spsList, ppsList);
                    }

                    // 读取 SPS 长度（大端序：高字节在前）
                    int spsLen = (avccData[currentOffset] << 8) | avccData[currentOffset + 1];
                    currentOffset += 2;

                    // 检查 SPS 数据是否越界
                    if (spsLen <= 0 || currentOffset + spsLen > avccData.Length)
                    {
                        Console.WriteLine($"SPS 长度无效或越界：长度={spsLen}，当前偏移={currentOffset}");
                        return (spsList, ppsList);
                    }

                    // 提取 SPS 数据
                    byte[] sps = new byte[spsLen];
                    Buffer.BlockCopy(avccData, currentOffset, sps, 0, spsLen);
                    spsList.Add(sps);

                    // 移动偏移到下一个字段
                    currentOffset += spsLen;
                }

                // 5. 解析 PPS 数量
                if (currentOffset >= avccData.Length)
                {
                    Console.WriteLine("解析 PPS 数量时偏移越界，终止解析");
                    return (spsList, ppsList);
                }

                int ppsCount = avccData[currentOffset++];
                if (ppsCount <= 0)
                {
                    Console.WriteLine("PPS 数量为0，解析结束");
                    return (spsList, ppsList);
                }

                // 6. 解析所有 PPS
                for (int i = 0; i < ppsCount; i++)
                {
                    // 检查偏移是否越界（PPS 长度占2字节）
                    if (currentOffset + 2 > avccData.Length)
                    {
                        Console.WriteLine("解析 PPS 时偏移越界，终止解析");
                        return (spsList, ppsList);
                    }

                    // 读取 PPS 长度（大端序）
                    int ppsLen = (avccData[currentOffset] << 8) | avccData[currentOffset + 1];
                    currentOffset += 2;

                    // 检查 PPS 数据是否越界
                    if (ppsLen <= 0 || currentOffset + ppsLen > avccData.Length)
                    {
                        Console.WriteLine($"PPS 长度无效或越界：长度={ppsLen}，当前偏移={currentOffset}");
                        return (spsList, ppsList);
                    }

                    // 提取 PPS 数据
                    byte[] pps = new byte[ppsLen];
                    Buffer.BlockCopy(avccData, currentOffset, pps, 0, ppsLen);
                    ppsList.Add(pps);

                    // 移动偏移到下一个字段
                    currentOffset += ppsLen;
                }

                Console.WriteLine($"解析 SPS/PPS 成功：SPS 数量={spsList.Count}，PPS 数量={ppsList.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"解析 SPS/PPS 时发生异常：{ex.Message}\n{ex.StackTrace}");
                // 异常时清空列表，避免返回不完整数据
                spsList.Clear();
                ppsList.Clear();
            }

            return (spsList, ppsList);
        }

        /// <summary>
        /// 简化版：提取第一个 SPS 和第一个 PPS（兼容单 SPS/PPS 场景）
        /// </summary>
        /// <param name="videoCodecCtx">视频解码器上下文</param>
        /// <returns>(第一个 SPS, 第一个 PPS)，若不存在则返回 null</returns>
        public static unsafe (byte[] sps, byte[] pps) ExtractFirstSpsPps(AVCodecContext* videoCodecCtx)
        {
            var (spsList, ppsList) = ExtractSpsPpsListFromStream(videoCodecCtx);
            byte[] sps = spsList.Count > 0 ? spsList[0] : null;
            byte[] pps = ppsList.Count > 0 ? ppsList[0] : null;
            return (sps, pps);
        }


        /// <summary>
        /// 为关键帧补充 SPS/PPS
        /// </summary>
        /// <param name="h264Data"></param>
        /// <param name="sps"></param>
        /// <param name="pps"></param>
        /// <returns></returns>
        public static byte[] AddSpsPpsToKeyFrame(byte[] h264Data, byte[] sps, byte[] pps)
        {
            if (h264Data == null || sps == null || pps == null)
                return h264Data;

            // AVCC格式：SPS（4字节长度+数据） + PPS（4字节长度+数据） + 关键帧数据
            using (MemoryStream ms = new MemoryStream())
            {
                // 写入SPS
                byte[] spsLength = BitConverter.GetBytes(sps.Length);
                if (BitConverter.IsLittleEndian) Array.Reverse(spsLength);
                ms.Write(spsLength, 0, 4);
                ms.Write(sps, 0, sps.Length);

                // 写入PPS
                byte[] ppsLength = BitConverter.GetBytes(pps.Length);
                if (BitConverter.IsLittleEndian) Array.Reverse(ppsLength);
                ms.Write(ppsLength, 0, 4);
                ms.Write(pps, 0, pps.Length);

                // 写入关键帧数据
                ms.Write(h264Data, 0, h264Data.Length);

                return ms.ToArray();
            }
        }


    }
}

using FFmpeg.AutoGen;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace FixVideoChannel
{
    // 推流任务类型
    public enum PushTaskType
    {
        H264Frame,
        AACFrame,
        Disconnect
    }

    // 推流任务数据
    public class PushTask
    {
        public PushTaskType Type { get; set; }
        public byte[] Data { get; set; }
        public long TimestampMs { get; set; }
        public bool IsKeyFrame { get; set; }
        public ZLMediaKitPusher Pusher { get; set; }
    }

    // 视频编码参数
    public class VideoCodecParams
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public AVRational TimeBase { get; set; }
        public long BitRate { get; set; }
        public AVPixelFormat PixelFormat { get; set; } = AVPixelFormat.AV_PIX_FMT_YUV420P;
    }

    // 音频编码参数
    public class AudioCodecParams
    {
        public int SampleRate { get; set; }
        public long BitRate { get; set; }
        public AVRational TimeBase { get; set; }
        public AVSampleFormat SampleFormat { get; set; } = AVSampleFormat.AV_SAMPLE_FMT_FLTP;
        public ulong ChannelLayout { get; set; } = ffmpeg.AV_CH_LAYOUT_STEREO;
        public int Channels { get; set; } = 2;
    }

    public class ZLMediaKitPusher : IDisposable
    {
        #region 私有字段
        private IntPtr _fmtCtxPtr = IntPtr.Zero;
        private IntPtr _videoStreamPtr = IntPtr.Zero;
        private IntPtr _audioStreamPtr = IntPtr.Zero;
        private IntPtr _packetPtr = IntPtr.Zero;

        private int _videoStreamIndex = -1;
        private int _audioStreamIndex = -1;
        private long _videoPts = 0;
        private long _audioPts = 0;

        // 从外部传入的编码参数
        private VideoCodecParams _videoParams;
        private AudioCodecParams _audioParams;

        private bool _isConnected;

        #endregion

        #region 公共属性
        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnected => _isConnected;
        #endregion

        public ZLMediaKitPusher() { }

        /// <summary>
        /// 初始化编码参数（必须在Connect前调用）
        /// </summary>
        /// <param name="videoParams">视频编码参数</param>
        /// <param name="audioParams">音频编码参数</param>
        public void InitializeCodecParams(VideoCodecParams videoParams, AudioCodecParams audioParams = null)
        {
            // 初始化FFmpeg数据包
            unsafe
            {
                _packetPtr = (IntPtr)ffmpeg.av_packet_alloc();
                if (_packetPtr == IntPtr.Zero)
                {
                    throw new OutOfMemoryException("无法分配AVPacket");
                }
            }
            _videoParams = videoParams ?? throw new ArgumentNullException(nameof(videoParams));
            _audioParams = audioParams;
        }

        /// <summary>
        /// 连接ZLMediaKit并初始化推流上下文
        /// </summary>
        /// <param name="pushUrl">推流地址 (rtmp://或rtsp://)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void Connect(string pushUrl)
        {
            if (string.IsNullOrEmpty(pushUrl))
                throw new ArgumentNullException(nameof(pushUrl));

            if (_videoParams == null)
                throw new InvalidOperationException("必须先调用InitializeCodecParams初始化编码参数");

            if (_isConnected)
                return;

            try
            {
                // 初始化FFmpeg推流上下文
                unsafe
                {
                    // 1. 分配输出格式上下文
                    AVFormatContext* fmtCtx = null;
                    int errorCode = ffmpeg.avformat_alloc_output_context2(&fmtCtx, null, null, pushUrl);
                    if (errorCode < 0 || fmtCtx == null)
                    {
                        throw new InvalidOperationException($"无法分配输出格式上下文: {GetFFmpegErrorDescription(errorCode)}");
                    }
                    _fmtCtxPtr = (IntPtr)fmtCtx;

                    // 2. 打开输出IO
                    if ((fmtCtx->oformat->flags & ffmpeg.AVFMT_NOFILE) == 0)
                    {
                        errorCode = ffmpeg.avio_open(&fmtCtx->pb, pushUrl, ffmpeg.AVIO_FLAG_WRITE);
                        if (errorCode < 0)
                        {
                            ffmpeg.avformat_free_context(fmtCtx);
                            _fmtCtxPtr = IntPtr.Zero;
                            throw new InvalidOperationException($"无法打开推流IO: {GetFFmpegErrorDescription(errorCode)}");
                        }
                    }
                }

                // 标记为已连接
                _isConnected = true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 推送H264帧到ZLMediaKit
        /// </summary>
        /// <param name="frameData">H264裸数据 (包含NALU头)</param>
        /// <param name="timestampMs">时间戳（毫秒）</param>
        /// <param name="isKeyFrame">是否为关键帧</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void PushH264Frame(byte[] frameData, long timestampMs, bool isKeyFrame)
        {
            if (!_isConnected)
                throw new InvalidOperationException("未连接到ZLMediaKit");

            if (frameData == null || frameData.Length == 0)
                throw new ArgumentNullException(nameof(frameData));

            // 创建任务并加入队列
            var task = new PushTask
            {
                Type = PushTaskType.H264Frame,
                Data = frameData,
                TimestampMs = timestampMs,
                IsKeyFrame = isKeyFrame,
                Pusher = this
            };

            StreamTaskScheduler.Instance.EnqueuePushTask(task);
        }

        /// <summary>
        /// 推送AAC帧到ZLMediaKit
        /// </summary>
        /// <param name="frameData">AAC裸数据 (ADTS头可选)</param>
        /// <param name="timestampMs">时间戳（毫秒）</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void PushAacFrame(byte[] frameData, long timestampMs)
        {
            if (!_isConnected)
                throw new InvalidOperationException("未连接到ZLMediaKit");

            if (frameData == null || frameData.Length == 0)
                throw new ArgumentNullException(nameof(frameData));

            if (_audioParams == null)
                throw new InvalidOperationException("未初始化音频编码参数");

            // 创建任务并加入队列
            var task = new PushTask
            {
                Type = PushTaskType.AACFrame,
                Data = frameData,
                TimestampMs = timestampMs,
                Pusher = this
            };

            StreamTaskScheduler.Instance.EnqueuePushTask(task);
        }

        /// <summary>
        /// 断开与ZLMediaKit的连接
        /// </summary>
        /// <returns></returns>
        public void Disconnect()
        {
            if (!_isConnected)
                return;

            // 清理资源
            _isConnected = false;
        }




        /// <summary>
        /// 处理H264帧推送（单线程执行）
        /// </summary>
        /// <param name="task">推流任务</param>
        public unsafe void ProcessH264Frame(PushTask task)
        {
            AVFormatContext* fmtCtx = (AVFormatContext*)_fmtCtxPtr;
            AVPacket* packet = (AVPacket*)_packetPtr;

            // 1. 首次推送时创建视频流
            if (_videoStreamIndex == -1)
            {
                CreateVideoStream(fmtCtx);
            }

            // 2. 初始化数据包
            ffmpeg.av_packet_unref(packet);
            fixed (byte* dataPtr = task.Data)
            {
                packet->data = dataPtr;
                packet->size = task.Data.Length;
            }

            // 3. 设置时间戳
            _videoPts = task.TimestampMs * _videoParams.TimeBase.den / _videoParams.TimeBase.num;
            packet->pts = _videoPts;
            packet->dts = _videoPts;
            packet->stream_index = _videoStreamIndex;

            // 4. 设置关键帧标记
            if (task.IsKeyFrame)
            {
                packet->flags |= ffmpeg.AV_PKT_FLAG_KEY;
            }

            // 5. 时间基转换
            ffmpeg.av_packet_rescale_ts(packet, _videoParams.TimeBase, ((AVStream*)_videoStreamPtr)->time_base);

            // 6. 写入数据包
            int errorCode = ffmpeg.av_interleaved_write_frame(fmtCtx, packet);
            if (errorCode < 0)
            {
                throw new InvalidOperationException($"推送H264帧失败: {GetFFmpegErrorDescription(errorCode)}");
            }
        }

        /// <summary>
        /// 处理AAC帧推送（单线程执行）
        /// </summary>
        /// <param name="task">推流任务</param>
        public unsafe void ProcessAacFrame(PushTask task)
        {
            AVFormatContext* fmtCtx = (AVFormatContext*)_fmtCtxPtr;
            AVPacket* packet = (AVPacket*)_packetPtr;

            // 1. 首次推送时创建音频流
            if (_audioStreamIndex == -1)
            {
                CreateAudioStream(fmtCtx);
            }

            // 2. 初始化数据包
            ffmpeg.av_packet_unref(packet);
            fixed (byte* dataPtr = task.Data)
            {
                packet->data = dataPtr;
                packet->size = task.Data.Length;
            }

            // 3. 设置时间戳
            _audioPts = task.TimestampMs * _audioParams.TimeBase.den / _audioParams.TimeBase.num;
            packet->pts = _audioPts;
            packet->dts = _audioPts;
            packet->stream_index = _audioStreamIndex;

            // 4. 时间基转换
            ffmpeg.av_packet_rescale_ts(packet, _audioParams.TimeBase, ((AVStream*)_audioStreamPtr)->time_base);

            // 5. 写入数据包
            int errorCode = ffmpeg.av_interleaved_write_frame(fmtCtx, packet);
            if (errorCode < 0)
            {
                throw new InvalidOperationException($"推送AAC帧失败: {GetFFmpegErrorDescription(errorCode)}");
            }
        }

        /// <summary>
        /// 处理断开连接（单线程执行）
        /// </summary>
        public unsafe void ProcessDisconnect()
        {
            AVFormatContext* fmtCtx = (AVFormatContext*)_fmtCtxPtr;
            if (fmtCtx != null)
            {
                // 写入文件尾
                ffmpeg.av_write_trailer(fmtCtx);

                // 关闭IO
                if ((fmtCtx->oformat->flags & ffmpeg.AVFMT_NOFILE) == 0 && fmtCtx->pb != null)
                {
                    ffmpeg.avio_closep(&fmtCtx->pb);
                }

                // 释放格式上下文
                ffmpeg.avformat_free_context(fmtCtx);
                _fmtCtxPtr = IntPtr.Zero;
            }

            // 重置流索引
            _videoStreamIndex = -1;
            _audioStreamIndex = -1;
            _videoStreamPtr = IntPtr.Zero;
            _audioStreamPtr = IntPtr.Zero;
        }

        /// <summary>
        /// 创建视频流并初始化编码参数（使用外部传入的参数）
        /// </summary>
        /// <param name="fmtCtx">格式上下文</param>
        private unsafe void CreateVideoStream(AVFormatContext* fmtCtx)
        {
            // 1. 创建视频流
            AVStream* videoStream = ffmpeg.avformat_new_stream(fmtCtx, null);
            if (videoStream == null)
            {
                throw new InvalidOperationException("无法创建视频流");
            }
            _videoStreamPtr = (IntPtr)videoStream;
            _videoStreamIndex = videoStream->index;

            // 2. 设置H264编码参数（从外部传入）
            AVCodecParameters* codecPar = videoStream->codecpar;
            codecPar->codec_id = AVCodecID.AV_CODEC_ID_H264;
            codecPar->codec_type = AVMediaType.AVMEDIA_TYPE_VIDEO;
            codecPar->format = (int)_videoParams.PixelFormat;
            codecPar->width = _videoParams.Width;
            codecPar->height = _videoParams.Height;
            codecPar->bit_rate = _videoParams.BitRate;

            // 3. 设置时间基
            videoStream->time_base = _videoParams.TimeBase;
            videoStream->avg_frame_rate = ffmpeg.av_inv_q(_videoParams.TimeBase);

            // 4. 写入流头
            if (ffmpeg.avformat_write_header(fmtCtx, null) < 0)
            {
                throw new InvalidOperationException("无法写入流头");
            }
        }

        /// <summary>
        /// 创建音频流并初始化编码参数（使用外部传入的参数）
        /// </summary>
        /// <param name="fmtCtx">格式上下文</param>
        private unsafe void CreateAudioStream(AVFormatContext* fmtCtx)
        {
            // 1. 创建音频流
            AVStream* audioStream = ffmpeg.avformat_new_stream(fmtCtx, null);
            if (audioStream == null)
            {
                throw new InvalidOperationException("无法创建音频流");
            }
            _audioStreamPtr = (IntPtr)audioStream;
            _audioStreamIndex = audioStream->index;

            // 2. 设置AAC编码参数（从外部传入）
            AVCodecParameters* codecPar = audioStream->codecpar;
            codecPar->codec_id = AVCodecID.AV_CODEC_ID_AAC;
            codecPar->codec_type = AVMediaType.AVMEDIA_TYPE_AUDIO;
            codecPar->format = (int)_audioParams.SampleFormat;

            int ret = ffmpeg.av_channel_layout_from_mask(&codecPar->ch_layout, _audioParams.ChannelLayout);
            if (ret < 0)
            {
                throw new InvalidOperationException($"初始化声道布局失败: {GetFFmpegErrorDescription(ret)}");
            }

            codecPar->sample_rate = _audioParams.SampleRate;
            codecPar->bit_rate = _audioParams.BitRate;
            codecPar->ch_layout.nb_channels = _audioParams.Channels;

            // 3. 设置时间基
            audioStream->time_base = _audioParams.TimeBase;

            // 若视频流已创建，无需重复写入头
            if (_videoStreamIndex == -1)
            {
                if (ffmpeg.avformat_write_header(fmtCtx, null) < 0)
                {
                    throw new InvalidOperationException("无法写入流头");
                }
            }
        }

        /// <summary>
        /// 获取FFmpeg错误描述
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <returns></returns>
        private static string GetFFmpegErrorDescription(int errorCode)
        {
            unsafe
            {
                Span<byte> buffer = stackalloc byte[1024];
                byte* bufferPtr = (byte*)Unsafe.AsPointer(ref buffer.GetPinnableReference());
                ffmpeg.av_strerror(errorCode, bufferPtr, (ulong)buffer.Length);
                return System.Text.Encoding.ASCII.GetString(buffer).TrimEnd('\0');
            }
        }

        #region 资源释放
        public void CleanupFFmpegResources()
        {
            unsafe
            {
                if (_fmtCtxPtr != IntPtr.Zero)
                {
                    AVFormatContext* fmtCtx = (AVFormatContext*)_fmtCtxPtr;
                    ffmpeg.avformat_close_input(&fmtCtx);
                    _fmtCtxPtr = IntPtr.Zero;
                }

                // 释放数据包
                if (_packetPtr != IntPtr.Zero)
                {
                    AVPacket* pkt = (AVPacket*)_packetPtr;
                    ffmpeg.av_packet_free(&pkt);
                    _packetPtr = IntPtr.Zero;
                }

            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 释放托管资源
                Disconnect();
            }

            // 释放非托管资源
            this.CleanupFFmpegResources();
        }

        ~ZLMediaKitPusher()
        {
            Dispose(false);
        }
        #endregion
    }
}
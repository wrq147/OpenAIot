using FFmpeg.AutoGen;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
        public byte[] sps { get; set; }
        public byte[] pps { get; set; }
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
        private long _lastVideoPts = 0;
        private long _videoFrameDuration = 3000; // 30fps默认帧间隔（90000/30）

        private VideoCodecParams _videoParams;
        private AudioCodecParams _audioParams;

        private bool _isConnected;
        private const int AV_INPUT_BUFFER_PADDING_SIZE = 8;

        #endregion

        #region 公共属性
        public bool IsConnected => _isConnected;
        #endregion

        private string _videoId;
        public string VideoId => _videoId;

        private enum PushProtocol
        {
            RTMP,
            RTSP,
            Unknown
        }

        private PushProtocol _pushProtocol;

        public ZLMediaKitPusher(string vid)
        {
            _videoId = vid;
        }



        public void InitializeCodecParams(VideoCodecParams videoParams, AudioCodecParams audioParams = null)
        {
            unsafe
            {
                if (_packetPtr != IntPtr.Zero)
                {
                    AVPacket* pkt = (AVPacket*)_packetPtr;
                    ffmpeg.av_packet_free(&pkt);
                }

                _packetPtr = (IntPtr)ffmpeg.av_packet_alloc();
                if (_packetPtr == IntPtr.Zero)
                    throw new OutOfMemoryException("无法分配AVPacket");
            }

            _videoParams = videoParams ?? throw new ArgumentNullException(nameof(videoParams));
            _audioParams = audioParams;

            // 修复：计算视频帧间隔时增加非零校验
            if (_videoParams.TimeBase.num > 0 && _videoParams.TimeBase.den > 0)
            {
                double fps = (double)_videoParams.TimeBase.den / _videoParams.TimeBase.num;
                fps = Math.Clamp(fps, 1, 60); // 限制帧率范围
                _videoFrameDuration = (long)(90000 / fps);
            }
            else
            {
                _videoFrameDuration = 3000; // 兜底30fps
            }

            // 音频参数非零校验
            if (_audioParams != null)
            {
                _audioParams.SampleRate = _audioParams.SampleRate <= 0 ? 44100 : _audioParams.SampleRate;
                if (_audioParams.TimeBase.num <= 0 || _audioParams.TimeBase.den <= 0)
                {
                    _audioParams.TimeBase = new AVRational { num = 1, den = _audioParams.SampleRate };
                }
            }
        }

        public unsafe void Connect(string pushUrl)
        {
            if (string.IsNullOrEmpty(pushUrl))
                throw new ArgumentNullException(nameof(pushUrl));
            if (_videoParams == null)
                throw new InvalidOperationException("必须先初始化编码参数");
            if (_isConnected)
                return;

            string formatName = null;
            AVDictionary* options = null;
            _pushProtocol = PushProtocol.Unknown;

            try
            {
                // 1. 识别协议并设置格式名（让 FFmpeg 自动处理 AVFMT_NOFILE）
                if (pushUrl.StartsWith("rtmp://", StringComparison.OrdinalIgnoreCase))
                {
                    _pushProtocol = PushProtocol.RTMP;
                    formatName = "flv";
                    ffmpeg.av_dict_set(&options, "flvflags", "no_duration_filesize", 0);
                    ffmpeg.av_dict_set(&options, "rtmp_live", "1", 0);
                }
                else if (pushUrl.StartsWith("rtsp://", StringComparison.OrdinalIgnoreCase))
                {
                    _pushProtocol = PushProtocol.RTSP;
                    formatName = "rtsp";
                    ffmpeg.av_dict_set(&options, "rtsp_transport", "tcp", 0);
                    ffmpeg.av_dict_set(&options, "stimeout", "5000000", 0);
                }
                else
                {
                    throw new NotSupportedException($"不支持的推流协议：{pushUrl}");
                }

                // 2. 分配格式上下文（FFmpeg 自动设置 oformat 标志）
                AVFormatContext* fmtCtx = null;
                int ret = ffmpeg.avformat_alloc_output_context2(&fmtCtx, null, formatName, pushUrl);
                if (ret < 0 || fmtCtx == null)
                {
                    throw new InvalidOperationException($"分配输出格式上下文失败: {GetFFmpegErrorDescription(ret)}");
                }
                _fmtCtxPtr = (IntPtr)fmtCtx;

                // 3. 打开 IO 上下文（FFmpeg 自动判断是否需要文件 IO）
                // 关键：仅当格式需要文件 IO 时才打开（由 FFmpeg 内部判断，无需手动干预）
                if ((fmtCtx->oformat->flags & ffmpeg.AVFMT_NOFILE) == 0)
                {
                    ret = ffmpeg.avio_open2(&fmtCtx->pb, pushUrl, ffmpeg.AVIO_FLAG_WRITE, null, &options);
                    if (ret < 0)
                    {
                        ffmpeg.avformat_free_context(fmtCtx);
                        _fmtCtxPtr = IntPtr.Zero;
                        throw new InvalidOperationException($"无法打开推流IO: {GetFFmpegErrorDescription(ret)}");
                    }
                }

                // 4. 安全设置格式上下文参数
                fmtCtx->max_delay = 500000; // 500ms 延迟
                if (fmtCtx->pb != null)
                {
                    fmtCtx->pb->seekable = 0; // 直播流禁用 seek（仅当 pb 非空时操作）
                }

                ffmpeg.av_dict_free(&options);
                _isConnected = true;
            }
            catch
            {
                ffmpeg.av_dict_free(&options); // 确保字典释放
                Dispose();
                throw;
            }
        }

        public void PushH264Frame(byte[] frameData, long timestampMs, bool isKeyFrame)
        {
            if (!_isConnected)
                throw new InvalidOperationException("未连接到ZLMediaKit");

            if (frameData == null || frameData.Length == 0)
                throw new ArgumentNullException(nameof(frameData));


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

        public void PushAacFrame(byte[] frameData, long timestampMs)
        {
            if (!_isConnected)
                throw new InvalidOperationException("未连接到ZLMediaKit");

            if (frameData == null || frameData.Length == 0)
                throw new ArgumentNullException(nameof(frameData));

            if (_audioParams == null)
                throw new InvalidOperationException("未初始化音频编码参数");

            var task = new PushTask
            {
                Type = PushTaskType.AACFrame,
                Data = frameData,
                TimestampMs = timestampMs,
                Pusher = this
            };

            StreamTaskScheduler.Instance.EnqueuePushTask(task);
        }

        public void Disconnect()
        {
            if (!_isConnected)
                return;

            _isConnected = false;
            unsafe
            {
                ProcessDisconnect();
            }
        }

        public unsafe void ProcessH264Frame(PushTask task)
        {
            if (!_isConnected)
                throw new InvalidOperationException("未连接到ZLMediaKit");

            AVFormatContext* fmtCtx = (AVFormatContext*)_fmtCtxPtr;
            AVPacket* packet = (AVPacket*)_packetPtr;

            if (_videoStreamIndex == -1)
                CreateVideoStream(fmtCtx);

            // 初始化数据包
            ffmpeg.av_packet_unref(packet);
            fixed (byte* dataPtr = task.Data)
            {
                packet->data = dataPtr;
                packet->size = task.Data.Length;
            }

            // 计算PTS
            AVStream* videoStream = (AVStream*)_videoStreamPtr;
            if (_pushProtocol == PushProtocol.RTMP)
            {
                _videoPts = task.TimestampMs * 90;
            }
            else if (_pushProtocol == PushProtocol.RTSP)
            {
                double fps = videoStream->avg_frame_rate.num > 0 && videoStream->avg_frame_rate.den > 0
                    ? (double)videoStream->avg_frame_rate.den / videoStream->avg_frame_rate.num
                    : 30;
                _videoPts = (long)(task.TimestampMs * fps / 1000);
            }

            // 避免PTS回退
            if (_videoPts <= _lastVideoPts)
                _videoPts = _lastVideoPts + _videoFrameDuration;
            _lastVideoPts = _videoPts;

            packet->pts = _videoPts;
            packet->dts = task.IsKeyFrame ? _videoPts : packet->pts;
            packet->stream_index = _videoStreamIndex;

            if (task.IsKeyFrame)
                packet->flags |= ffmpeg.AV_PKT_FLAG_KEY;

            // 时间基转换（安全校验）
            AVRational srcTimeBase = new AVRational { num = 1, den = 1000 };
            AVRational dstTimeBase = videoStream->time_base;
            if (dstTimeBase.num <= 0 || dstTimeBase.den <= 0)
            {
                dstTimeBase = new AVRational { num = 1, den = 90000 };
            }
            ffmpeg.av_packet_rescale_ts(packet, srcTimeBase, dstTimeBase);

            // 写入数据包
            int errorCode = ffmpeg.av_interleaved_write_frame(fmtCtx, packet);
            if (errorCode < 0)
                throw new InvalidOperationException($"推送H264帧失败: {GetFFmpegErrorDescription(errorCode)}");
        }

        public unsafe void ProcessAacFrame(PushTask task)
        {
            if (!_isConnected)
                throw new InvalidOperationException("未连接到ZLMediaKit");

            AVFormatContext* fmtCtx = (AVFormatContext*)_fmtCtxPtr;
            AVPacket* packet = (AVPacket*)_packetPtr;

            if (_audioStreamIndex == -1)
                CreateAudioStream(fmtCtx);

            // 初始化数据包
            ffmpeg.av_packet_unref(packet);
            fixed (byte* dataPtr = task.Data)
            {
                packet->data = dataPtr;
                packet->size = task.Data.Length;
            }

            // 获取音频流并验证时间基
            AVStream* audioStream = (AVStream*)_audioStreamPtr;

            // 验证并修复音频流时间基
            AVRational audioTimeBase = audioStream->time_base;
            if (audioTimeBase.num <= 0 || audioTimeBase.den <= 0)
            {
                audioTimeBase = new AVRational { num = 1, den = _audioParams?.SampleRate > 0 ? _audioParams.SampleRate : 44100 };
                audioStream->time_base = audioTimeBase;
            }

            // 使用毫秒时间戳计算音频PTS
            int sampleRate = _audioParams?.SampleRate > 0 ? _audioParams.SampleRate : 44100;

            // 确保采样率有效
            if (sampleRate <= 0)
            {
                sampleRate = 44100;
                Console.WriteLine("警告：采样率无效，已设置为默认值44100Hz");
            }

            // 计算PTS：毫秒转采样点数
            _audioPts = task.TimestampMs * sampleRate / 1000;

            // 避免PTS回退
            if (_audioPts <= 0)
            {
                _audioPts = sampleRate / 10; // 给一个合理的初始值（100ms）
            }

            packet->pts = _audioPts;
            packet->dts = _audioPts;
            packet->stream_index = _audioStreamIndex;

            AVRational srcTimeBase = new AVRational { num = 1, den = sampleRate };
            // 目标时间基：音频流的时间基
            AVRational dstTimeBase = audioStream->time_base;

            // 校验时间基有效性
            if (dstTimeBase.num <= 0 || dstTimeBase.den <= 0)
            {
                dstTimeBase = new AVRational { num = 1, den = 90000 }; // 兜底为90000（RTMP标准）
                audioStream->time_base = dstTimeBase;
            }

            // 使用FFmpeg原生方法进行时间基转换（高精度、安全）
            if (packet->pts != ffmpeg.AV_NOPTS_VALUE)
            {
                ffmpeg.av_packet_rescale_ts(packet, srcTimeBase, dstTimeBase);
            }

            int frameSize = 1024; // AAC 标准帧大小
            long duration = frameSize; // 采样点数
            packet->duration = duration;
            // 时间基转换时，同时转换 duration
            if (packet->duration != ffmpeg.AV_NOPTS_VALUE)
            {
                packet->duration = ffmpeg.av_rescale_q(packet->duration, srcTimeBase, dstTimeBase);
            }

            // 写入数据包
            int errorCode = ffmpeg.av_interleaved_write_frame(fmtCtx, packet);
            if (errorCode < 0)
            {
                ffmpeg.av_packet_unref(packet);
                string errorMsg = $"推送AAC帧失败: {GetFFmpegErrorDescription(errorCode)}, " +
                           $"采样率: {sampleRate}, PTS: {_audioPts}, " +
                           $"时间基: {audioStream->time_base.num}/{audioStream->time_base.den}, " +
                           $"流索引: {_audioStreamIndex}";
                throw new InvalidOperationException(errorMsg);
            }
        }

        public unsafe void ProcessDisconnect()
        {
            AVFormatContext* fmtCtx = (AVFormatContext*)_fmtCtxPtr;
            if (fmtCtx != null)
            {
                try
                {
                    if (_isConnected)
                        ffmpeg.av_write_trailer(fmtCtx);
                }
                catch { }

                if ((fmtCtx->oformat->flags & ffmpeg.AVFMT_NOFILE) == 0 && fmtCtx->pb != null)
                {
                    ffmpeg.avio_closep(&fmtCtx->pb);
                }

                ffmpeg.avformat_free_context(fmtCtx);
                _fmtCtxPtr = IntPtr.Zero;
            }

            // 重置状态
            _videoStreamIndex = -1;
            _audioStreamIndex = -1;
            _videoStreamPtr = IntPtr.Zero;
            _audioStreamPtr = IntPtr.Zero;
            _lastVideoPts = 0;
        }
        private bool _headerWritten = false;
        private unsafe void CreateVideoStream(AVFormatContext* fmtCtx)
        {
            AVStream* videoStream = ffmpeg.avformat_new_stream(fmtCtx, null);
            if (videoStream == null)
                throw new InvalidOperationException("无法创建视频流");

            _videoStreamPtr = (IntPtr)videoStream;
            _videoStreamIndex = videoStream->index;

            // 设置编码参数
            AVCodecParameters* codecPar = videoStream->codecpar;
            codecPar->codec_id = AVCodecID.AV_CODEC_ID_H264;
            codecPar->codec_type = AVMediaType.AVMEDIA_TYPE_VIDEO;
            codecPar->format = (int)_videoParams.PixelFormat;
            codecPar->width = _videoParams.Width;
            codecPar->height = _videoParams.Height;
            codecPar->bit_rate = _videoParams.BitRate;

            // 封装SPS/PPS到extradata
            if (_pushProtocol == PushProtocol.RTMP && _videoParams.sps != null && _videoParams.pps != null)
            {
                byte[] pureSps = H264Utils.RemoveAnnexBStartCode(_videoParams.sps);
                byte[] purePps = H264Utils.RemoveAnnexBStartCode(_videoParams.pps);

                if (pureSps.Length == 0 || purePps.Length == 0)
                    throw new InvalidOperationException("SPS/PPS数据为空（需去掉Annex-B起始码）");

                int extradataTotalSize = 6 + 2 + pureSps.Length + 1 + 2 + purePps.Length;
                byte* extradata = (byte*)ffmpeg.av_mallocz((ulong)(extradataTotalSize + ffmpeg.AV_INPUT_BUFFER_PADDING_SIZE));
                if (extradata == null)
                    throw new OutOfMemoryException("无法分配extradata内存");

                try
                {
                    int offset = 0;
                    extradata[offset++] = 0x01;
                    extradata[offset++] = pureSps[1];
                    extradata[offset++] = pureSps[2];
                    extradata[offset++] = pureSps[3];
                    extradata[offset++] = (byte)(0xFC | 0x03);
                    extradata[offset++] = (byte)(0xE0 | 0x01);

                    extradata[offset++] = (byte)(pureSps.Length >> 8);
                    extradata[offset++] = (byte)(pureSps.Length & 0xFF);
                    Marshal.Copy(pureSps, 0, (IntPtr)(extradata + offset), pureSps.Length);
                    offset += pureSps.Length;

                    extradata[offset++] = 0x01;
                    extradata[offset++] = (byte)(purePps.Length >> 8);
                    extradata[offset++] = (byte)(purePps.Length & 0xFF);
                    Marshal.Copy(purePps, 0, (IntPtr)(extradata + offset), purePps.Length);

                    codecPar->extradata = extradata;
                    codecPar->extradata_size = extradataTotalSize;
                }
                catch
                {
                    ffmpeg.av_free(extradata);
                    throw;
                }
            }

            // 设置时间基（安全赋值）
            if (_pushProtocol == PushProtocol.RTMP)
            {
                videoStream->time_base = new AVRational { num = 1, den = 90000 };
                videoStream->avg_frame_rate = new AVRational { num = 30, den = 1 };
                videoStream->r_frame_rate = videoStream->avg_frame_rate;
                _videoFrameDuration = 3000;
            }
            else if (_pushProtocol == PushProtocol.RTSP)
            {
                AVRational videoTimeBase = new AVRational { num = 1, den = 90000 }; // 标准RTSP视频时间基
                if (_videoParams.TimeBase.num > 0 && _videoParams.TimeBase.den > 0)
                {
                    videoTimeBase = _videoParams.TimeBase;
                }
                videoStream->time_base = videoTimeBase;
                // 帧率设置（avg_frame_rate 是帧率，如30/1）
                AVRational videoFps = new AVRational { num = 30, den = 1 };
                if (_videoParams.TimeBase.num > 0 && _videoParams.TimeBase.den > 0)
                {
                    videoFps = new AVRational { num = _videoParams.TimeBase.den, den = _videoParams.TimeBase.num };
                }
                videoStream->avg_frame_rate = videoFps;
                videoStream->r_frame_rate = videoFps;
                // 计算帧间隔（90000/帧率）
                _videoFrameDuration = (long)(videoTimeBase.den / (double)videoFps.den * videoFps.num);
            }

            // 写入流头
            if (!_headerWritten)
            {
                WriteStreamHeader(fmtCtx);
                _headerWritten = true;
            }
        }

        private unsafe void CreateAudioStream(AVFormatContext* fmtCtx)
        {
            AVStream* audioStream = ffmpeg.avformat_new_stream(fmtCtx, null);
            if (audioStream == null)
                throw new InvalidOperationException("无法创建音频流");

            _audioStreamPtr = (IntPtr)audioStream;
            _audioStreamIndex = audioStream->index;

            // 设置编码参数
            AVCodecParameters* codecPar = audioStream->codecpar;
            codecPar->codec_id = AVCodecID.AV_CODEC_ID_AAC;
            codecPar->codec_type = AVMediaType.AVMEDIA_TYPE_AUDIO;
            codecPar->format = (int)_audioParams.SampleFormat;

            // 初始化声道布局（优先使用新的 API）
            if (_audioParams.ChannelLayout != 0)
            {
                int ret = ffmpeg.av_channel_layout_from_mask(&codecPar->ch_layout, _audioParams.ChannelLayout);
                if (ret < 0)
                {
                    // 失败时使用默认布局
                    ffmpeg.av_channel_layout_default(&codecPar->ch_layout, _audioParams.Channels);
                    Console.WriteLine($"警告：从mask初始化声道布局失败，已回退到默认: {GetFFmpegErrorDescription(ret)}");
                }
            }
            else
            {
                ffmpeg.av_channel_layout_default(&codecPar->ch_layout, _audioParams.Channels);
            }
            if (codecPar->ch_layout.nb_channels <= 0)
            {
                codecPar->ch_layout.nb_channels = _audioParams.Channels;
                Console.WriteLine($"警告：声道数无效，已重置为: {_audioParams.Channels}");
            }

            int sampleRate = _audioParams.SampleRate > 0 ? _audioParams.SampleRate : 44100;
            if (sampleRate <= 0)
            {
                sampleRate = 44100;
            }
            codecPar->sample_rate = sampleRate;
            codecPar->bit_rate = _audioParams.BitRate;
            codecPar->ch_layout.nb_channels = _audioParams.Channels;

            audioStream->avg_frame_rate = new AVRational { num = codecPar->sample_rate, den = 1024 };
            audioStream->r_frame_rate = audioStream->avg_frame_rate;

            if (_pushProtocol == PushProtocol.RTMP)
            {
                // RTMP: 音频通常使用采样率作为时间基
                audioStream->time_base = new AVRational { num = 1, den = sampleRate };
            }
            else if (_pushProtocol == PushProtocol.RTSP)
            {
                // RTSP: 根据音频参数设置时间基
                if (_audioParams.TimeBase.num > 0 && _audioParams.TimeBase.den > 0)
                {
                    audioStream->time_base = _audioParams.TimeBase;
                }
                else
                {
                    audioStream->time_base = new AVRational { num = 1, den = sampleRate };
                }
            }

            // 验证时间基有效性
            if (audioStream->time_base.num <= 0 || audioStream->time_base.den <= 0)
            {
                audioStream->time_base = new AVRational { num = 1, den = sampleRate };
                Console.WriteLine($"警告：音频流时间基无效，已重置为: 1/{sampleRate}");
            }

            // 写入流头
            if (!_headerWritten)
            {
                WriteStreamHeader(fmtCtx);
                _headerWritten = true;
            }
        }
        private unsafe void WriteStreamHeader(AVFormatContext* fmtCtx)
        {
            AVDictionary* options = null;
            int ret = ffmpeg.avformat_write_header(fmtCtx, &options);
            ffmpeg.av_dict_free(&options);
            if (ret < 0)
                throw new InvalidOperationException($"无法写入流头: {GetFFmpegErrorDescription(ret)}");
        }
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
                Disconnect();
            }

            CleanupFFmpegResources();
        }

        ~ZLMediaKitPusher()
        {
            Dispose(false);
        }
        #endregion
    }
}
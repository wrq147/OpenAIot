using FFmpeg.AutoGen;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace FixVideoChannel
{
    public class RtspStreamProcessor : IDisposable
    {
        #region FFmpeg资源
        private IntPtr _inputFormatContextPtr;
        private IntPtr _videoCodecContextPtr;
        private IntPtr _audioCodecContextPtr;
        private IntPtr _videoFramePtr;
        private IntPtr _rgbFramePtr;
        private IntPtr _swsContextPtr;
        private IntPtr _encodeCodecContextPtr;
        private IntPtr _packetPtr;
        #endregion

        #region 依赖组件
        private readonly AIDetector _aiDetector;
        private readonly ZLMediaKitPusher _zlPusher;
        private readonly string _rtspUrl;
        private readonly string _pushUrl;
        #endregion

        #region 状态控制
        private CancellationTokenSource _cts;
        private Task _processingTask;
        private bool _isDisposed;
        private int _videoStreamIndex = -1;
        private int _audioStreamIndex = -1;
        private int _videoWidth;
        private int _videoHeight;
        private AVRational _videoTimeBase; // 视频时间基
        private AVRational _audioTimeBase; // 音频时间基
        #endregion

        private RtspParam _rtspParam;
        // 构造函数
        public RtspStreamProcessor(string rtspUrl, string pushUrl, RtspParam rtspParam)
        {
            _rtspParam = rtspParam;
            _rtspUrl = rtspUrl ?? throw new ArgumentNullException(nameof(rtspUrl));
            _pushUrl = pushUrl ?? throw new ArgumentNullException(nameof(pushUrl));

            // 初始化FFmpeg资源
            unsafe
            {
                _inputFormatContextPtr = (IntPtr)ffmpeg.avformat_alloc_context();
                _packetPtr = (IntPtr)ffmpeg.av_packet_alloc();
                _videoFramePtr = (IntPtr)ffmpeg.av_frame_alloc();
                _rgbFramePtr = (IntPtr)ffmpeg.av_frame_alloc();

                if (_inputFormatContextPtr == IntPtr.Zero || _packetPtr == IntPtr.Zero ||
                    _videoFramePtr == IntPtr.Zero || _rgbFramePtr == IntPtr.Zero)
                {
                    throw new OutOfMemoryException("无法分配FFmpeg资源");
                }
            }

            // 初始化组件
            _aiDetector = new AIDetector();
            _zlPusher = new ZLMediaKitPusher();
        }

        /// <summary>
        /// 启动RTSP流处理
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<bool> StartAsync(CancellationToken cancellationToken)
        {
            if (_processingTask != null && !_processingTask.IsCompleted)
            {
                throw new InvalidOperationException("已在处理流");
            }

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            try
            {
                // 1. 打开RTSP流
                await OpenRtspStreamAsync();

                // 2. 初始化编解码器
                InitializeCodecs();

                // 3. 初始化推流器的编码参数
                InitializePusherCodecParams();

                // 4. 连接ZLMediaKit
                await _zlPusher.ConnectAsync(_pushUrl, cancellationToken);

                // 5. 启动流处理任务
                _processingTask = ProcessStreamAsync(_cts.Token);
                return true;
            }
            catch
            {
                _cts.Cancel();
                Dispose();
                return false;
            }
        }

        /// <summary>
        /// 初始化推流器的编码参数
        /// </summary>
        private unsafe void InitializePusherCodecParams()
        {
            AVFormatContext* inputFormatContext = (AVFormatContext*)_inputFormatContextPtr;
            AVStream* videoStream = inputFormatContext->streams[_videoStreamIndex];
            AVCodecParameters* videoCodecPar = videoStream->codecpar;

            // 构建视频编码参数
            var videoParams = new VideoCodecParams
            {
                Width = _videoWidth,
                Height = _videoHeight,
                TimeBase = videoStream->time_base, // 使用原流的时间基
                BitRate = videoCodecPar->bit_rate > 0 ? videoCodecPar->bit_rate : 1000000,
                PixelFormat = (AVPixelFormat)videoCodecPar->format
            };

            AudioCodecParams audioParams = null;
            if (_audioStreamIndex != -1)
            {
                AVStream* audioStream = inputFormatContext->streams[_audioStreamIndex];
                AVCodecParameters* audioCodecPar = audioStream->codecpar;

                // 构建音频编码参数
                audioParams = new AudioCodecParams
                {
                    SampleRate = audioCodecPar->sample_rate,
                    BitRate = audioCodecPar->bit_rate > 0 ? audioCodecPar->bit_rate : 128000,
                    TimeBase = audioStream->time_base,
                    SampleFormat = (AVSampleFormat)audioCodecPar->format,
                    ChannelLayout = audioCodecPar->ch_layout.u.mask,
                    Channels = audioCodecPar->ch_layout.nb_channels
                };
            }

            // 传递给推流器
            _zlPusher.InitializeCodecParams(videoParams, audioParams);
        }

        /// <summary>
        /// 停止流处理
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync()
        {
            _cts?.Cancel();
            if (_processingTask != null)
            {
                await _processingTask;
            }

            await _zlPusher.DisconnectAsync();
            CleanupFFmpegResources();
        }

        #region FFmpeg核心操作
        /// <summary>
        /// 打开RTSP流
        /// </summary>
        private async Task OpenRtspStreamAsync()
        {
            unsafe
            {
                AVFormatContext* inputFormatContext = (AVFormatContext*)_inputFormatContextPtr;
                AVDictionary* options = null;

                // 设置RTSP选项
                ffmpeg.av_dict_set(&options, "rtsp_transport", "tcp", 0);
                ffmpeg.av_dict_set(&options, "stimeout", "5000000", 0); // 5秒超时

                int errorCode = ffmpeg.avformat_open_input(&inputFormatContext, _rtspUrl, null, &options);
                if (errorCode < 0)
                {
                    // 释放字典
                    ffmpeg.av_dict_free(&options);
                    throw new InvalidOperationException($"无法打开RTSP流: {GetFFmpegErrorDescription(errorCode)}");
                }

                // 释放字典（avformat_open_input后不再需要）
                ffmpeg.av_dict_free(&options);

                // 更新IntPtr
                _inputFormatContextPtr = (IntPtr)inputFormatContext;

                errorCode = ffmpeg.avformat_find_stream_info(inputFormatContext, null);
                if (errorCode < 0)
                {
                    throw new InvalidOperationException($"无法获取流信息: {GetFFmpegErrorDescription(errorCode)}");
                }

                // 查找视频和音频流索引，并保存时间基
                for (int i = 0; i < inputFormatContext->nb_streams; i++)
                {
                    AVStream* stream = inputFormatContext->streams[i];
                    if (stream->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO && _videoStreamIndex == -1)
                    {
                        _videoStreamIndex = i;
                        _videoWidth = stream->codecpar->width;
                        _videoHeight = stream->codecpar->height;
                        _videoTimeBase = stream->time_base; // 保存视频时间基
                    }
                    else if (stream->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO && _audioStreamIndex == -1)
                    {
                        _audioStreamIndex = i;
                        _audioTimeBase = stream->time_base; // 保存音频时间基
                    }
                }

                if (_videoStreamIndex == -1)
                {
                    throw new InvalidOperationException("未找到视频流");
                }
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// 初始化编解码器
        /// </summary>
        private void InitializeCodecs()
        {
            unsafe
            {
                AVFormatContext* inputFormatContext = (AVFormatContext*)_inputFormatContextPtr;
                AVStream* videoStream = inputFormatContext->streams[_videoStreamIndex];
                AVCodecParameters* videoCodecPar = videoStream->codecpar;

                // 初始化视频解码器
                AVCodec* videoCodec = ffmpeg.avcodec_find_decoder(videoCodecPar->codec_id);
                if (videoCodec == null) throw new InvalidOperationException("不支持的视频解码器");

                AVCodecContext* videoCodecContext = ffmpeg.avcodec_alloc_context3(videoCodec);
                ffmpeg.avcodec_parameters_to_context(videoCodecContext, videoCodecPar);
                int errorCode = ffmpeg.avcodec_open2(videoCodecContext, videoCodec, null);
                if (errorCode < 0) throw new InvalidOperationException($"无法打开视频解码器: {GetFFmpegErrorDescription(errorCode)}");
                _videoCodecContextPtr = (IntPtr)videoCodecContext;

                // 初始化视频编码器（H264）
                AVCodec* encodeCodec = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_H264);
                if (encodeCodec == null) throw new InvalidOperationException("未找到H264编码器");

                AVCodecContext* encodeCodecContext = ffmpeg.avcodec_alloc_context3(encodeCodec);
                encodeCodecContext->width = _videoWidth;
                encodeCodecContext->height = _videoHeight;
                encodeCodecContext->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUV420P;
                encodeCodecContext->time_base = videoStream->time_base;
                encodeCodecContext->bit_rate = 1000000; // 1Mbps
                encodeCodecContext->gop_size = 10;

                errorCode = ffmpeg.avcodec_open2(encodeCodecContext, encodeCodec, null);
                if (errorCode < 0) throw new InvalidOperationException($"无法打开H264编码器: {GetFFmpegErrorDescription(errorCode)}");
                _encodeCodecContextPtr = (IntPtr)encodeCodecContext;

                // 初始化像素格式转换上下文（YUV420P -> RGB24）
                SwsContext* swsContext = ffmpeg.sws_getContext(
                    _videoWidth, _videoHeight, videoCodecContext->pix_fmt,
                    _videoWidth, _videoHeight, AVPixelFormat.AV_PIX_FMT_RGB24, 1, null, null, null);

                if (swsContext == null) throw new InvalidOperationException("无法创建像素格式转换上下文");
                _swsContextPtr = (IntPtr)swsContext;
            }
        }

        /// <summary>
        /// 处理RTSP流
        /// </summary>
        private async Task ProcessStreamAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int errorCode;
                unsafe
                {
                    AVFormatContext* inputFormatContext = (AVFormatContext*)_inputFormatContextPtr;
                    AVPacket* packet = (AVPacket*)_packetPtr;
                    errorCode = ffmpeg.av_read_frame(inputFormatContext, packet);
                }

                if (errorCode < 0)
                {
                    if (errorCode == ffmpeg.AVERROR_EOF) break;
                    await Task.Delay(100);
                    continue;
                }

                try
                {
                    int streamIndex;
                    unsafe
                    {
                        AVPacket* packet = (AVPacket*)_packetPtr;
                        streamIndex = packet->stream_index;
                    }

                    if (streamIndex == _videoStreamIndex)
                    {
                        await ProcessVideoFrameAsync();
                    }
                    else if (streamIndex == _audioStreamIndex)
                    {
                        await ProcessAudioFrameAsync();
                    }
                }
                finally
                {
                    unsafe
                    {
                        AVPacket* packet = (AVPacket*)_packetPtr;
                        ffmpeg.av_packet_unref(packet);
                    }
                }
            }
        }

        /// <summary>
        /// 处理视频帧
        /// </summary>
        private async Task ProcessVideoFrameAsync()
        {
            byte[] h264Data = null;
            long timestampMs = 0;
            bool isKeyFrame = false;

            unsafe
            {
                AVCodecContext* videoCodecContext = (AVCodecContext*)_videoCodecContextPtr;
                AVPacket* packet = (AVPacket*)_packetPtr;
                AVFrame* videoFrame = (AVFrame*)_videoFramePtr;
                AVFrame* rgbFrame = (AVFrame*)_rgbFramePtr;
                SwsContext* swsContext = (SwsContext*)_swsContextPtr;
                AVFormatContext* inputFormatContext = (AVFormatContext*)_inputFormatContextPtr;

                int errorCode = ffmpeg.avcodec_send_packet(videoCodecContext, packet);
                if (errorCode < 0) return;

                while (ffmpeg.avcodec_receive_frame(videoCodecContext, videoFrame) == 0)
                {
                    // 转换为RGB24
                    byte[] rgbBuffer = new byte[_videoWidth * _videoHeight * 3];
                    fixed (byte* pRgb = rgbBuffer)
                    {
                        rgbFrame->data[0] = pRgb;
                        rgbFrame->linesize[0] = _videoWidth * 3;

                        ffmpeg.sws_scale(
                            swsContext,
                            videoFrame->data,
                            videoFrame->linesize,
                            0,
                            _videoHeight,
                            rgbFrame->data,
                            rgbFrame->linesize);
                    }

                    // AI检测
                    var detectionBoxes = _aiDetector.Detect(rgbBuffer, _videoWidth, _videoHeight, _rtspParam);

                    // 提取参数
                    isKeyFrame = (videoFrame->flags & ffmpeg.AV_FRAME_FLAG_KEY) != 0;
                    long timestamp = videoFrame->pts;
                    timestampMs = (long)(timestamp * ffmpeg.av_q2d(inputFormatContext->streams[_videoStreamIndex]->time_base) * 1000);

                    // 提取H264数据
                    h264Data = new byte[packet->size];
                    Marshal.Copy((IntPtr)packet->data, h264Data, 0, packet->size);

                    break;
                }
            }

            // 推流（unsafe外的await）
            if (h264Data != null)
            {
                await _zlPusher.PushH264FrameAsync(h264Data, timestampMs, isKeyFrame);
            }
        }

        /// <summary>
        /// 处理音频帧
        /// </summary>
        private async Task ProcessAudioFrameAsync()
        {
            byte[] audioData = null;
            long timestampMs = 0;
            unsafe
            {
                AVFormatContext* inputFormatContext = (AVFormatContext*)_inputFormatContextPtr;
                AVPacket* packet = (AVPacket*)_packetPtr;

                // 提取音频信息
                long timestamp = packet->pts;
                timestampMs = (long)(timestamp * ffmpeg.av_q2d(inputFormatContext->streams[_audioStreamIndex]->time_base) * 1000);
                audioData = new byte[packet->size];
                Marshal.Copy((IntPtr)packet->data, audioData, 0, packet->size);
            }

            if (audioData != null)
            {
                // 推送到ZLMediaKit
                await _zlPusher.PushAacFrameAsync(audioData, timestampMs);
            }
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 获取FFmpeg错误描述
        /// </summary>
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

        /// <summary>
        /// 清理FFmpeg资源
        /// </summary>
        private void CleanupFFmpegResources()
        {
            unsafe
            {
                // 1. 释放像素格式转换上下文
                if (_swsContextPtr != IntPtr.Zero)
                {
                    ffmpeg.sws_freeContext((SwsContext*)_swsContextPtr);
                    _swsContextPtr = IntPtr.Zero;
                }

                // 2. 释放视频解码器上下文
                if (_videoCodecContextPtr != IntPtr.Zero)
                {
                    AVCodecContext* codecCtx = (AVCodecContext*)_videoCodecContextPtr;
                    ffmpeg.avcodec_free_context(&codecCtx);
                    _videoCodecContextPtr = IntPtr.Zero;
                }

                // 3. 释放音频解码器上下文
                if (_audioCodecContextPtr != IntPtr.Zero)
                {
                    AVCodecContext* codecCtx = (AVCodecContext*)_audioCodecContextPtr;
                    ffmpeg.avcodec_free_context(&codecCtx);
                    _audioCodecContextPtr = IntPtr.Zero;
                }

                // 4. 释放视频编码器上下文
                if (_encodeCodecContextPtr != IntPtr.Zero)
                {
                    AVCodecContext* codecCtx = (AVCodecContext*)_encodeCodecContextPtr;
                    ffmpeg.avcodec_free_context(&codecCtx);
                    _encodeCodecContextPtr = IntPtr.Zero;
                }

                // 5. 释放格式上下文
                if (_inputFormatContextPtr != IntPtr.Zero)
                {
                    AVFormatContext* fmtCtx = (AVFormatContext*)_inputFormatContextPtr;
                    ffmpeg.avformat_close_input(&fmtCtx);
                    _inputFormatContextPtr = IntPtr.Zero;
                }

                // 6. 释放视频帧
                if (_videoFramePtr != IntPtr.Zero)
                {
                    AVFrame* frame = (AVFrame*)_videoFramePtr;
                    ffmpeg.av_frame_free(&frame);
                    _videoFramePtr = IntPtr.Zero;
                }

                // 7. 释放RGB帧
                if (_rgbFramePtr != IntPtr.Zero)
                {
                    AVFrame* frame = (AVFrame*)_rgbFramePtr;
                    ffmpeg.av_frame_free(&frame);
                    _rgbFramePtr = IntPtr.Zero;
                }

                // 8. 释放数据包
                if (_packetPtr != IntPtr.Zero)
                {
                    AVPacket* pkt = (AVPacket*)_packetPtr;
                    ffmpeg.av_packet_free(&pkt);
                    _packetPtr = IntPtr.Zero;
                }
            }
        }
        #endregion

        #region 释放资源
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                _cts?.Cancel();
                _ = StopAsync();
                _aiDetector.Dispose();
                _zlPusher.Dispose();
            }

            CleanupFFmpegResources();

            _isDisposed = true;
        }

        ~RtspStreamProcessor()
        {
            Dispose(false);
        }
        #endregion
    }
}

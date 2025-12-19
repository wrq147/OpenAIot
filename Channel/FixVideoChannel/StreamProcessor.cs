using ChannelUtility.Message;
using FFmpeg.AutoGen;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace FixVideoChannel
{
    public class StreamProcessor : IDisposable
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
        private volatile List<AIDetectorTask> _aiDetectTaskList;
        private readonly ZLMediaKitPusher _zlPusher;
        private readonly string _inputUrl;
        private readonly string _pushUrl;
        #endregion

        #region 状态控制
        private bool _isDisposed;
        private int _videoStreamIndex = -1;
        private int _audioStreamIndex = -1;
        private int _videoWidth;
        private int _videoHeight;
        private AVRational _videoTimeBase; // 视频时间基
        private AVRational _audioTimeBase; // 音频时间基
        #endregion
        private byte[] _reusableRgbBuffer;
        private IntPtr _rgbToYuvSwsContextPtr;
        private IntPtr _yuvFramePtr;
        private bool _isProcessing = false;
        private volatile VideoCaptureItem _item;
        private IDeviceEventListener _listener;
        // 构造函数
        public StreamProcessor(VideoCaptureItem item, List<AIDetectorTask> tasks, IDeviceEventListener listener)
        {
            _item = item;
            _inputUrl = item.PullAddr;
            _pushUrl = item.PushAddr;
            // 初始化组件
            _aiDetectTaskList = tasks;
            _zlPusher = new ZLMediaKitPusher();
            _listener = listener;
        }
        public void UpdateAIDraw(string detType, List<BoxItem> boxList)
        {
            var detectTasks = _aiDetectTaskList?.ToArray() ?? Array.Empty<AIDetectorTask>();
            if (detectTasks != null && detectTasks.Length > 0)
            {
                foreach(var tmpit in detectTasks)
                {
                    tmpit.UpdateBoxList(detType, boxList);
                }
            }
        }
        /// <summary>
        /// 更新检测任务
        /// </summary>
        /// <param name="tasks"></param>
        public void UpdateDetectTask(List<AIDetectorTask> tasks)
        {
            _aiDetectTaskList = tasks ?? new List<AIDetectorTask>();
        }
        public void UpdateItem(VideoCaptureItem item)
        {
            _item = item;
        }
        /// <summary>
        /// 启动RTSP流处理
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<bool> StartAsync()
        {
            if (_isProcessing)
            {
                throw new InvalidOperationException("已在处理流");
            }
            try
            {
                // 初始化FFmpeg资源
                unsafe
                {
                    _inputFormatContextPtr = (IntPtr)ffmpeg.avformat_alloc_context();
                    _packetPtr = (IntPtr)ffmpeg.av_packet_alloc();
                    _videoFramePtr = (IntPtr)ffmpeg.av_frame_alloc();
                    _rgbFramePtr = (IntPtr)ffmpeg.av_frame_alloc();
                    _yuvFramePtr = (IntPtr)ffmpeg.av_frame_alloc();

                    if (_inputFormatContextPtr == IntPtr.Zero || _packetPtr == IntPtr.Zero ||
                        _videoFramePtr == IntPtr.Zero || _rgbFramePtr == IntPtr.Zero || _yuvFramePtr == IntPtr.Zero)
                    {
                        throw new OutOfMemoryException("无法分配FFmpeg资源");
                    }
                }

                // 1. 打开流
                OpenStream();

                // 2. 初始化编解码器
                InitializeCodecs();

                // 3. 初始化推流器的编码参数
                InitializePusherCodecParams();

                // 4. 连接ZLMediaKit
                _zlPusher.Connect(_pushUrl);

                // 5. 启动流处理任务
                _isProcessing = true;
                StreamTaskScheduler.Instance.EnqueueProcessTask(this);
                await _listener.OnEventOnline(_item);
                return true;
            }
            catch
            {
                _isProcessing = false;
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


        #region FFmpeg核心操作
        /// <summary>
        /// 打开流
        /// </summary>
        private void OpenStream()
        {
            unsafe
            {
                AVFormatContext* inputFormatContext = (AVFormatContext*)_inputFormatContextPtr;
                AVDictionary* options = null;

                // 根据URL协议类型设置不同的参数
                var uri = new Uri(_inputUrl);
                var protocol = uri.Scheme.ToLowerInvariant();

                switch (protocol)
                {
                    case "rtsp":
                        // RTSP专属参数
                        ffmpeg.av_dict_set(&options, "rtsp_transport", "tcp", 0);
                        ffmpeg.av_dict_set(&options, "stimeout", "5000000", 0); // 5秒超时（微秒）
                        ffmpeg.av_dict_set(&options, "buffer_size", "1024000", 0);
                        break;
                    case "rtmp":
                        // RTMP专属参数
                        ffmpeg.av_dict_set(&options, "timeout", "5000000", 0); // 5秒超时
                        ffmpeg.av_dict_set(&options, "buffer_size", "1024000", 0);
                        ffmpeg.av_dict_set(&options, "rtmp_connect_timeout", "5000", 0); // 连接超时（毫秒）
                        break;
                    case "http":
                    case "https":
                        // HTTP-FLV等协议参数
                        ffmpeg.av_dict_set(&options, "timeout", "5000000", 0);
                        break;
                    default:
                        // 通用参数
                        ffmpeg.av_dict_set(&options, "timeout", "5000000", 0);
                        break;
                }

                // 打开流（FFmpeg自动识别协议）
                int errorCode = ffmpeg.avformat_open_input(&inputFormatContext, _inputUrl, null, &options);
                if (errorCode < 0)
                {
                    ffmpeg.av_dict_free(&options);
                    throw new InvalidOperationException($"无法打开流（{protocol}）: {GetFFmpegErrorDescription(errorCode)}");
                }

                // 释放字典
                ffmpeg.av_dict_free(&options);

                // 更新IntPtr
                _inputFormatContextPtr = (IntPtr)inputFormatContext;

                // 获取流信息（通用逻辑）
                errorCode = ffmpeg.avformat_find_stream_info(inputFormatContext, null);
                if (errorCode < 0)
                {
                    throw new InvalidOperationException($"无法获取流信息: {GetFFmpegErrorDescription(errorCode)}");
                }

                // 查找视频和音频流索引（通用逻辑）
                for (int i = 0; i < inputFormatContext->nb_streams; i++)
                {
                    AVStream* stream = inputFormatContext->streams[i];
                    if (stream->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO && _videoStreamIndex == -1)
                    {
                        _videoStreamIndex = i;
                        _videoWidth = stream->codecpar->width;
                        _videoHeight = stream->codecpar->height;
                        _videoTimeBase = stream->time_base;
                        _reusableRgbBuffer = new byte[_videoWidth * _videoHeight * 3];
                    }
                    else if (stream->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO && _audioStreamIndex == -1)
                    {
                        _audioStreamIndex = i;
                        _audioTimeBase = stream->time_base;
                    }
                }

                if (_videoStreamIndex == -1)
                {
                    throw new InvalidOperationException("未找到视频流");
                }
            }
        }

        /// <summary>
        /// 初始化编解码器
        /// </summary>
        private unsafe void InitializeCodecs()
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
            encodeCodecContext->bit_rate = videoCodecPar->bit_rate > 0 ? videoCodecPar->bit_rate : 1000000;
            encodeCodecContext->gop_size = 10;
            encodeCodecContext->max_b_frames = 0; // 关闭B帧，减少延迟
            encodeCodecContext->framerate = ffmpeg.av_inv_q(videoStream->time_base);

            // 设置编码器选项：超快编码、零延迟
            AVDictionary* encodeOptions = null;
            ffmpeg.av_dict_set(&encodeOptions, "preset", "ultrafast", 0);
            ffmpeg.av_dict_set(&encodeOptions, "tune", "zerolatency", 0);
            ffmpeg.av_dict_set(&encodeOptions, "profile", "baseline", 0);

            errorCode = ffmpeg.avcodec_open2(encodeCodecContext, encodeCodec, &encodeOptions);
            ffmpeg.av_dict_free(&encodeOptions);
            if (errorCode < 0) throw new InvalidOperationException($"无法打开H264编码器: {GetFFmpegErrorDescription(errorCode)}");
            _encodeCodecContextPtr = (IntPtr)encodeCodecContext;

            // 初始化像素格式转换上下文（YUV420P -> RGB24）
            SwsContext* swsContext = ffmpeg.sws_getContext(
                _videoWidth, _videoHeight, videoCodecContext->pix_fmt,
                _videoWidth, _videoHeight, AVPixelFormat.AV_PIX_FMT_RGB24,
                1, null, null, null);

            if (swsContext == null) throw new InvalidOperationException("无法创建像素格式转换上下文");
            _swsContextPtr = (IntPtr)swsContext;

            // 初始化RGB24转YUV420P的转换上下文
            SwsContext* rgbToYuvSwsContext = ffmpeg.sws_getContext(
                _videoWidth, _videoHeight, AVPixelFormat.AV_PIX_FMT_RGB24,
                _videoWidth, _videoHeight, AVPixelFormat.AV_PIX_FMT_YUV420P,
                1, null, null, null);

            if (rgbToYuvSwsContext == null) throw new InvalidOperationException("无法创建RGB转YUV的转换上下文");
            _rgbToYuvSwsContextPtr = (IntPtr)rgbToYuvSwsContext;

            // 初始化YUV帧的缓冲区
            AVFrame* yuvFrame = (AVFrame*)_yuvFramePtr;
            yuvFrame->format = (int)AVPixelFormat.AV_PIX_FMT_YUV420P;
            yuvFrame->width = _videoWidth;
            yuvFrame->height = _videoHeight;
            errorCode = ffmpeg.av_frame_get_buffer(yuvFrame, 0);
            if (errorCode < 0) throw new InvalidOperationException($"无法分配YUV帧缓冲区: {GetFFmpegErrorDescription(errorCode)}");
        }

        /// <summary>
        /// 处理RTSP流
        /// </summary>
        public async Task ProcessStreamAsync()
        {
            if (!_isProcessing) return;
            int errorCode;
            unsafe
            {
                AVFormatContext* inputFormatContext = (AVFormatContext*)_inputFormatContextPtr;
                AVPacket* packet = (AVPacket*)_packetPtr;
                errorCode = ffmpeg.av_read_frame(inputFormatContext, packet);
            }

            if (errorCode < 0)
            {
                if (errorCode == ffmpeg.AVERROR_EOF)
                {
                    _isProcessing = false;
                    _zlPusher.Disconnect();
                    CleanupFFmpegResources();
                    await _listener.OnEventOffline(_item);
                }
                return;
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
                    ProcessAudioFrame();
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

            if (_isProcessing)
            {
                StreamTaskScheduler.Instance.EnqueueProcessTask(this);
            }
        }
        private int _increaseFrame = 0;
        /// <summary>
        /// 处理视频帧
        /// </summary>
        private async Task ProcessVideoFrameAsync()
        {
            long videoPts = 0;
            AVPictureType pictType = AVPictureType.AV_PICTURE_TYPE_NONE;
            bool decodeSuccess = false;

            try
            {
                unsafe
                {
                    AVCodecContext* videoCodecContext = (AVCodecContext*)_videoCodecContextPtr;
                    AVPacket* packet = (AVPacket*)_packetPtr;
                    AVFrame* videoFrame = (AVFrame*)_videoFramePtr;
                    AVFrame* rgbFrame = (AVFrame*)_rgbFramePtr;
                    SwsContext* swsContext = (SwsContext*)_swsContextPtr;

                    // 1. 发送数据包到解码器
                    int sendResult = ffmpeg.avcodec_send_packet(videoCodecContext, packet);
                    if (sendResult < 0)
                    {
                        if (sendResult != ffmpeg.AVERROR_EOF)
                        {
                            Console.WriteLine($"发送数据包到解码器失败: {GetFFmpegErrorDescription(sendResult)}");
                        }
                        return;
                    }

                    // 2. 循环接收解码后的帧（处理多帧情况，实时流中取第一帧后退出）
                    int receiveResult;
                    while ((receiveResult = ffmpeg.avcodec_receive_frame(videoCodecContext, videoFrame)) == 0)
                    {
                        // 转换为RGB24（使用复用的缓冲区）
                        fixed (byte* pRgb = _reusableRgbBuffer)
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

                        // 保存时间戳和帧类型
                        videoPts = videoFrame->pts;
                        pictType = (AVPictureType)videoFrame->pict_type;
                        decodeSuccess = true;

                        // 实时流中，处理一帧后退出循环（避免多帧累积）
                        break;
                    }

                    // 处理解码器的非致命错误
                    if (receiveResult != ffmpeg.AVERROR(ffmpeg.EAGAIN) && receiveResult != ffmpeg.AVERROR_EOF)
                    {
                        Console.WriteLine($"接收解码帧失败: {GetFFmpegErrorDescription(receiveResult)}");
                    }
                }

                // 3. 退出unsafe块后，执行异步AI检测
                if (decodeSuccess)
                {
                    ++_increaseFrame;
                    if (_increaseFrame >= _item.FrameInterval)
                    {
                        //发送给AI模块处理
                        var detectTasks = _aiDetectTaskList?.ToArray() ?? Array.Empty<AIDetectorTask>();
                        if (detectTasks != null && detectTasks.Length > 0)
                        {
                            // 并行执行AI检测任务，提高效率
                            await Task.WhenAll(detectTasks.Select(t => t.Detect(_item.Id, _reusableRgbBuffer, _videoWidth, _videoHeight, _listener)));
                        }

                        //处理AI模块返回的绘画
                        foreach (var t in detectTasks)
                        {
                            t.Draw(_reusableRgbBuffer, _videoWidth, _videoHeight);
                        }
                        _increaseFrame = 0;
                    }


                    // 4. 重新进入unsafe块，将处理后的RGB编码为H264
                    unsafe
                    {
                        AVCodecContext* encodeCodecContext = (AVCodecContext*)_encodeCodecContextPtr;
                        AVFrame* yuvFrame = (AVFrame*)_yuvFramePtr;
                        SwsContext* rgbToYuvSwsContext = (SwsContext*)_rgbToYuvSwsContextPtr;
                        AVFormatContext* inputFormatContext = (AVFormatContext*)_inputFormatContextPtr;

                        // 5. 将AI处理后的RGB转换为YUV420P
                        fixed (byte* pRgb = _reusableRgbBuffer)
                        {
                            AVFrame* rgbFrame = (AVFrame*)_rgbFramePtr;
                            rgbFrame->data[0] = pRgb;
                            rgbFrame->linesize[0] = _videoWidth * 3;

                            // 锁定YUV帧缓冲区
                            ffmpeg.av_frame_make_writable(yuvFrame);

                            // 转换RGB到YUV
                            ffmpeg.sws_scale(
                                rgbToYuvSwsContext,
                                rgbFrame->data,
                                rgbFrame->linesize,
                                0,
                                _videoHeight,
                                yuvFrame->data,
                                yuvFrame->linesize);
                        }

                        // 6. 设置YUV帧的属性
                        yuvFrame->pts = videoPts;
                        yuvFrame->pict_type = pictType;
                        bool isKeyFrame = (yuvFrame->pict_type == AVPictureType.AV_PICTURE_TYPE_I);

                        // 7. 发送YUV帧到编码器
                        int sendResult = ffmpeg.avcodec_send_frame(encodeCodecContext, yuvFrame);
                        if (sendResult < 0)
                        {
                            if (sendResult != ffmpeg.AVERROR(ffmpeg.EAGAIN) && sendResult != ffmpeg.AVERROR_EOF)
                            {
                                Console.WriteLine($"发送帧到编码器失败: {GetFFmpegErrorDescription(sendResult)}");
                            }
                            return;
                        }

                        // 8. 循环接收编码后的包（必须处理所有包，包括SPS/PPS）
                        AVPacket* encodePacket = ffmpeg.av_packet_alloc();
                        if (encodePacket == null) return;

                        try
                        {
                            int receiveResult;
                            while ((receiveResult = ffmpeg.avcodec_receive_packet(encodeCodecContext, encodePacket)) == 0)
                            {
                                // 时间基转换：编码器时间基 -> 流时间基
                                ffmpeg.av_packet_rescale_ts(encodePacket, encodeCodecContext->time_base,
                                    inputFormatContext->streams[_videoStreamIndex]->time_base);

                                // 提取H264数据
                                byte[] encodedH264Data = new byte[encodePacket->size];
                                Marshal.Copy((IntPtr)encodePacket->data, encodedH264Data, 0, encodePacket->size);

                                // 计算时间戳（毫秒）
                                long timestampMs = (long)(encodePacket->pts * ffmpeg.av_q2d(inputFormatContext->streams[_videoStreamIndex]->time_base) * 1000);

                                // 推送编码后的H264数据
                                _zlPusher.PushH264Frame(encodedH264Data, timestampMs, isKeyFrame);
                            }

                            // 处理编码器的非致命错误
                            if (receiveResult != ffmpeg.AVERROR(ffmpeg.EAGAIN) && receiveResult != ffmpeg.AVERROR_EOF)
                            {
                                Console.WriteLine($"接收编码包失败: {GetFFmpegErrorDescription(receiveResult)}");
                            }
                        }
                        finally
                        {
                            // 释放编码数据包
                            ffmpeg.av_packet_free(&encodePacket);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理视频帧失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理音频帧
        /// </summary>
        private void ProcessAudioFrame()
        {
            long timestampMs = 0;
            unsafe
            {
                AVFormatContext* inputFormatContext = (AVFormatContext*)_inputFormatContextPtr;
                AVPacket* packet = (AVPacket*)_packetPtr;

                // 提取音频信息
                long timestamp = packet->pts;
                timestampMs = (long)(timestamp * ffmpeg.av_q2d(inputFormatContext->streams[_audioStreamIndex]->time_base) * 1000);
                byte[] audioData = new byte[packet->size];
                Marshal.Copy((IntPtr)packet->data, audioData, 0, packet->size);
                _zlPusher.PushAacFrame(audioData, timestampMs);
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
        public void CleanupFFmpegResources()
        {
            unsafe
            {
                // 释放像素格式转换上下文
                if (_swsContextPtr != IntPtr.Zero)
                {
                    ffmpeg.sws_freeContext((SwsContext*)_swsContextPtr);
                    _swsContextPtr = IntPtr.Zero;
                }

                // 释放视频解码器上下文
                if (_videoCodecContextPtr != IntPtr.Zero)
                {
                    AVCodecContext* codecCtx = (AVCodecContext*)_videoCodecContextPtr;
                    ffmpeg.avcodec_free_context(&codecCtx);
                    _videoCodecContextPtr = IntPtr.Zero;
                }

                // 释放音频解码器上下文
                if (_audioCodecContextPtr != IntPtr.Zero)
                {
                    AVCodecContext* codecCtx = (AVCodecContext*)_audioCodecContextPtr;
                    ffmpeg.avcodec_free_context(&codecCtx);
                    _audioCodecContextPtr = IntPtr.Zero;
                }

                // 释放视频编码器上下文
                if (_encodeCodecContextPtr != IntPtr.Zero)
                {
                    AVCodecContext* codecCtx = (AVCodecContext*)_encodeCodecContextPtr;
                    ffmpeg.avcodec_free_context(&codecCtx);
                    _encodeCodecContextPtr = IntPtr.Zero;
                }

                // 释放格式上下文
                if (_inputFormatContextPtr != IntPtr.Zero)
                {
                    AVFormatContext* fmtCtx = (AVFormatContext*)_inputFormatContextPtr;
                    ffmpeg.avformat_close_input(&fmtCtx);
                    _inputFormatContextPtr = IntPtr.Zero;
                }

                if (_rgbToYuvSwsContextPtr != IntPtr.Zero)
                {
                    SwsContext* codecCtx = (SwsContext*)_rgbToYuvSwsContextPtr;
                    ffmpeg.sws_free_context(&codecCtx);
                    _rgbToYuvSwsContextPtr = IntPtr.Zero;
                }

                // 释放视频帧
                if (_videoFramePtr != IntPtr.Zero)
                {
                    AVFrame* frame = (AVFrame*)_videoFramePtr;
                    ffmpeg.av_frame_free(&frame);
                    _videoFramePtr = IntPtr.Zero;
                }

                // 释放RGB帧
                if (_rgbFramePtr != IntPtr.Zero)
                {
                    AVFrame* frame = (AVFrame*)_rgbFramePtr;
                    ffmpeg.av_frame_free(&frame);
                    _rgbFramePtr = IntPtr.Zero;
                }

                // 释放YUV帧
                if (_yuvFramePtr != IntPtr.Zero)
                {
                    AVFrame* frame = (AVFrame*)_yuvFramePtr;
                    ffmpeg.av_frame_free(&frame);
                    _yuvFramePtr = IntPtr.Zero;
                }

                // 释放数据包
                if (_packetPtr != IntPtr.Zero)
                {
                    AVPacket* pkt = (AVPacket*)_packetPtr;
                    ffmpeg.av_packet_free(&pkt);
                    _packetPtr = IntPtr.Zero;
                }

            }
            _zlPusher.CleanupFFmpegResources();
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
                _isProcessing = false;
                _zlPusher.Dispose();
            }

            CleanupFFmpegResources();

            _isDisposed = true;
        }

        ~StreamProcessor()
        {
            Dispose(false);
        }
        #endregion
    }
}

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
        private IntPtr _rgbToYuvSwsContextPtr;
        private IntPtr _yuvFramePtr;
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

        private byte[] _sps;
        private byte[] _pps;
        private byte[] _reusableRgbBuffer;
        private bool _isProcessing = false;
        private volatile VideoCaptureItem _item;
        public string VideoId => _item.Id;
        private IDeviceEventListener _listener;

        // 构造函数
        public StreamProcessor(VideoCaptureItem item, List<AIDetectorTask> tasks, IDeviceEventListener listener)
        {
            _item = item ?? throw new ArgumentNullException(nameof(item));
            _inputUrl = item.PullAddr ?? throw new ArgumentNullException(nameof(item.PullAddr));

            // 修复推流地址拼接逻辑
            if (string.IsNullOrEmpty(item.PushKey))
            {
                throw new ArgumentNullException(nameof(item.PushKey));
            }
            var uri = new Uri(_inputUrl);
            _pushUrl = $"{uri.Scheme}://{item.PushKey}";

            // 初始化组件
            _aiDetectTaskList = tasks ?? new List<AIDetectorTask>();
            _zlPusher = new ZLMediaKitPusher(_item.Id);
            _listener = listener ?? throw new ArgumentNullException(nameof(listener));
        }

        public void UpdateAIDraw(string detType, List<BoxItem> boxList)
        {
            var detectTasks = _aiDetectTaskList?.ToArray() ?? Array.Empty<AIDetectorTask>();
            foreach (var tmpit in detectTasks)
            {
                tmpit.UpdateBoxList(detType, boxList);
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
            _item = item ?? throw new ArgumentNullException(nameof(item));
        }

        /// <summary>
        /// 启动流处理
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

                    AVFrame* rgbFrame = (AVFrame*)_rgbFramePtr;
                    rgbFrame->format = (int)AVPixelFormat.AV_PIX_FMT_RGB24;
                    rgbFrame->width = 0;
                    rgbFrame->height = 0;
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
            catch (Exception ex)
            {
                _isProcessing = false;
                Console.WriteLine($"启动流处理失败: {ex.Message}");
                await _listener.OnEventOffline(_item);
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

            // 构建视频编码参数（修复时间基为1/1000，适配RTMP推流）
            var videoParams = new VideoCodecParams
            {
                sps = _sps,
                pps = _pps,
                Width = _videoWidth,
                Height = _videoHeight,
                TimeBase = new AVRational { num = 1, den = 1000 }, // 毫秒时间基，适配RTMP
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
                    TimeBase = new AVRational { num = 1, den = audioCodecPar->sample_rate }, // 音频采样率时间基
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

                // 获取流信息
                errorCode = ffmpeg.avformat_find_stream_info(inputFormatContext, null);
                if (errorCode < 0)
                {
                    throw new InvalidOperationException($"无法获取流信息: {GetFFmpegErrorDescription(errorCode)}");
                }

                // 查找视频和音频流索引
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

                if (_videoWidth <= 0 || _videoHeight <= 0)
                {
                    throw new InvalidOperationException("无效的视频分辨率");
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
            if (videoCodec == null) throw new InvalidOperationException($"不支持的视频解码器: {videoCodecPar->codec_id}");

            AVCodecContext* videoCodecContext = ffmpeg.avcodec_alloc_context3(videoCodec);
            if (videoCodecContext == null) throw new OutOfMemoryException("无法分配视频解码器上下文");

            int errorCode = ffmpeg.avcodec_parameters_to_context(videoCodecContext, videoCodecPar);
            if (errorCode < 0) throw new InvalidOperationException($"无法复制解码器参数: {GetFFmpegErrorDescription(errorCode)}");

            errorCode = ffmpeg.avcodec_open2(videoCodecContext, videoCodec, null);
            if (errorCode < 0) throw new InvalidOperationException($"无法打开视频解码器: {GetFFmpegErrorDescription(errorCode)}");
            _videoCodecContextPtr = (IntPtr)videoCodecContext;

            // 初始化视频编码器（H264）
            AVCodec* encodeCodec = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_H264);
            if (encodeCodec == null) throw new InvalidOperationException("未找到H264编码器");

            AVCodecContext* encodeCodecContext = ffmpeg.avcodec_alloc_context3(encodeCodec);
            if (encodeCodecContext == null) throw new OutOfMemoryException("无法分配编码器上下文");

            // 编码器参数（适配RTMP推流）
            encodeCodecContext->width = _videoWidth;
            encodeCodecContext->height = _videoHeight;
            encodeCodecContext->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUV420P;
            encodeCodecContext->time_base = new AVRational { num = 1, den = 90000 }; // RTMP标准时间基
            encodeCodecContext->bit_rate = videoCodecPar->bit_rate > 0 ? videoCodecPar->bit_rate : 1000000;
            encodeCodecContext->gop_size = 30; // 每30帧一个关键帧
            encodeCodecContext->max_b_frames = 0; // 关闭B帧
            encodeCodecContext->framerate = ffmpeg.av_inv_q(_videoTimeBase);
            encodeCodecContext->flags |= ffmpeg.AV_CODEC_FLAG_GLOBAL_HEADER; // 全局头

            // 设置编码器选项
            AVDictionary* encodeOptions = null;
            ffmpeg.av_dict_set(&encodeOptions, "preset", "ultrafast", 0);
            ffmpeg.av_dict_set(&encodeOptions, "tune", "zerolatency", 0);
            ffmpeg.av_dict_set(&encodeOptions, "profile", "baseline", 0);
            ffmpeg.av_dict_set(&encodeOptions, "crf", "23", 0);

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
            errorCode = ffmpeg.av_frame_get_buffer(yuvFrame, 32); // 32字节对齐
            if (errorCode < 0) throw new InvalidOperationException($"无法分配YUV帧缓冲区: {GetFFmpegErrorDescription(errorCode)}");

            // 提取 SPS/PPS
            var (sps, pps) = H264Utils.ExtractFirstSpsPps(videoCodecContext);

            _sps = sps;
            _pps = pps;

            if (_sps == null || _pps == null)
            {
                throw new InvalidOperationException("无法从视频流中提取SPS/PPS");
            }
        }

        /// <summary>
        /// 处理流
        /// </summary>
        public async Task ProcessStreamAsync()
        {
            if (!_isProcessing) return;

            int errorCode;
            unsafe
            {
                AVFormatContext* inputFormatContext = (AVFormatContext*)_inputFormatContextPtr;
                if (_packetPtr == IntPtr.Zero)
                {
                    return;
                }
                AVPacket* packet = (AVPacket*)_packetPtr;
                errorCode = ffmpeg.av_read_frame(inputFormatContext, packet);
            }

            if (errorCode < 0)
            {
                _isProcessing = false;
                _zlPusher.Disconnect();
                await _listener.OnEventOffline(_item);
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
            try
            {
                bool needProcess = false;
                _increaseFrame++;

                // 检查是否达到帧间隔
                if (_increaseFrame >= _item.FrameInterval)
                {
                    var detectTasks = _aiDetectTaskList?.ToArray() ?? Array.Empty<AIDetectorTask>();
                    needProcess = detectTasks.Length > 0;
                }

                // 分支1：无需AI处理，直接转发原始包
                if (!needProcess)
                {
                    unsafe
                    {
                        AVPacket* packet = (AVPacket*)_packetPtr;
                        AVFormatContext* inputFormatContext = (AVFormatContext*)_inputFormatContextPtr;

                        // 计算时间戳（毫秒）
                        long timestampMs = (long)(packet->pts * ffmpeg.av_q2d(_videoTimeBase) * 1000);
                        // 判断是否为关键帧
                        bool isKeyFrame = (packet->flags & ffmpeg.AV_PKT_FLAG_KEY) != 0;
                        // 提取原始H264数据
                        byte[] h264Data = new byte[packet->size];
                        Marshal.Copy((IntPtr)packet->data, h264Data, 0, packet->size);

                        // 关键帧添加SPS/PPS
                        if (isKeyFrame)
                        {
                            h264Data = H264Utils.AddSpsPpsToKeyFrame(h264Data, _sps, _pps);
                        }

                        // 推送数据
                        _zlPusher.PushH264Frame(h264Data, timestampMs, isKeyFrame);
                    }
                    return;
                }

                // 分支2：需要AI处理，执行解码-处理-编码流程
                long videoPts = 0;
                AVPictureType pictType = AVPictureType.AV_PICTURE_TYPE_NONE;
                bool decodeSuccess = false;

                unsafe
                {
                    AVCodecContext* videoCodecContext = (AVCodecContext*)_videoCodecContextPtr;
                    AVPacket* packet = (AVPacket*)_packetPtr;
                    AVFrame* videoFrame = (AVFrame*)_videoFramePtr;
                    AVFrame* rgbFrame = (AVFrame*)_rgbFramePtr;
                    SwsContext* swsContext = (SwsContext*)_swsContextPtr;

                    // 1. 发送数据包到解码器
                    int sendResult = ffmpeg.avcodec_send_packet(videoCodecContext, packet);
                    if (sendResult < 0 && sendResult != ffmpeg.AVERROR_EOF)
                    {
                        Console.WriteLine($"发送数据包到解码器失败: {GetFFmpegErrorDescription(sendResult)}");
                        return;
                    }

                    // 2. 接收解码后的帧
                    int receiveResult;
                    while ((receiveResult = ffmpeg.avcodec_receive_frame(videoCodecContext, videoFrame)) == 0)
                    {
                        // 转换为RGB24
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
                        break;
                    }

                    if (receiveResult != 0 && receiveResult != ffmpeg.AVERROR(ffmpeg.EAGAIN) && receiveResult != ffmpeg.AVERROR_EOF)
                    {
                        Console.WriteLine($"接收解码帧失败: {GetFFmpegErrorDescription(receiveResult)}");
                    }
                }

                if (decodeSuccess)
                {
                    bool hasDraw = false;
                    var detectTasks = _aiDetectTaskList?.ToArray() ?? Array.Empty<AIDetectorTask>();

                    // 执行AI检测
                    await Task.WhenAll(detectTasks.Select(t => t.Detect(_item.Id, _reusableRgbBuffer, _videoWidth, _videoHeight, _listener)));

                    // 执行绘制
                    foreach (var t in detectTasks)
                    {
                        if (t.Draw(_reusableRgbBuffer, _videoWidth, _videoHeight))
                        {
                            hasDraw = true;
                        }
                    }

                    _increaseFrame = 0;

                    // 如果无绘制，直接转发原始包
                    if (!hasDraw)
                    {
                        unsafe
                        {
                            AVPacket* packet = (AVPacket*)_packetPtr;
                            long timestampMs = (long)(packet->pts * ffmpeg.av_q2d(_videoTimeBase) * 1000);
                            bool isKeyFrame = (packet->flags & ffmpeg.AV_PKT_FLAG_KEY) != 0;
                            byte[] h264Data = new byte[packet->size];
                            Marshal.Copy((IntPtr)packet->data, h264Data, 0, packet->size);

                            if (isKeyFrame)
                            {
                                h264Data = H264Utils.AddSpsPpsToKeyFrame(h264Data, _sps, _pps);
                            }

                            _zlPusher.PushH264Frame(h264Data, timestampMs, isKeyFrame);
                        }
                        return;
                    }

                    // 有绘制，执行编码流程
                    unsafe
                    {
                        AVCodecContext* encodeCodecContext = (AVCodecContext*)_encodeCodecContextPtr;
                        AVFrame* yuvFrame = (AVFrame*)_yuvFramePtr;
                        SwsContext* rgbToYuvSwsContext = (SwsContext*)_rgbToYuvSwsContextPtr;

                        // 将RGB转换为YUV420P
                        fixed (byte* pRgb = _reusableRgbBuffer)
                        {
                            AVFrame* rgbFrame = (AVFrame*)_rgbFramePtr;
                            rgbFrame->data[0] = pRgb;
                            rgbFrame->linesize[0] = _videoWidth * 3;

                            ffmpeg.av_frame_make_writable(yuvFrame);

                            ffmpeg.sws_scale(
                                rgbToYuvSwsContext,
                                rgbFrame->data,
                                rgbFrame->linesize,
                                0,
                                _videoHeight,
                                yuvFrame->data,
                                yuvFrame->linesize);
                        }

                        // 设置YUV帧属性（修复PTS转换逻辑）
                        yuvFrame->pts = ffmpeg.av_rescale_q(videoPts, _videoTimeBase, encodeCodecContext->time_base);
                        yuvFrame->pict_type = pictType;
                        bool isKeyFrame = (yuvFrame->pict_type == AVPictureType.AV_PICTURE_TYPE_I);

                        // 发送帧到编码器
                        int sendResult = ffmpeg.avcodec_send_frame(encodeCodecContext, yuvFrame);
                        if (sendResult < 0 && sendResult != ffmpeg.AVERROR(ffmpeg.EAGAIN) && sendResult != ffmpeg.AVERROR_EOF)
                        {
                            Console.WriteLine($"发送帧到编码器失败: {GetFFmpegErrorDescription(sendResult)}");
                            return;
                        }

                        // 接收编码后的包
                        AVPacket* encodePacket = ffmpeg.av_packet_alloc();
                        if (encodePacket == null) return;

                        try
                        {
                            int receiveResult;
                            while ((receiveResult = ffmpeg.avcodec_receive_packet(encodeCodecContext, encodePacket)) == 0)
                            {
                                // 计算时间戳（毫秒）
                                long timestampMs = (long)(encodePacket->pts * ffmpeg.av_q2d(encodeCodecContext->time_base) * 1000);

                                // 提取H264数据
                                byte[] encodedH264Data = new byte[encodePacket->size];
                                Marshal.Copy((IntPtr)encodePacket->data, encodedH264Data, 0, encodePacket->size);

                                // 关键帧添加SPS/PPS
                                if (isKeyFrame)
                                {
                                    encodedH264Data = H264Utils.AddSpsPpsToKeyFrame(encodedH264Data, _sps, _pps);
                                }

                                // 推送数据
                                _zlPusher.PushH264Frame(encodedH264Data, timestampMs, isKeyFrame);

                                ffmpeg.av_packet_unref(encodePacket);
                            }

                            if (receiveResult != ffmpeg.AVERROR(ffmpeg.EAGAIN) && receiveResult != ffmpeg.AVERROR_EOF)
                            {
                                Console.WriteLine($"接收编码包失败: {GetFFmpegErrorDescription(receiveResult)}");
                            }
                        }
                        finally
                        {
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
            try
            {
                long timestampMs = 0;
                unsafe
                {
                    AVFormatContext* inputFormatContext = (AVFormatContext*)_inputFormatContextPtr;
                    AVPacket* packet = (AVPacket*)_packetPtr;

                    // 计算时间戳（毫秒）
                    long timestamp = packet->pts;
                    timestampMs = (long)(timestamp * ffmpeg.av_q2d(_audioTimeBase) * 1000);

                    // 提取音频数据
                    byte[] audioData = new byte[packet->size];
                    Marshal.Copy((IntPtr)packet->data, audioData, 0, packet->size);

                    // 推送音频数据
                    _zlPusher.PushAacFrame(audioData, timestampMs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理音频帧失败: {ex.Message}");
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

                if (_rgbToYuvSwsContextPtr != IntPtr.Zero)
                {
                    ffmpeg.sws_freeContext((SwsContext*)_rgbToYuvSwsContextPtr);
                    _rgbToYuvSwsContextPtr = IntPtr.Zero;
                }

                // 释放解码器上下文
                if (_videoCodecContextPtr != IntPtr.Zero)
                {
                    AVCodecContext* codecCtx = (AVCodecContext*)_videoCodecContextPtr;
                    ffmpeg.avcodec_free_context(&codecCtx);
                    _videoCodecContextPtr = IntPtr.Zero;
                }

                if (_audioCodecContextPtr != IntPtr.Zero)
                {
                    AVCodecContext* codecCtx = (AVCodecContext*)_audioCodecContextPtr;
                    ffmpeg.avcodec_free_context(&codecCtx);
                    _audioCodecContextPtr = IntPtr.Zero;
                }

                // 释放编码器上下文
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

                // 释放帧资源
                if (_videoFramePtr != IntPtr.Zero)
                {
                    AVFrame* frame = (AVFrame*)_videoFramePtr;
                    ffmpeg.av_frame_free(&frame);
                    _videoFramePtr = IntPtr.Zero;
                }

                if (_rgbFramePtr != IntPtr.Zero)
                {
                    AVFrame* frame = (AVFrame*)_rgbFramePtr;
                    ffmpeg.av_frame_free(&frame);
                    _rgbFramePtr = IntPtr.Zero;
                }

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

            // 清理推流器资源
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

            // 清理FFmpeg资源
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

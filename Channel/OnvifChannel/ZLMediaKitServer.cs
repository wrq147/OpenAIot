using ChannelUtility;
using ChannelUtility.Message;
using Microsoft.Extensions.DependencyInjection;
using Onvif.Core.Client.Camera;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ZLMediaKit;

namespace OnvifChannel
{
    public unsafe class ZLMediaKitServer
    {
        private ConcurrentDictionary<string, RecordContext> _recordContexts = new ConcurrentDictionary<string, RecordContext>();
        private ConcurrentDictionary<string, MkPlayerT> _players;
        // Player 相关回调委托
        private ZLMediaKit.OnMkPlayEvent _onPlayDelegate;
        private ZLMediaKit.OnMkPlayEvent _onShutdownDelegate;


        private ZLMediaKit.OnMkFrameOut _onParseFrameDelegate;
        private ZLMediaKit.OnMkDecode _onDecodeFrameDelegate;
        private ZLMediaKit.Delegates.Func_int___IntPtr___IntPtr _onMediaNotFoundDelegate;
        private ZLMediaKit.Delegates.Action___IntPtr _onMediaNoReaderDelegate;
        private ZLMediaKit.Delegates.Action_int___IntPtr _onMediaChangedDelegate;
        private ZLMediaKit.Delegates.Action___IntPtr___IntPtr___IntPtr _onMediaPublishDelegate;
        private ZLMediaKit.Delegates.Action___IntPtr___IntPtr___IntPtr _onMediaPlayDelegate;
        private ZLMediaKit.Delegates.Action___IntPtr___IntPtr_intPtr___IntPtr _onHttpRequestDelegate;
        private ZLMediaKit.Delegates.Action___IntPtr_string8_int___IntPtr___IntPtr _onHttpAccessDelegate;
        private ZLMediaKit.Delegates.Action___IntPtr_sbytePtr___IntPtr _onHttpBeforeRequestDelegate;
        private ZLMediaKit.Delegates.Action___IntPtr _onRecordMp4Delegate;
        private ZLMediaKit.Delegates.Action___IntPtr _onRecordHLSDelegate;
        private ZLMediaKit.Delegates.Action___IntPtr_ulong_ulong_int___IntPtr _onFlowReportDelegate;

        private MkEvents _mkEvents;
        private IServiceProvider _provider;
        private OnvifDeviceEventListener _listener;
        private OnvifOption _option;
        private ConcurrentDictionary<string, Account> _cameraAccountDict;
        private ConcurrentDictionary<string, string> _keyToId;
        private ConcurrentDictionary<string, VideoData> _videoItems;
        private ConcurrentDictionary<string, FrameContext> _contextMap;
        private ConcurrentDictionary<string, IntPtr> _contextPtrMap;

        private static readonly Lazy<ZLMediaKitServer> _instance = new Lazy<ZLMediaKitServer>(() => new ZLMediaKitServer());
        public static ZLMediaKitServer Instance => _instance.Value;
        private ZLMediaKitServer()
        {
            _players = new ConcurrentDictionary<string, MkPlayerT>();
            // Player 回调
            _onPlayDelegate = OnPlay;
            _onShutdownDelegate = OnShutdown;

            _keyToId = new ConcurrentDictionary<string, string>();
            _cameraAccountDict = new ConcurrentDictionary<string, Account>();
            _contextMap = new ConcurrentDictionary<string, FrameContext>();
            _contextPtrMap = new ConcurrentDictionary<string, IntPtr>();
            _videoItems = new ConcurrentDictionary<string, VideoData>();

            _onParseFrameDelegate = OnParseFrame;
            _onDecodeFrameDelegate = OnDecodeFrame;
            _onMediaNotFoundDelegate = On_mk_media_not_found;
            _onMediaNoReaderDelegate = On_mk_media_no_reader;
            _onMediaChangedDelegate = On_mk_media_changed;
            _onMediaPublishDelegate = On_mk_media_publish;
            _onMediaPlayDelegate = On_mk_media_play;
            _onHttpRequestDelegate = On_mk_http_request;
            _onHttpAccessDelegate = On_mk_http_access;
            _onHttpBeforeRequestDelegate = On_mk_http_before_access;
            _onRecordMp4Delegate = On_mk_record_mp4;
            _onRecordHLSDelegate = On_mk_record_hls;
            _onFlowReportDelegate = On_mk_flow_report;
        }

        private int On_mk_media_not_found(IntPtr url,
                                      IntPtr sock)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            var url_info = (MkMediaInfoT)url;
            var streamId = mk_events_objects.MkMediaInfoGetStream(url_info);
            var schema = mk_events_objects.MkMediaInfoGetSchema(url_info);
            eventBus.PublishMediaNotFound(streamId, 0);
            return 0;
        }
        private void On_mk_media_no_reader(IntPtr senderPtr)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            var sender = (MkMediaSourceT)senderPtr;
            var streamId = mk_events_objects.MkMediaSourceGetStream(sender);
            eventBus.PublishMediaNotReader(streamId, 0);
        }
        private void OnParseFrame(IntPtr user_data, IntPtr frame)
        {
            var mkFrame = (MkFrameT)frame;
            FrameContext context = CallbackHelper.UnwrapIntPtrToInstance<FrameContext>(user_data);
            if (context == null || !context.CanParse)
            {
                return;
            }
            try
            {
                if (_videoItems.TryGetValue(context.VideoId, out VideoData item))
                {
                    if (item.Configs != null && item.Configs.Count > 0 && context.VideoDecoder != null)
                    {
                        mk_transcode.MkDecoderDecode(context.VideoDecoder, mkFrame, 1, 0);
                        return;
                    }
                }
                if (context.Media != null)
                {
                    mk_media.MkMediaInputFrame(context.Media, mkFrame);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("解码异常：" + e.Message);

            }
        }
        private void OnDecodeFrame(IntPtr user_data, IntPtr yuvFrame)
        {
            MkFramePixT pixFrame = (MkFramePixT)yuvFrame;
            AVFrame avFrame = mk_transcode.MkFramePixGetAvFrame(pixFrame);
            long lpts = mk_transcode.MkGetAvFramePts(avFrame);
            int w = mk_transcode.MkGetAvFrameWidth(avFrame);
            int h = mk_transcode.MkGetAvFrameHeight(avFrame);
            int pixFmt = mk_transcode.MkGetAvFrameFormat(avFrame);
            FrameContext context = CallbackHelper.UnwrapIntPtrToInstance<FrameContext>(user_data);
            if (context == null || string.IsNullOrEmpty(context.VideoKey))
            {
                return;
            }

            const int pixelSize = 3;
            int rawLineSize = w * pixelSize;
            int alignedLineSize = (rawLineSize + 32 - 1) & ~(32 - 1);
            int totalSize = alignedLineSize * h;
            byte[] rgb24 = new byte[totalSize];
            try
            {
                if (context.Swscale == null)
                {
                    return;
                }
                unsafe
                {
                    fixed (byte* pRgb = rgb24)
                    {
                        mk_transcode.MkSwscaleInputFrame(context.Swscale, pixFrame, pRgb);
                    }
                }


                if (_videoItems.TryGetValue(context.VideoId, out VideoData item))
                {
                    if (context.Motion == null)
                    {
                        context.Motion = new MotionDetector();
                    }

                    context.Motion.MotionBlockRatioThreshold = item.MotionRatio;

                    var tmpboxlist = item.BoxList;
                    bool needDraw = tmpboxlist != null && tmpboxlist.Count > 0;
                    long now = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
                    if (now - context.LastTriggerTime >= item.CoolDownMs)
                    {
                        context.LastTriggerTime = now;
                        // 执行AI检测
                        var (isMotionDetected, motionRatio) = context.Motion.IsMotionKeyframe(rgb24, w, h);
                        if (isMotionDetected || item.NeedUp || needDraw)
                        {
                            AIDetectorTask.Detect(item, w, h, motionRatio, _listener, rgb24);
                        }
                    }

                    if (needDraw)
                    {
                        AIDetectorTask.Draw(rgb24, w, h, tmpboxlist);
                    }

                    byte[] yuvData;
                    int[] yuvLineSizes;
                    if (!ZLUtility.ConvertRgb24ToTargetYuv(rgb24, w, h, alignedLineSize, (AVPixelFormat)pixFmt, out yuvData, out yuvLineSizes))
                    {
                        return;
                    }

                    if (yuvLineSizes == null || yuvLineSizes.Length != 3)
                    {
                        Console.WriteLine("行大小数组长度错误，必须为3（Y/U/V）");
                        return;
                    }


                    context.YuvQueue.Enqueue(new YuvFrame
                    {
                        YuvData = yuvData,
                        LineSizes = yuvLineSizes,
                        Pts = lpts,
                        Width = w,
                        Height = h
                    });

                    context.FrameSemaphore.Release();

                    // 启动编码线程（只启动一次）
                    if (context.EncodeThread == null)
                    {
                        context.EncodeThread = new Thread(EncodeLoop)
                        {
                            IsBackground = true,
                            Priority = ThreadPriority.AboveNormal
                        };
                        context.EncodeThread.Start(context);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void EncodeLoop(object state)
        {
            var context = (FrameContext)state;
            while (context.CanParse)
            {
                context.FrameSemaphore.Wait(1000);
                if (!context.CanParse)
                    break;
                if (context.YuvQueue.TryDequeue(out var frame))
                {
                    try
                    {
                        unsafe
                        {
                            fixed (byte* pYuv = frame.YuvData)
                            {
                                IntPtr[] planes = new IntPtr[3];
                                planes[0] = (IntPtr)pYuv;
                                planes[1] = (IntPtr)(pYuv + frame.Width * frame.Height);
                                planes[2] = (IntPtr)(pYuv + frame.Width * frame.Height + (frame.Width / 2) * (frame.Height / 2));

                                // 真正耗时的调用，放在独立线程
                                mk_media.MkMediaInputYuv(
                                    context.Media,
                                    planes,
                                    frame.LineSizes,
                                    (ulong)frame.Pts);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"编码异常: {ex.Message}");
                    }
                }
            }
        }


        private void On_mk_media_changed(int regist, IntPtr senderPtr)
        {
            MkMediaSourceT mediaSourceT = (MkMediaSourceT)senderPtr;
            string streamId = mk_events_objects.MkMediaSourceGetStream(mediaSourceT);
            if (_keyToId.TryGetValue(streamId, out string tmpid))
            {
                if (_videoItems.TryGetValue(tmpid, out VideoData item))
                {
                    if (regist == 1)
                    {
                        _listener.OnEventOnline(item.Item);
                        if (_recordContexts.TryGetValue(streamId, out RecordContext tmprec))
                        {
                            var rs = mk_recorder.MkRecorderStart(tmprec.Msg.SaveType, "__defaultVhost__", "live", tmprec.Msg.StreamId, null, 0);
                            if (rs == 1)
                            {
                                tmprec.Callback.Invoke(true, string.Empty);
                            }
                            else
                            {
                                tmprec.Callback.Invoke(false, "录像失败");
                            }
                        }
                    }
                    else
                    {
                        _listener.OnEventOffline(item.Item);
                    }
                }
            }

        }

        private void On_mk_media_publish(IntPtr url,
                              IntPtr invoker,
                              IntPtr sock)
        {
        }
        private void On_mk_media_play(IntPtr url,
                               IntPtr invoker,
                               IntPtr sock)
        {
            //允许播放
            var url_info = (MkMediaInfoT)url;
            var schema = mk_events_objects.MkMediaInfoGetSchema(url_info);
            if (schema != "rtmp" && schema != "hls")
            {
                return;
            }
            var streamId = mk_events_objects.MkMediaInfoGetStream(url_info);
            mk_events_objects.MkAuthInvokerDo((MkAuthInvokerT)invoker, null);
        }

        private void On_mk_http_request(IntPtr parserPtr,
                      IntPtr invoker, int* consumed,
                      IntPtr sock)
        {

        }

        private void On_mk_http_access(IntPtr parserPtr,
                                       string path,
                                       int is_dir,
                                       IntPtr invoker,
                                       IntPtr sock)
        {
            var parser = (MkParserT)parserPtr;
            var sender = (MkSockInfoT)sock;

            //有访问权限,每次访问文件都需要鉴权
            mk_events_objects.MkHttpAccessPathInvokerDo((MkHttpAccessPathInvokerT)invoker, null, null, 0);
        }
        private void On_mk_http_before_access(IntPtr parser, string path, IntPtr sender)
        {

        }

        private void On_mk_record_mp4(IntPtr mp4Ptr)
        {
            var sender = (MkRecordInfoT)mp4Ptr;
            var app = mk_events_objects.MkRecordInfoGetApp(sender);
            var stream = mk_events_objects.MkRecordInfoGetStream(sender);
            var filePath = mk_events_objects.MkRecordInfoGetFilePath(sender);
            var fileName = mk_events_objects.MkRecordInfoGetFileName(sender);
            var fileSize = mk_events_objects.MkRecordInfoGetFileSize(sender);
            var startTime = mk_events_objects.MkRecordInfoGetStartTime(sender);
            var timeLen = mk_events_objects.MkRecordInfoGetTimeLen(sender);

            if (_recordContexts.TryGetValue(stream, out RecordContext tmprec))
            {
                _ = _listener.OnSendRecordFile(tmprec.Msg.DeviceId, stream, fileName, fileSize, startTime, timeLen, tmprec.StorageWay, 0);
                if (tmprec.StorageWay == 1)
                {
                    string upfilePosition = $"{stream}/{startTime.ToString("yyyy-MM-dd")}/{fileName}";
                    _provider.GetService<MinioHelper>().UploadFile(filePath, upfilePosition);
                }
            }

        }
        private void On_mk_record_hls(IntPtr hlsPtr)
        {
            var sender = (MkRecordInfoT)hlsPtr;
            var app = mk_events_objects.MkRecordInfoGetApp(sender);
            var stream = mk_events_objects.MkRecordInfoGetStream(sender);
            var filePath = mk_events_objects.MkRecordInfoGetFilePath(sender);
            var fileName = mk_events_objects.MkRecordInfoGetFileName(sender);
            var fileSize = mk_events_objects.MkRecordInfoGetFileSize(sender);
            var startTime = mk_events_objects.MkRecordInfoGetStartTime(sender);
            var timeLen = mk_events_objects.MkRecordInfoGetTimeLen(sender);

            if (_recordContexts.TryGetValue(stream, out RecordContext tmprec))
            {
                var videoId = tmprec.Msg.DeviceId;
                var storageWay = tmprec.StorageWay;
                var archiveTimer = new System.Timers.Timer(1000);
                archiveTimer.AutoReset = false;
                archiveTimer.Elapsed += (senderTimer, e) =>
                {
                    try
                    {
                        string sliceDir = Path.GetDirectoryName(filePath);
                        string vodM3u8Path = Path.Combine(sliceDir, "vod.m3u8");
                        bool isVodArchived = File.Exists(vodM3u8Path);
                        if (isVodArchived)
                        {
                            _ = _listener.OnSendRecordFile(videoId, stream, fileName, fileSize, startTime, timeLen, storageWay, 1);
                        }
                    }
                    finally
                    {
                        // 立即停止并销毁定时器，防止内存泄漏
                        archiveTimer.Stop();
                        archiveTimer.Dispose();
                    }
                };
                archiveTimer.Start();

            }
        }


        private void On_mk_flow_report(IntPtr url,
                                      ulong total_bytes,
                                      ulong total_seconds,
                                      int is_player,
                                      IntPtr sock)
        {

        }

        private void OnPlay(IntPtr user_data, int err_code, string err_msg, IntPtr[] tracks, int track_count)
        {
            FrameContext context = CallbackHelper.UnwrapIntPtrToInstance<FrameContext>(user_data);
            for (int i = 0; i < track_count; i++)
            {
                if (i >= tracks.Length)
                {
                    continue;
                }
                MkTrackT mkTrack = (MkTrackT)tracks[i];

                if (mk_track.MkTrackIsVideo(mkTrack) > 0)
                {
                    int codec_id = mk_track.MkTrackCodecId(mkTrack);
                    int width = mk_track.MkTrackVideoWidth(mkTrack);
                    int height = mk_track.MkTrackVideoHeight(mkTrack);
                    float tfps = mk_track.MkTrackVideoFps(mkTrack);
                    int bit_rate = mk_track.MkTrackBitRate(mkTrack);
                    mk_media.MkMediaInitVideo(context.Media, codec_id, width, height, tfps, bit_rate);

                    MkDecoderT mkDecoder = mk_transcode.MkDecoderCreate(mkTrack, 0);
                    context.VideoDecoder = mkDecoder;
                    context.Swscale = mk_transcode.MkSwscaleCreate(2, 0, 0);

                    mk_transcode.MkDecoderSetCb(mkDecoder, _onDecodeFrameDelegate, user_data);
                    mk_track.MkTrackAddDelegate(mkTrack, _onParseFrameDelegate, user_data);

                }
                else
                {
                    mk_media.MkMediaInitTrack(context.Media, mkTrack);
                }
            }

            mk_media.MkMediaInitComplete(context.Media);
        }
        private void OnShutdown(IntPtr user_data, int err_code, string err_msg, IntPtr[] tracks, int track_count)
        {
            FrameContext context = CallbackHelper.UnwrapIntPtrToInstance<FrameContext>(user_data);
            context.CanParse = false;
            Thread.Sleep(200);
            if (context.Swscale != null)
            {
                mk_transcode.MkSwscaleRelease(context.Swscale);
                context.Swscale = null;
            }
            if (context.VideoDecoder != null)
            {
                mk_transcode.MkDecoderRelease(context.VideoDecoder, 1);
                context.VideoDecoder = null;
            }

        }
        public MyCamera AddAccount(VideoData data)
        {
            Account account = new Account(data.Item.PullAddr, data.Item.UserName, data.Item.Password);
            _cameraAccountDict.TryAdd(data.Item.Id, account);
            var camera = MyCamera.Create(account, ex =>
            {
                Console.WriteLine(ex.Message);
            });
            return camera;
        }
        public (MyCamera, VideoData) GetCamera(string videoId)
        {
            if (_videoItems.TryGetValue(videoId, out var videodata))
            {
                if (_cameraAccountDict.TryGetValue(videodata.Item.Id, out var tmpacc))
                {
                    return (MyCamera.Get(tmpacc), videodata);
                }
            }
            return (null, null);
        }
        public void UpdateCamera(VideoData data)
        {

            FrameContext context = new FrameContext();
            context.VideoKey = data.Item.PushKey;
            context.VideoId = data.Item.Id;
            IntPtr contextPtr = CallbackHelper.WrapInstanceToIntPtr(context);
            _contextPtrMap.TryAdd(context.VideoKey, contextPtr);
            _contextMap.TryAdd(context.VideoKey, context);
            //创建播放器
            MkPlayerT mkPlayer = mk_player.MkPlayerCreate();

            mk_player.MkPlayerSetOnResult(mkPlayer, _onPlayDelegate, contextPtr);
            mk_player.MkPlayerSetOnShutdown(mkPlayer, _onShutdownDelegate, contextPtr);
            _keyToId.TryAdd(data.Item.PushKey, data.Item.Id);
            _players.TryAdd(data.Item.Id, mkPlayer);

            MkIniT option = mk_util.MkIniCreate();
            mk_util.MkIniSetOptionInt(option, "enable_mp4", 0);
            mk_util.MkIniSetOptionInt(option, "enable_audio", 1);
            mk_util.MkIniSetOptionInt(option, "enable_fmp4", 0);
            mk_util.MkIniSetOptionInt(option, "enable_ts", 0);
            mk_util.MkIniSetOptionInt(option, "enable_hls", 0);
            mk_util.MkIniSetOptionInt(option, "enable_rtsp", 0);
            mk_util.MkIniSetOptionInt(option, "enable_rtmp", 1);
            mk_util.MkIniSetOptionInt(option, "add_mute_audio", 0);
            mk_util.MkIniSetOptionInt(option, "auto_close", 0);
            context.Media = mk_media.MkMediaCreate2("__defaultVhost__", "live", context.VideoKey, 0, option);
            mk_util.MkIniRelease(option);
            mk_player.MkPlayerPlay(mkPlayer, data.url);
        }
        public void RemoveCamera(string id)
        {
            if (_cameraAccountDict.TryRemove(id, out Account tmpCameraAccount))
            {
                if (_videoItems.TryRemove(id, out VideoData tmpdata))
                {
                    if (_contextMap.TryRemove(tmpdata.Item.PushKey, out FrameContext handle))
                    {
                        handle.CanParse = false;
                        Thread.Sleep(200);
                        if (handle.Media != null)
                        {
                            mk_media.MkMediaRelease(handle.Media);
                            handle.Media = null;
                        }
                        handle.FrameSemaphore.Release();
                        handle.EncodeThread = null;
                    }
                    if (_contextPtrMap.TryRemove(tmpdata.Item.PushKey, out IntPtr contextPtr))
                    {
                        CallbackHelper.FreeInstancePtr(contextPtr);
                    }
                    _keyToId.Remove(tmpdata.Item.PushKey, out string tmpvvv);
                }

                MyCamera.ReleaseCamera(tmpCameraAccount);
            }
        }

        public void UpdateAIDraw(string videoId, List<BoxItem> boxList, bool needUpdate)
        {
            if (_videoItems.TryGetValue(videoId, out VideoData tmpval))
            {
                if (needUpdate)
                {
                    tmpval.NeedUp = true;
                }
                else
                {
                    tmpval.BoxList = boxList;
                }
            }
        }
        public void RecorderStart(MediaRecordStartMessage msg, int retrycount, Action<bool, string> cb)
        {
            var mediaSource = mk_events_objects.MkMediaSourceFind2("rtmp", "__defaultVhost__", "live", msg.StreamId, 0);
            if (mediaSource == null)
            {
                RecordContext recordContext = new RecordContext();
                recordContext.StorageWay = msg.Storage;
                recordContext.Msg = msg;
                recordContext.Callback = cb;
                _recordContexts.AddOrUpdate(msg.StreamId, recordContext, (s, r) => recordContext);

                _provider.GetService<ClientBusProxy>().PublishMediaNotFound(msg.StreamId, 0);
                return;
            }
            if (msg.Storage == 0)
            {
                var rs = mk_recorder.MkRecorderStart(msg.SaveType, "__defaultVhost__", "live", msg.StreamId, null, 0);
                if (rs == 1)
                {
                    cb.Invoke(true, string.Empty);
                }
                else
                {
                    if (retrycount > 0)
                    {
                        cb.Invoke(false, "未知原因");
                        return;
                    }
                    Thread.Sleep(100);
                    RecorderStart(msg, retrycount + 1, cb);
                }
            }
            else
            {
                cb.Invoke(false, "存储方式不支持");
            }
        }
        public void RecorderStop(MediaRecordStopMessage msg, Action<bool, string> cb)
        {
            bool isSuccess = false;
            string reason = string.Empty;
            var rs = mk_recorder.MkRecorderStop(msg.SaveType, "__defaultVhost__", "live", msg.StreamId);
            if (rs == 1)
            {
                isSuccess = true;
            }
            else
            {
                reason = "关闭录像失败";
            }
            cb.Invoke(isSuccess, reason);
        }
        public void Start(OnvifOption option, IServiceProvider provider, OnvifDeviceEventListener listener)
        {
            _provider = provider;
            _option = option;
            _listener = listener;


            unsafe
            {

                var init_path = mk_util.MkUtilGetExeDir("config.ini");

                MkConfig config = new MkConfig()
                {
                    Ini = new string(init_path),
                    IniIsPath = 1,
                    LogLevel = 0,
                    LogMask = (int)LogMask.Console,
                    LogFilePath = null,
                    LogFileDays = 0,
                    Ssl = string.Empty,
                    SslIsPath = 1,
                    SslPwd = null,
                    ThreadNum = 0
                };


                mk_common.MkEnvInit(config);
                mk_common.MkRtpServerStart((ushort)_option.rtp_port);
                mk_common.MkRtmpServerStart((ushort)_option.rtmp_port, 0);
                mk_common.MkHttpServerStart((ushort)_option.http_port, 0);

                _mkEvents = new MkEvents()
                {
                    OnMkMediaNotFound = _onMediaNotFoundDelegate,
                    OnMkMediaNoReader = _onMediaNoReaderDelegate,
                    OnMkMediaChanged = _onMediaChangedDelegate,
                    OnMkMediaPublish = _onMediaPublishDelegate,
                    OnMkMediaPlay = _onMediaPlayDelegate,
                    OnMkHttpRequest = _onHttpRequestDelegate,
                    OnMkHttpAccess = _onHttpAccessDelegate,
                    OnMkHttpBeforeAccess = _onHttpBeforeRequestDelegate,
                    OnMkRecordMp4 = _onRecordMp4Delegate,
                    OnMkRecordTs = _onRecordHLSDelegate,
                    OnMkFlowReport = _onFlowReportDelegate
                };
                MkEvents.MkEventsListen(_mkEvents);
            }
        }

        public void Stop()
        {
            mk_common.MkStopAllServer();
        }

    }

    public class RecordContext
    {
        public byte StorageWay { get; set; }
        public MediaRecordStartMessage Msg { get; set; }
        public Action<bool, string> Callback { get; set; }
    }
    public class VideoData
    {
        private List<BoxItem> _boxList;

        // 公开属性：每次赋新值
        public List<BoxItem> BoxList
        {
            get => Volatile.Read(ref _boxList);
            set => Volatile.Write(ref _boxList, value);
        }
        public VideoCaptureItem Item { get; set; }
        public float MotionRatio { get; set; }
        public int CoolDownMs { get; set; }
        public List<AIConfigData> Configs { get; set; }
        public bool NeedUp { get; set; }
        public string token { get; set; }
        public string url { get; set; }
    }

    public class FrameContext
    {
        public SemaphoreSlim FrameSemaphore { get; set; } = new SemaphoreSlim(0);
        public ConcurrentQueue<YuvFrame> YuvQueue { get; set; } = new ConcurrentQueue<YuvFrame>();
        public Thread EncodeThread { get; set; }
        public bool CanParse { get; set; } = true;
        public string VideoId { get; set; }
        public string VideoKey { get; set; }
        public MkMediaT Media { get; set; }
        public MkDecoderT VideoDecoder { get; set; }
        public MkSwscaleT Swscale { get; set; }
        public MotionDetector Motion { get; set; }
        // 上次触发时间
        public long LastTriggerTime { get; set; } = 0;
    }
    public class YuvFrame
    {
        public byte[] YuvData { get; set; }
        public int[] LineSizes { get; set; }
        public long Pts { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public static class CallbackHelper
    {
        /// <summary>
        /// 将实例绑定到GCHandle
        /// </summary>
        public static IntPtr WrapInstanceToIntPtr(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            GCHandle handle = GCHandle.Alloc(instance);
            return GCHandle.ToIntPtr(handle);
        }

        /// <summary>
        /// 仅获取实例
        /// </summary>
        public static T? UnwrapIntPtrToInstance<T>(IntPtr ptr) where T : class
        {
            if (ptr == IntPtr.Zero) return null;

            try
            {
                GCHandle handle = GCHandle.FromIntPtr(ptr);
                return handle.Target as T;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 手动释放句柄（仅在注销时调用）
        /// </summary>
        public static void FreeInstancePtr(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return;

            try
            {
                GCHandle handle = GCHandle.FromIntPtr(ptr);
                if (handle.IsAllocated)
                    handle.Free();
            }
            catch (Exception)
            {
                // 忽略释放失败（比如已释放）
            }
        }
    }
}

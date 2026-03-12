using ChannelUtility;
using ChannelUtility.Message;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ZLMediaKit;

namespace FixVideoChannel
{
    public unsafe class ZLMediaKitServer
    {
        private IServiceProvider _provider;
        private FixVideoOption _option;
        private ConcurrentDictionary<string, MkPlayerT> _players;
        private ConcurrentDictionary<string, VideoData> _videoKeyItems;
        private ConcurrentDictionary<string, string> _IdToKeys;
        private ConcurrentDictionary<string, FrameContext> _contextMap;
        private ConcurrentDictionary<string, IntPtr> _contextPtrMap;

        // Player 相关回调委托
        private ZLMediaKit.OnMkPlayEvent _onPlayDelegate;
        private ZLMediaKit.OnMkPlayEvent _onShutdownDelegate;
        // Track/Decoder 回调委托
        private ZLMediaKit.OnMkFrameOut _onParseFrameDelegate;
        private ZLMediaKit.OnMkDecode _onDecodeFrameDelegate;
        // ZLMediaKit 全局事件委托（避免隐式委托被回收）
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

        private IVideoDeviceEventListener _listener;
        private MkEvents _mkEvents;
        private static readonly Lazy<ZLMediaKitServer> _instance = new Lazy<ZLMediaKitServer>(() => new ZLMediaKitServer());
        public static ZLMediaKitServer Instance => _instance.Value;
        private ZLMediaKitServer()
        {

            // Player 回调
            _onPlayDelegate = OnPlay;
            _onShutdownDelegate = OnShutdown;
            // Track/Decoder 回调
            _onParseFrameDelegate = OnParseFrame;
            _onDecodeFrameDelegate = OnDecodeFrame;
            // 全局事件回调
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
            if (context == null)
            {
                return;
            }
            context.LastFrame = mkFrame;
            try
            {
                if (_videoKeyItems.TryGetValue(context.VideoKey, out VideoData item))
                {
                    if (item.Configs != null && item.Configs.Count > 0)
                    {
                        mk_transcode.MkDecoderDecode(context.VideoDecoder, mkFrame, 0, 0);
                        if (context.LastFrame != null)
                        {
                            mk_media.MkMediaInputFrame(context.Media, mkFrame);
                            context.LastFrame = null;
                        }

                        return;
                    }
                }
                mk_media.MkMediaInputFrame(context.Media, mkFrame);
            }
            catch (Exception e)
            {
                Console.WriteLine("解码异常：" + e.Message);

            }
            finally
            {
                context.LastFrame = null;
            }
        }
        private void OnDecodeFrame(IntPtr user_data, IntPtr yuvFrame)
        {
            MkFramePixT pixFrame = (MkFramePixT)yuvFrame;
            ZLMediaKit.AVFrame avFrame = mk_transcode.MkFramePixGetAvFrame(pixFrame);
            long lpts = mk_transcode.MkGetAvFramePts(avFrame);
            int w = mk_transcode.MkGetAvFrameWidth(avFrame);
            int h = mk_transcode.MkGetAvFrameHeight(avFrame);
            int pixFmt = mk_transcode.MkGetAvFrameFormat(avFrame);
            FrameContext context = CallbackHelper.UnwrapIntPtrToInstance<FrameContext>(user_data);
            if (context == null || string.IsNullOrEmpty(context.VideoKey))
            {
                return;
            }
            byte[] rgb24 = FrameBufferPool.GetRgb24Buffer(context.VideoKey, w, h);
            try
            {
                unsafe
                {
                    fixed (byte* pRgb = rgb24)
                    {
                        mk_transcode.MkSwscaleInputFrame(context.Swscale, pixFrame, pRgb);
                    }
                }

                if (_videoKeyItems.TryGetValue(context.VideoKey, out VideoData item))
                {
                    if (context.Motion == null)
                    {
                        context.Motion = new MotionDetector();
                    }
                    bool hasDraw = false;

                    context.Motion.CoolDownMs = item.CoolDownMs;
                    context.Motion.MotionBlockRatioThreshold = item.MotionRatio;
                    // 执行AI检测
                    var (isMotionDetected, motionRatio) = context.Motion.IsMotionKeyframe(rgb24, w, h);
                    if (isMotionDetected || item.NeedUp)
                    {
                        AIDetectorTask.Detect(item, w, h, motionRatio, _listener, rgb24);
                    }



                    // 执行绘制
                    if (item.BoxList != null && item.BoxList.Count > 0)
                    {
                        AIDetectorTask.Draw(rgb24, w, h, item.BoxList);
                        hasDraw = true;
                    }

                    if (hasDraw)
                    {
                        byte[] yuvData;
                        int[] yuvLineSizes;
                        int alignedLineSize = (w * 3 + 31) & ~31;
                        if (!ZLUtility.ConvertRgb24ToTargetYuv(rgb24, w, h, alignedLineSize, (AVPixelFormat)pixFmt, out yuvData, out yuvLineSizes))
                        {
                            return;
                        }
                        // 2. 校验行大小数组长度（必须为3）
                        if (yuvLineSizes == null || yuvLineSizes.Length != 3)
                        {
                            Console.WriteLine("行大小数组长度错误，必须为3（Y/U/V）");
                            return;
                        }

                        unsafe
                        {
                            // 3. 固定托管YUV数组，防止GC回收/移动
                            fixed (byte* pYuvBase = yuvData)
                            {
                                // 4. 构建3个平面的指针数组（对应 C 层 const char* yuv[3]）
                                IntPtr[] yuvPlanes = new IntPtr[3];
                                // Y平面：起始地址
                                yuvPlanes[0] = (IntPtr)pYuvBase;
                                // U平面：Y平面后偏移 w*h 字节
                                yuvPlanes[1] = (IntPtr)(pYuvBase + w * h);
                                // V平面：U平面后偏移 (w/2)*(h/2) 字节
                                yuvPlanes[2] = (IntPtr)(pYuvBase + w * h + (w / 2) * (h / 2));

                                mk_media.MkMediaInputYuv(context.Media, yuvPlanes, yuvLineSizes, (ulong)lpts);
                                context.LastFrame = null;
                            }
                        }
                    }
                }
            }
            finally
            {
                FrameBufferPool.ReturnRgb24Buffer(context.VideoKey, rgb24);
            }
        }
        private void On_mk_media_changed(int regist, IntPtr senderPtr)
        {
            MkMediaSourceT mediaSourceT = (MkMediaSourceT)senderPtr;
            string streamId = mk_events_objects.MkMediaSourceGetStream(mediaSourceT);
            if (_videoKeyItems.TryGetValue(streamId, out VideoData item))
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
        private void On_mk_media_publish(IntPtr url,
                          IntPtr invoker,
                          IntPtr sock)
        {
            ////允许推流，并且允许转hls/mp4
            //mk_events_objects.MkPublishAuthInvokerDo((MkPublishAuthInvokerT)invoker, null, 1, 1);
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
            if (_videoKeyItems.ContainsKey(streamId))
            {
                mk_events_objects.MkAuthInvokerDo((MkAuthInvokerT)invoker, null);
            }
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
                mk_media.MkMediaInitTrack(context.Media, mkTrack);
                if (mk_track.MkTrackIsVideo(mkTrack) > 0)
                {
                    MkDecoderT mkDecoder = mk_transcode.MkDecoderCreate(mkTrack, 0);
                    context.VideoDecoder = mkDecoder;
                    context.Swscale = mk_transcode.MkSwscaleCreate(2, 0, 0);

                    mk_transcode.MkDecoderSetCb(mkDecoder, _onDecodeFrameDelegate, user_data);
                    mk_track.MkTrackAddDelegate(mkTrack, _onParseFrameDelegate, user_data);
                }
            }
            mk_media.MkMediaInitComplete(context.Media);
        }
        private void OnShutdown(IntPtr user_data, int err_code, string err_msg, IntPtr[] tracks, int track_count)
        {
            FrameContext context = CallbackHelper.UnwrapIntPtrToInstance<FrameContext>(user_data);
            if (context.Swscale != null)
            {
                mk_transcode.MkSwscaleRelease(context.Swscale);
            }
            if (context.VideoDecoder != null)
            {
                mk_transcode.MkDecoderRelease(context.VideoDecoder, 1);
            }

        }
        public void AddPullProxy(VideoData data)
        {
            if (_players.ContainsKey(data.Item.Id))
            {
                return;
            }
            //创建播放器
            MkPlayerT mkPlayer = mk_player.MkPlayerCreate();
            FrameContext context = new FrameContext();
            context.VideoKey = data.Item.PushKey;
            IntPtr contextPtr = CallbackHelper.WrapInstanceToIntPtr(context);
            _contextPtrMap.TryAdd(context.VideoKey, contextPtr);
            _contextMap.TryAdd(context.VideoKey, context);


            mk_player.MkPlayerSetOnResult(mkPlayer, _onPlayDelegate, contextPtr);
            mk_player.MkPlayerSetOnShutdown(mkPlayer, _onShutdownDelegate, contextPtr);

            _players.TryAdd(data.Item.Id, mkPlayer);
            _IdToKeys.TryAdd(data.Item.Id, data.Item.PushKey);
            _videoKeyItems.TryAdd(data.Item.PushKey, data);


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


            mk_player.MkPlayerPlay(mkPlayer, data.Item.PullAddr);
        }
        private ConcurrentDictionary<string, RecordContext> _recordContexts = new ConcurrentDictionary<string, RecordContext>();
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
        public void RemovePullProxy(string id)
        {
            if (_players.TryRemove(id, out MkPlayerT tmpt))
            {
                if (_IdToKeys.TryRemove(id, out string tkey))
                {
                    _videoKeyItems.TryRemove(tkey, out VideoData tmpval);
                    if (_contextPtrMap.TryRemove(tkey, out IntPtr contextPtr))
                    {
                        CallbackHelper.FreeInstancePtr(contextPtr);
                    }
                    if (_contextMap.TryRemove(tkey, out FrameContext handle))
                    {
                        if (handle.Media != null)
                        {
                            mk_media.MkMediaRelease(handle.Media);
                        }
                        FrameBufferPool.ClearCache(tkey);
                    }
                }
                mk_player.MkPlayerRelease(tmpt);
            }
        }
        public void UpdateAIDraw(string videoId, List<BoxItem> boxList, bool needUpdate)
        {
            if (_IdToKeys.TryGetValue(videoId, out string tmpkey))
            {
                if (_videoKeyItems.TryGetValue(tmpkey, out VideoData tmpval))
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
        }

        public void Start(FixVideoOption option, IServiceProvider provider, IVideoDeviceEventListener listener)
        {
            _provider = provider;
            _option = option;
            _listener = listener;
            _players = new ConcurrentDictionary<string, MkPlayerT>();
            _videoKeyItems = new ConcurrentDictionary<string, VideoData>();
            _IdToKeys = new ConcurrentDictionary<string, string>();
            _contextMap = new ConcurrentDictionary<string, FrameContext>();
            _contextPtrMap = new ConcurrentDictionary<string, IntPtr>();
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

                mk_common.MkRtmpServerStart((ushort)_option.RTMPPort, 0);
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
            FrameBufferPool.ClearAllCache();
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
        public VideoCaptureItem Item { get; set; }
        public float MotionRatio { get; set; }
        public int CoolDownMs { get; set; }
        public List<BoxItem> BoxList { get; set; }
        public List<AIConfigData> Configs { get; set; }
        public bool NeedUp { get; set; }
    }
    public class FrameContext
    {
        public string VideoKey { get; set; }
        public MkMediaT Media { get; set; }
        public MkDecoderT VideoDecoder { get; set; }
        public MkSwscaleT Swscale { get; set; }
        public MkFrameT LastFrame { get; set; }
        public MotionDetector Motion { get; set; }
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

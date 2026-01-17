using ChannelUtility;
using GB28181Channel.GB28181;
using GB28181Channel.GB28181.DTO;
using GB28181Channel.GB28181.Event;
using GB28181Channel.GB28181.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ZLMediaKit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GB28181Channel
{
    public unsafe class ZLMediaKitServer
    {
        private IServiceProvider _provider;
        private GB28181Option _option;
        private GB28181DeviceEventListener _listener;
        private MkEvents _mkEvents;
        private ZLMediaKit.OnMkFrameOut _onParseFrameDelegate;
        private ZLMediaKit.OnMkDecode _onDecodeFrameDelegate;
        private ZLMediaKit.Delegates.Func_int___IntPtr___IntPtr _onMediaNotFoundDelegate;
        private ZLMediaKit.Delegates.Action___IntPtr _onMediaNoReaderDelegate;
        private ZLMediaKit.Delegates.Action_int___IntPtr _onMediaChangedDelegate;
        private ZLMediaKit.Delegates.Action___IntPtr___IntPtr___IntPtr _onMediaPublishDelegate;
        private ZLMediaKit.Delegates.Action___IntPtr___IntPtr___IntPtr _onMediaPlayDelegate;
        private ZLMediaKit.Delegates.Action___IntPtr___IntPtr_intPtr___IntPtr _onHttpRequestDelegate;
        private ZLMediaKit.Delegates.Action___IntPtr_string8_int___IntPtr___IntPtr _onHttpAccessDelegate;
        private ZLMediaKit.Delegates.Action___IntPtr _onRecordMp4Delegate;
        private ZLMediaKit.Delegates.Action___IntPtr_ulong_ulong_int___IntPtr _onFlowReportDelegate;
        private GB28181Server _server;
        private ConcurrentDictionary<string, PlaybackParams> _mediaDict;
        private ConcurrentDictionary<string, FrameContext> _contextMap;
        private ConcurrentDictionary<string, IntPtr> _contextPtrMap;
        private static readonly Lazy<ZLMediaKitServer> _instance = new Lazy<ZLMediaKitServer>(() => new ZLMediaKitServer());
        public static ZLMediaKitServer Instance => _instance.Value;
        private ZLMediaKitServer()
        {
            _onParseFrameDelegate = OnParseFrame;
            _onDecodeFrameDelegate = OnDecodeFrame;
            _onMediaNotFoundDelegate = On_mk_media_not_found;
            _onMediaNoReaderDelegate = On_mk_media_no_reader;
            _onMediaChangedDelegate = On_mk_media_changed;
            _onMediaPublishDelegate = On_mk_media_publish;
            _onMediaPlayDelegate = On_mk_media_play;
            _onHttpRequestDelegate = On_mk_http_request;
            _onHttpAccessDelegate = On_mk_http_access;
            _onRecordMp4Delegate = On_mk_record_mp4;
            _onFlowReportDelegate = On_mk_flow_report;
        }
        private int On_mk_media_not_found(IntPtr url,
                                IntPtr sock)
        {
            var url_info = (MkMediaInfoT)url;
            var streamId = mk_events_objects.MkMediaInfoGetStream(url_info);
            var lowerstreamid = streamId.ToLower();
            if (_mediaDict.TryGetValue(lowerstreamid, out var playbackParams))
            {
                InMemoryDeviceStorage storage = (InMemoryDeviceStorage)_provider.GetService<IDeviceStorage>();
                var channelList = storage.GetChannelsByDeviceId(playbackParams.DeviceId);
                var channelInfo = channelList.Where(x => x.ChannelId == playbackParams.ChannelId).FirstOrDefault();
                if (channelInfo == null)
                {
                    return 1;
                }
                _ = _server.StartActiveStream(channelInfo.DeviceId, channelInfo.ChannelId, _option.rtp_port);
                return 0;
            }
            else
            {
                InMemoryDeviceStorage storage = (InMemoryDeviceStorage)_provider.GetService<IDeviceStorage>();
                var channelInfo = storage.GetChannelFrom(streamId);
                if (channelInfo == null)
                {
                    return 1;
                }

                _ = _server.StartActiveStream(channelInfo.DeviceId, channelInfo.ChannelId, _option.rtp_port);
                return 0;
            }
 
        }
        private void On_mk_media_no_reader(IntPtr senderPtr)
        {
            var sender = (MkMediaSourceT)senderPtr;
            var streamId = mk_events_objects.MkMediaSourceGetStream(sender);
            InMemoryDeviceStorage storage = (InMemoryDeviceStorage)_provider.GetService<IDeviceStorage>();
            var channelInfo = storage.GetChannelFrom(streamId);
            if (channelInfo != null)
            {
                _ = _server.StopActiveStream(channelInfo.DeviceId, channelInfo.ChannelId);
            }
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
            var storage = _provider.GetService<IDeviceStorage>();
            var device = storage.GetDevice(context.DeviceId);
            if (device != null)
            {
                if (device.VideoData.DetectList.Count > 0)
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
            context.LastFrame = null;
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
                var storage = _provider.GetService<IDeviceStorage>();
                var device = storage.GetDevice(context.DeviceId);
                if (device != null)
                {
                    if (context.Motion == null)
                    {
                        context.Motion = new MotionDetector();
                    }
                    bool hasDraw = false;
                    var detectTasks = device.VideoData.DetectList;
                    byte[] tdata = rgb24;
                    bool isPress = false;

                    context.Motion.CoolDownMs = device.VideoData.CoolDownMs;
                    context.Motion.MotionBlockRatioThreshold = device.VideoData.MotionRatio;
                    // 执行AI检测
                    var (isMotionDetected, motionRatio) = context.Motion.IsMotionKeyframe(rgb24, w, h);
                    if (isMotionDetected)
                    {
                        foreach (var task in detectTasks)
                        {
                            task.Detect(device.VideoData.Item.Id, w, h, motionRatio, _listener, ref tdata, ref isPress);
                        }
                    }


                    // 执行绘制
                    foreach (var t in detectTasks)
                    {
                        if (t.Draw(rgb24, w, h))
                        {
                            hasDraw = true;
                        }
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
            string streamId = mk_events_objects.MkMediaSourceGetStream(mediaSourceT).ToLower();

            if (_mediaDict.TryGetValue(streamId, out var playbackParams))
            {
                var storage = _provider.GetService<IDeviceStorage>();
                var channelList = storage.GetChannelsByDeviceId(playbackParams.DeviceId);
                var channelInfo = channelList.Where(x => x.ChannelId == playbackParams.ChannelId).FirstOrDefault();
                if (channelInfo == null)
                {
                    return;
                }
                if (regist == 1)
                {
                    if (_contextMap.ContainsKey(channelInfo.PushKey))
                    {
                        return;
                    }


                    FrameContext context = new FrameContext();
                    context.VideoKey = channelInfo.PushKey;
                    context.DeviceId = channelInfo.DeviceId;
                    context.ChannelId = channelInfo.ChannelId;

                    IntPtr contextPtr = CallbackHelper.WrapInstanceToIntPtr(context);
                    _contextPtrMap.TryAdd(context.VideoKey, contextPtr);
                    _contextMap.TryAdd(context.VideoKey, context);

                    //创建实时拉流
                    MkIniT option = mk_util.MkIniCreate();
                    mk_util.MkIniSetOptionInt(option, "enable_mp4", 0);
                    mk_util.MkIniSetOptionInt(option, "enable_audio", 1);
                    mk_util.MkIniSetOptionInt(option, "enable_fmp4", 0);
                    mk_util.MkIniSetOptionInt(option, "enable_ts", 0);
                    mk_util.MkIniSetOptionInt(option, "enable_hls", 1);
                    mk_util.MkIniSetOptionInt(option, "enable_rtsp", 0);
                    mk_util.MkIniSetOptionInt(option, "enable_rtmp", 1);
                    mk_util.MkIniSetOptionInt(option, "add_mute_audio", 0);
                    mk_util.MkIniSetOptionInt(option, "auto_close", 0);

                    context.Media = mk_media.MkMediaCreate2("__defaultVhost__", "live", context.VideoKey, 0, option);
                    mk_util.MkIniRelease(option);

                    int trackCount = mk_events_objects.MkMediaSourceGetTrackCount(mediaSourceT);
                    for (int i = 0; i < trackCount; i++)
                    {
                        MkTrackT mkTrack = mk_events_objects.MkMediaSourceGetTrack(mediaSourceT, i);
                        if (mkTrack == null) { continue; }
                        mk_media.MkMediaInitTrack(context.Media, mkTrack);
                        if (mk_track.MkTrackIsVideo(mkTrack) > 0)
                        {
                            MkDecoderT mkDecoder = mk_transcode.MkDecoderCreate(mkTrack, 0);
                            context.VideoDecoder = mkDecoder;
                            context.Swscale = mk_transcode.MkSwscaleCreate(2, 0, 0);

                            mk_transcode.MkDecoderSetCb(mkDecoder, _onDecodeFrameDelegate, contextPtr);
                            mk_track.MkTrackAddDelegate(mkTrack, _onParseFrameDelegate, contextPtr);
                        }
                    }
                    mk_media.MkMediaInitComplete(context.Media);
                }
                else
                {
                    _mediaDict.TryRemove(streamId, out PlaybackParams ch);
                    if (_contextPtrMap.TryRemove(channelInfo.PushKey, out IntPtr contextPtr))
                    {
                        CallbackHelper.FreeInstancePtr(contextPtr);
                    }
                    if (_contextMap.TryRemove(channelInfo.PushKey, out FrameContext context))
                    {
                        if (context.Swscale != null)
                        {
                            mk_transcode.MkSwscaleRelease(context.Swscale);
                        }
                        if (context.VideoDecoder != null)
                        {
                            mk_transcode.MkDecoderRelease(context.VideoDecoder, 1);
                        }
                        if (context.Media != null)
                        {
                            mk_media.MkMediaRelease(context.Media);
                        }
                        FrameBufferPool.ClearCache(channelInfo.PushKey);
                    }
                }
            }
        }
        private void On_mk_media_publish(IntPtr url,
                              IntPtr invoker,
                              IntPtr sock)
        {
            var url_info = (MkMediaInfoT)url;
            var rawStreamId = mk_events_objects.MkMediaInfoGetStream(url_info);
            var streamId = rawStreamId.ToLower();
            if (_mediaDict.TryGetValue(streamId, out var playbackParams))
            {
                MkIniT toption = mk_util.MkIniCreate();
                mk_util.MkIniSetOptionInt(toption, "enable_mp4", 0);
                mk_util.MkIniSetOptionInt(toption, "enable_audio", 1);
                mk_util.MkIniSetOptionInt(toption, "enable_fmp4", 0);
                mk_util.MkIniSetOptionInt(toption, "enable_ts", 0);
                mk_util.MkIniSetOptionInt(toption, "enable_hls", 0);
                mk_util.MkIniSetOptionInt(toption, "enable_rtsp", 1);
                mk_util.MkIniSetOptionInt(toption, "enable_rtmp", 0);
                mk_util.MkIniSetOptionInt(toption, "add_mute_audio", 0);
                mk_util.MkIniSetOptionInt(toption, "auto_close", 0);
                mk_events_objects.MkPublishAuthInvokerDo2((MkPublishAuthInvokerT)invoker, null, toption);
                mk_util.MkIniRelease(toption);


            }
        }
        private void On_mk_media_play(IntPtr url,
                               IntPtr invoker,
                               IntPtr sock)
        {
            //允许播放
            var url_info = (MkMediaInfoT)url;
            var streamId = mk_events_objects.MkMediaInfoGetStream(url_info);
            InMemoryDeviceStorage storage = (InMemoryDeviceStorage)_provider.GetService<IDeviceStorage>();
            var channelInfo = storage.GetChannelFrom(streamId);
            if (channelInfo == null)
            {
                return;
            }
            mk_events_objects.MkAuthInvokerDo((MkAuthInvokerT)invoker, null);
        }
        private unsafe void On_mk_http_request(IntPtr parserPtr,
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

        private void On_mk_record_mp4(IntPtr mp4Ptr)
        {
        }

        private void On_mk_flow_report(IntPtr url,
                                      ulong total_bytes,
                                      ulong total_seconds,
                                      int is_player,
                                      IntPtr sock)
        {

        }
        public void BindSsrc(StreamPlayEventArgs e)
        {
            string streamId = ZLUtility.SsrcToStreamId(e.Params.Ssrc);
            _mediaDict.AddOrUpdate(streamId, _ => e.Params, (x, y) => e.Params);
        }
        public void Start(GB28181Option option, IServiceProvider provider, GB28181DeviceEventListener listener, GB28181Server server)
        {
            _provider = provider;
            _option = option;
            _listener = listener;
            _server = server;
            _mediaDict = new ConcurrentDictionary<string, PlaybackParams>();
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
                    OnMkRecordMp4 = _onRecordMp4Delegate,
                    OnMkFlowReport = _onFlowReportDelegate
                };
                MkEvents.MkEventsListen(_mkEvents);
            }
        }

        public void Stop()
        {
            _mediaDict.Clear();
            mk_common.MkStopAllServer();
        }
    }

    public class FrameContext
    {
        public string VideoKey { get; set; }
        public string DeviceId { get; set; }
        public string ChannelId { get; set; }
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

using ChannelUtility;
using ChannelUtility.Message;
using FFmpeg.AutoGen;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Exceptions;
using System.Collections.Concurrent;

using System.Runtime.InteropServices;
using System.Text;
using ZLMediaKit;

namespace FixVideoChannel
{
    public class ZLMediaKitServer
    {
        private IServiceProvider _provider;
        private FixVideoOption _option;
        private ConcurrentDictionary<string, MkPlayerT> _players;
        private ConcurrentDictionary<string, VideoData> _videoKeyItems;
        private ConcurrentDictionary<string, string> _IdToKeys;
        private ConcurrentDictionary<string, FrameContext> _contextMap;
        private ConcurrentDictionary<string, IntPtr> _contextPtrMap;
        private IDeviceEventListener _listener;
        private MkEvents _mkEvents;
        private static readonly Lazy<ZLMediaKitServer> _instance = new Lazy<ZLMediaKitServer>(() => new ZLMediaKitServer());
        public static ZLMediaKitServer Instance => _instance.Value;
        private ZLMediaKitServer() { }

        private int On_mk_media_not_found(IntPtr url,
                                        IntPtr sock)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            var url_info = (MkMediaInfoT)url;
            var streamId = mk_events_objects.MkMediaInfoGetStream(url_info);
            eventBus.PublishMediaNotFound(streamId);
            return 0;
        }
        private void On_mk_media_no_reader(IntPtr senderPtr)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            var sender = (MkMediaSourceT)senderPtr;
            var streamId = mk_events_objects.MkMediaSourceGetStream(sender);
            eventBus.PublishMediaNotReader(streamId);
        }
        private void OnParseFrame(IntPtr user_data, IntPtr frame)
        {
            var mkFrame = (MkFrameT)frame;
            FrameContext context = CallbackHelper.UnwrapIntPtrToInstance<FrameContext>(user_data);
            if (_videoKeyItems.TryGetValue(context.VideoKey, out VideoData item))
            {
                if (item.DetectList.Count > 0)
                {
                    mk_transcode.MkDecoderDecode(context.VideoDecoder, mkFrame, 1, 1);
                    return;
                }
            }
            mk_media.MkMediaInputFrame(context.Media, mkFrame);
        }
        private void OnDecodeFrame(IntPtr user_data, IntPtr yuvFrame)
        {
            MkFramePixT pixFrame = (MkFramePixT)yuvFrame;
            ZLMediaKit.AVFrame avFrame = mk_transcode.MkFramePixGetAvFrame(pixFrame);
            long lpts = mk_transcode.MkGetAvFramePts(avFrame);
            int w = mk_transcode.MkGetAvFrameWidth(avFrame);
            int h = mk_transcode.MkGetAvFrameHeight(avFrame);
            int pixFmt = mk_transcode.MkGetAvFrameFormat(avFrame);
            int linesizeLength = 4;
            switch (pixFmt)
            {
                case (int)AVPixelFormat.AV_PIX_FMT_YUV420P:
                    linesizeLength = 3;
                    break;
                case (int)AVPixelFormat.AV_PIX_FMT_YUV422P:
                    linesizeLength = 3;
                    break;
                case (int)AVPixelFormat.AV_PIX_FMT_YUV444P:
                    linesizeLength = 3;
                    break;
                case (int)AVPixelFormat.AV_PIX_FMT_NV12:
                    linesizeLength = 2;
                    break;
                case (int)AVPixelFormat.AV_PIX_FMT_RGB24:
                    linesizeLength = 1;
                    break;
                case (int)AVPixelFormat.AV_PIX_FMT_BGR24:
                    linesizeLength = 1;
                    break;
            }
            int[] tmplinesize = new int[linesizeLength];
            unsafe
            {
                int* linesizePtr = mk_transcode.MkGetAvFrameLineSize(avFrame);
                Marshal.Copy((IntPtr)linesizePtr, tmplinesize, 0, linesizeLength);
            }

            FrameContext context = CallbackHelper.UnwrapIntPtrToInstance<FrameContext>(user_data);
            int align = 32;
            int pixel_size = 3;
            int raw_linesize = w * pixel_size;
            // 对齐后的宽度
            int aligned_linesize = (raw_linesize + align - 1) & ~(align - 1);
            int total_size = aligned_linesize * h;
            byte[] brg24 = new byte[total_size];
            unsafe
            {
                fixed (byte* pRgb = brg24)
                {
                    mk_transcode.MkSwscaleInputFrame(context.Swscale, pixFrame, pRgb);
                }
            }

            if (_videoKeyItems.TryGetValue(context.VideoKey, out VideoData item))
            {
                bool hasDraw = false;
                var detectTasks = item.DetectList.ToArray();
                // 执行AI检测
                detectTasks.Select(t => t.Detect(item.Item.Id, brg24, w, h, _listener));
                // 执行绘制
                foreach (var t in detectTasks)
                {
                    if (t.Draw(brg24, w, h))
                    {
                        hasDraw = true;
                    }
                }
                if (hasDraw)
                {
                    //long pts = mk_frame.MkAvFrameGetPts(ref avFrame);
                    string[] yuv = new string[0];
                    mk_media.MkMediaInputYuv(context.Media, yuv, tmplinesize, (ulong)lpts);


                }
            }


        }
        private void On_mk_media_changed(int regist, IntPtr senderPtr)
        {

        }
        private void On_mk_media_publish(IntPtr url,
                          IntPtr invoker,
                          IntPtr sock)
        {
            //允许推流，并且允许转hls/mp4
            mk_events_objects.MkPublishAuthInvokerDo((MkPublishAuthInvokerT)invoker, null, 1, 1);
        }
        private void On_mk_media_play(IntPtr url,
                               IntPtr invoker,
                               IntPtr sock)
        {
            //允许播放
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
        private void OnPlay(IntPtr user_data, int err_code, string err_msg, IntPtr[] tracks, int track_count)
        {
            FrameContext context = CallbackHelper.UnwrapIntPtrToInstance<FrameContext>(user_data);
            context.Media = mk_media.MkMediaCreate("_defaultVhost_", "live", context.VideoKey, 0, 0, 0);
            for (int i = 0; i < track_count; i++)
            {
                MkTrackT mkTrack = (MkTrackT)tracks[i];
                if (mk_track.MkTrackIsVideo(mkTrack) > 0)
                {
                    MkDecoderT mkDecoder = mk_transcode.MkDecoderCreate(mkTrack, 0);
                    context.VideoDecoder = mkDecoder;
                    context.Track = mkTrack;
                    context.Swscale = mk_transcode.MkSwscaleCreate(3, 0, 0);

                    mk_transcode.MkDecoderSetCb(mkDecoder, OnDecodeFrame, user_data);
                    mk_track.MkTrackAddDelegate(mkTrack, OnParseFrame, user_data);
                    break;
                }
            }
            _contextMap.TryAdd(context.VideoKey, context);
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
            if (context.Media != null)
            {
                mk_media.MkMediaRelease(context.Media);
            }
            _contextMap.TryRemove(context.VideoKey, out FrameContext handle);
        }
        public void AddPullProxy(VideoData data)
        {
            if (_players.ContainsKey(data.Item.Id))
            {
                return;
            }
            //创建拉流代理
            MkPlayerT mkPlayer = mk_player.MkPlayerCreate();
            //MkProxyPlayerT mk_proxy = mk_proxyplayer.MkProxyPlayerCreate4("__defaultVhost__", "live", data.Item.PushKey, option, 3);
            ////设置代理参数 rtp_type  rtsp播放方式:RTP_TCP = 0, RTP_UDP = 1, RTP_MULTICAST = 2
            //mk_proxyplayer.MkProxyPlayerSetOption(mk_proxy, "rtp_type", "1");
            ////设置代理参数 protocol_timeout_ms  协议超时时间 毫秒 更多参数参见mk_proxy_player_set_option注释
            //mk_proxyplayer.MkProxyPlayerSetOption(mk_proxy, "protocol_timeout_ms", "2000");
            //如果是rtsp回放流支持配置开始倍速
            //ZLM_API.mk_proxy_player_set_option(mk_proxy, "rtsp_speed", "1.5");
            //开始播放代理地址
            mk_player.MkPlayerPlay(mkPlayer, data.Item.PullAddr);
            FrameContext context = new FrameContext();
            context.VideoKey = data.Item.PushKey;
            IntPtr contextPtr = CallbackHelper.WrapInstanceToIntPtr(context);
            _contextPtrMap.TryAdd(context.VideoKey, contextPtr);

            mk_player.MkPlayerSetOnResult(mkPlayer, OnPlay, contextPtr);
            mk_player.MkPlayerSetOnShutdown(mkPlayer, OnShutdown, contextPtr);

            _players.TryAdd(data.Item.Id, mkPlayer);
            _IdToKeys.TryAdd(data.Item.Id, data.Item.PushKey);
            _videoKeyItems.TryAdd(data.Item.PushKey, data);
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
                }
                mk_player.MkPlayerRelease(tmpt);
            }
        }
        public void Start(FixVideoOption option, IServiceProvider provider, IDeviceEventListener listener)
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

                mk_common.MkRtspServerStart((ushort)_option.zlmedia_server.RTSPPort, 0);
                mk_common.MkRtmpServerStart((ushort)_option.zlmedia_server.RTMPPort, 0);

                _mkEvents = new MkEvents()
                {
                    OnMkMediaNotFound = On_mk_media_not_found,
                    OnMkMediaNoReader = On_mk_media_no_reader,
                    OnMkMediaChanged = On_mk_media_changed,
                    OnMkMediaPublish = On_mk_media_publish,
                    OnMkMediaPlay = On_mk_media_play,
                    OnMkHttpRequest = On_mk_http_request,
                    OnMkHttpAccess = On_mk_http_access,
                    OnMkRecordMp4 = On_mk_record_mp4,
                    OnMkFlowReport = On_mk_flow_report
                };
                MkEvents.MkEventsListen(_mkEvents);

            }
        }

        public void Stop()
        {
            mk_common.MkStopAllServer();
        }
    }
    public class VideoData
    {
        public VideoCaptureItem Item { get; set; }
        public List<AIDetectorTask> DetectList { get; set; }
    }
    public class FrameContext
    {
        public string VideoKey { get; set; }
        public MkMediaT Media { get; set; }
        public MkDecoderT VideoDecoder { get; set; }
        public MkTrackT Track { get; set; }
        public MkSwscaleT Swscale { get; set; }
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

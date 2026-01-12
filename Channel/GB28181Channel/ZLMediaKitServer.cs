using ChannelUtility;
using GB28181Channel.GB28181;
using GB28181Channel.GB28181.DTO;
using GB28181Channel.GB28181.Event;
using GB28181Channel.GB28181.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ZLMediaKit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GB28181Channel
{
    public class ZLMediaKitServer
    {
        private IServiceProvider _provider;
        private GB28181Option _option;
        private GB28181DeviceEventListener _listener;
        private MkEvents _mkEvents;
        private ZLMediaKit.Delegates.Func_int___IntPtr___IntPtr _onMediaNotFoundDelegate;
        private ZLMediaKit.Delegates.Action___IntPtr _onMediaNoReaderDelegate;
        private ZLMediaKit.Delegates.Action_int___IntPtr _onMediaChangedDelegate;
        private GB28181Server _server;
        private ConcurrentDictionary<string, ChannelInfo> _mediaDict;
        private ConcurrentDictionary<string, FrameContext> _contextMap;
        private ConcurrentDictionary<string, IntPtr> _contextPtrMap;
        private static readonly Lazy<ZLMediaKitServer> _instance = new Lazy<ZLMediaKitServer>(() => new ZLMediaKitServer());
        public static ZLMediaKitServer Instance => _instance.Value;
        private ZLMediaKitServer()
        {
            _onMediaNotFoundDelegate = On_mk_media_not_found;
            _onMediaNoReaderDelegate = On_mk_media_no_reader;
            _onMediaChangedDelegate = On_mk_media_changed;
        }
        private int On_mk_media_not_found(IntPtr url,
                                IntPtr sock)
        {
            var url_info = (MkMediaInfoT)url;
            var streamId = mk_events_objects.MkMediaInfoGetStream(url_info);
            InMemoryDeviceStorage storage = (InMemoryDeviceStorage)_provider.GetService<IDeviceStorage>();
            var channelInfo = storage.GetChannelFrom(streamId);
            if (channelInfo == null)
            {
                return 1;
            }
            Task t = _server.StartActiveStream(channelInfo.DeviceId, channelInfo.ChannelId, _option.rtp_port);
            return 0;
        }
        private void On_mk_media_no_reader(IntPtr senderPtr)
        {
            var sender = (MkMediaSourceT)senderPtr;
            var streamId = mk_events_objects.MkMediaSourceGetStream(sender);
            InMemoryDeviceStorage storage = (InMemoryDeviceStorage)_provider.GetService<IDeviceStorage>();
            var channelInfo = storage.GetChannelFrom(streamId);
            if (channelInfo != null)
            {
                Task t = _server.StopActiveStream(channelInfo.DeviceId, channelInfo.ChannelId);
            }
        }

        private void On_mk_media_changed(int regist, IntPtr senderPtr)
        {
            MkMediaSourceT mediaSourceT = (MkMediaSourceT)senderPtr;
            string streamId = mk_events_objects.MkMediaSourceGetStream(mediaSourceT);
            if (_mediaDict.TryGetValue(streamId, out var media))
            {
                InMemoryDeviceStorage storage = (InMemoryDeviceStorage)_provider.GetService<IDeviceStorage>();
                if (regist == 1)
                {
                    FrameContext context = new FrameContext();
                    context.VideoKey = media.PushKey;
                    IntPtr contextPtr = CallbackHelper.WrapInstanceToIntPtr(context);
                    _contextPtrMap.TryAdd(context.VideoKey, contextPtr);
                    _contextMap.TryAdd(context.VideoKey, context);


                    MkIniT option = mk_util.MkIniCreate();
                    mk_util.MkIniSetOptionInt(option, "enable_mp4", 0);
                    mk_util.MkIniSetOptionInt(option, "enable_audio", 0);
                    mk_util.MkIniSetOptionInt(option, "enable_fmp4", 0);
                    mk_util.MkIniSetOptionInt(option, "enable_ts", 0);
                    mk_util.MkIniSetOptionInt(option, "enable_hls", 0);
                    mk_util.MkIniSetOptionInt(option, "enable_rtsp", 0);
                    mk_util.MkIniSetOptionInt(option, "enable_rtmp", 1);
                    mk_util.MkIniSetOptionInt(option, "add_mute_audio", 0);
                    mk_util.MkIniSetOptionInt(option, "auto_close", 0);
                    context.Media = mk_media.MkMediaCreate2("_defaultVhost_", "live", context.VideoKey, 0, option);
                    mk_util.MkIniRelease(option);
                }
                else
                {
                    if (_mediaDict.TryRemove(streamId, out ChannelInfo ch))
                    {
                        if (_contextPtrMap.TryRemove(ch.PushKey, out IntPtr contextPtr))
                        {
                            CallbackHelper.FreeInstancePtr(contextPtr);
                        }
                        if (_contextMap.TryRemove(ch.PushKey, out FrameContext handle))
                        {
                            if (handle.Media != null)
                            {
                                mk_media.MkMediaRelease(handle.Media);
                            }
                            FrameBufferPool.ClearCache(ch.PushKey);
                        }
                    }
                }
            }
        }

        public void BindSsrc(StreamPlayEventArgs e)
        {
            InMemoryDeviceStorage storage = (InMemoryDeviceStorage)_provider.GetService<IDeviceStorage>();
            var channels = storage.GetChannelsByDeviceId(e.Params.DeviceId);
            var channelInfo = channels.Where(x => x.ChannelId == e.Params.ChannelId).FirstOrDefault();
            if (channelInfo != null)
            {
                _mediaDict.AddOrUpdate(e.Params.Ssrc, _ => channelInfo, (x, y) => channelInfo);
            }
        }
        public void Start(GB28181Option option, IServiceProvider provider, GB28181DeviceEventListener listener, GB28181Server server)
        {
            _provider = provider;
            _option = option;
            _listener = listener;
            _server = server;

            _mediaDict = new ConcurrentDictionary<string, ChannelInfo>();
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


                _mkEvents = new MkEvents()
                {
                    OnMkMediaNotFound = _onMediaNotFoundDelegate,
                    OnMkMediaNoReader = _onMediaNoReaderDelegate,
                    OnMkMediaChanged = _onMediaChangedDelegate
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

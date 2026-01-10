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
using System.Text;
using System.Threading.Tasks;
using ZLMediaKit;

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

                }
                else
                {
                    _mediaDict.TryRemove(streamId, out ChannelInfo ch);
                }
            }
        }
        private ConcurrentDictionary<string, ChannelInfo> _mediaDict;
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
}

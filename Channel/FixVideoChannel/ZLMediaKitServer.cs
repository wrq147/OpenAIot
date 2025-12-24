using ChannelUtility;
using ChannelUtility.Message;
using FFmpeg.AutoGen;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Text;
using ZLMediaKit;

namespace FixVideoChannel
{
    public class ZLMediaKitServer
    {
        private IServiceProvider _provider;
        private FixVideoOption _option;
        private ConcurrentDictionary<string, MkProxyPlayerT> _players;
        private MkEvents _mkEvents;
        private static readonly Lazy<ZLMediaKitServer> _instance = new Lazy<ZLMediaKitServer>(() => new ZLMediaKitServer());
        public static ZLMediaKitServer Instance => _instance.Value;
        private ZLMediaKitServer(){}

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
        private unsafe  void On_mk_http_request(IntPtr parserPtr,
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
        private unsafe void On_mk_http_before_access(IntPtr parserPtr,
                                           sbyte* path,
                                           IntPtr sock)
        {
            var parser = (MkParserT)parserPtr;
            var sender = (MkSockInfoT)sock;

        }
        private void On_mk_rtsp_get_realm(IntPtr url,
                                         IntPtr invoker,
                                         IntPtr sock)
        {
   
            //rtsp播放默认鉴权
            mk_events_objects.MkRtspGetRealmInvokerDo((MkRtspGetRealmInvokerT)invoker, "zlmediakit");
        }
        private void On_mk_rtsp_auth(IntPtr url,
                                    string realm,
                                    string user_name,
                                    int must_no_encrypt,
                                    IntPtr invoker,
                                    IntPtr sock)
        {
            var url_info = (MkMediaInfoT)url;
            var sender = (MkSockInfoT)sock;

            //rtsp播放用户名跟密码一致
            mk_events_objects.MkRtspAuthInvokerDo((MkRtspAuthInvokerT)invoker, 0, user_name);
        }
        private void On_mk_record_mp4(IntPtr mp4Ptr)
        {
        }
        private void On_mk_shell_login(string user_name,
                                     string passwd,
                                     IntPtr invoker,
                                     IntPtr sock)
        {

            //允许登录shell
            mk_events_objects.MkAuthInvokerDo((MkAuthInvokerT)invoker, null);
        }
        private void On_mk_flow_report(IntPtr url,
                                      ulong total_bytes,
                                      ulong total_seconds,
                                      int is_player,
                                      IntPtr sock)
        {

        }
        public void AddPullProxy(VideoCaptureItem item)
        {
            MkIniT option = mk_util.MkIniCreate();
            mk_util.MkIniSetOptionInt(option, "enable_mp4", 0);
            mk_util.MkIniSetOptionInt(option, "enable_audio", 0);
            mk_util.MkIniSetOptionInt(option, "enable_fmp4", 0);
            mk_util.MkIniSetOptionInt(option, "enable_ts", 0);
            mk_util.MkIniSetOptionInt(option, "enable_hls", 0);
            mk_util.MkIniSetOptionInt(option, "enable_rtsp", 1);
            mk_util.MkIniSetOptionInt(option, "enable_rtmp", 1);
            //ZLM_API.mk_ini_set_option_int(option, "mp4_max_second", 3600);
            //！！非有必要，不要配置下面两个参数，否则会导致无法播放
            //ZLM_API.mk_ini_set_option(option,"mp4_save_path","D:/record");
            //ZLM_API.mk_ini_set_option(option,"hls_save_path","D:/record");
            mk_util.MkIniSetOptionInt(option, "add_mute_audio", 0);
            mk_util.MkIniSetOptionInt(option, "auto_close", 0);
            //创建拉流代理
            MkProxyPlayerT mk_proxy = mk_proxyplayer.MkProxyPlayerCreate4("__defaultVhost__", _option.zlmedia_server.App, item.PushKey, option, 3);
            //设置代理参数 rtp_type  rtsp播放方式:RTP_TCP = 0, RTP_UDP = 1, RTP_MULTICAST = 2
            mk_proxyplayer.MkProxyPlayerSetOption(mk_proxy, "rtp_type", "1");
            //设置代理参数 protocol_timeout_ms  协议超时时间 毫秒 更多参数参见mk_proxy_player_set_option注释
            mk_proxyplayer.MkProxyPlayerSetOption(mk_proxy, "protocol_timeout_ms", "2000");
            //如果是rtsp回放流支持配置开始倍速
            //ZLM_API.mk_proxy_player_set_option(mk_proxy, "rtsp_speed", "1.5");
            //开始播放代理地址
            mk_proxyplayer.MkProxyPlayerPlay(mk_proxy, item.PullAddr);
            _players.TryAdd(item.Id, mk_proxy);
            mk_util.MkIniRelease(option);
        }
        public void RemovePullProxy(string id)
        {
            if (_players.TryGetValue(id, out MkProxyPlayerT tmpt))
            {
                mk_proxyplayer.MkProxyPlayerRelease(tmpt);
            }
        }
        public void Start(FixVideoOption option, IServiceProvider provider)
        {
            _provider = provider;
            _option = option;
            _players = new ConcurrentDictionary<string, MkProxyPlayerT>();
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
                    OnMkHttpBeforeAccess = On_mk_http_before_access,
                    OnMkRtspGetRealm = On_mk_rtsp_get_realm,
                    OnMkRtspAuth = On_mk_rtsp_auth,
                    OnMkRecordMp4 = On_mk_record_mp4,
                    OnMkShellLogin = On_mk_shell_login,
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
}

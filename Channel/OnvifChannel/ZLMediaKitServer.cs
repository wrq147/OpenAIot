using ChannelUtility;
using ChannelUtility.Message;
using Microsoft.Extensions.DependencyInjection;
using Onvif.Core.Client.Camera;
using Onvif.Core.Client.Camera.Requests;
using Onvif.Core.Client.Common;
using Onvif.Core.Client.Device;
using Onvif.Core.Client.Media;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using ZLMediaKit;

namespace OnvifChannel
{
    public unsafe class ZLMediaKitServer
    {
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
        private ConcurrentDictionary<string, string> _IdToKeys;
        private ConcurrentDictionary<string, FrameContext> _contextMap;
        private ConcurrentDictionary<string, IntPtr> _contextPtrMap;
        private ConcurrentDictionary<string, VideoData> _videoKeyItems;
        private static readonly Lazy<ZLMediaKitServer> _instance = new Lazy<ZLMediaKitServer>(() => new ZLMediaKitServer());
        public static ZLMediaKitServer Instance => _instance.Value;
        private ZLMediaKitServer()
        {
            _cameraAccountDict = new ConcurrentDictionary<string, Account>();
            _IdToKeys = new ConcurrentDictionary<string, string>();
            _contextMap = new ConcurrentDictionary<string, FrameContext>();
            _contextPtrMap = new ConcurrentDictionary<string, IntPtr>();
            _videoKeyItems = new ConcurrentDictionary<string, VideoData>();

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
                if (_videoKeyItems.TryGetValue(context.VideoKey, out VideoData item))
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

            byte[] rgb24 = FrameBufferPool.GetRgb24Buffer(context.VideoKey, w, h);
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


                if (_videoKeyItems.TryGetValue(context.VideoKey, out VideoData item))
                {
                    if (context.Motion == null)
                    {
                        context.Motion = new MotionDetector();
                    }

                    context.Motion.CoolDownMs = item.CoolDownMs;
                    context.Motion.MotionBlockRatioThreshold = item.MotionRatio;
                    // 执行AI检测
                    var (isMotionDetected, motionRatio) = context.Motion.IsMotionKeyframe(rgb24, w, h);
                    if (isMotionDetected || item.NeedUp)
                    {
                        AIDetectorTask.Detect(item, w, h, motionRatio, _listener, rgb24);
                    }

                    // 执行绘制
                    var tmpboxlist = item.BoxList;
                    if (tmpboxlist != null && tmpboxlist.Count > 0)
                    {
                        AIDetectorTask.Draw(rgb24, w, h, tmpboxlist);
                    }


                    byte[] yuvData;
                    int[] yuvLineSizes;
                    int alignedLineSize = (w * 3 + 31) & ~31;

                    if (!ZLUtility.ConvertRgb24ToTargetYuv(rgb24, w, h, alignedLineSize, (AVPixelFormat)pixFmt, out yuvData, out yuvLineSizes))
                    {
                        return;
                    }
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
                            if (context.Media == null)
                            {
                                return;
                            }
                            mk_media.MkMediaInputYuv(context.Media, yuvPlanes, yuvLineSizes, (ulong)lpts);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
            if (regist == 1)
            {
                if (_cache.TryGetValue<PlaybackParams>(streamId, out var playbackParams))
                {
                    _mediaDict.AddOrUpdate(streamId, _ => playbackParams, (x, y) => playbackParams);
                    var storage = _provider.GetService<IDeviceStorage>();
                    var channelList = storage.GetChannelsByDeviceId(playbackParams.DeviceId);
                    var channelInfo = channelList.Where(x => x.ChannelId == playbackParams.ChannelId).FirstOrDefault();
                    if (channelInfo == null)
                    {
                        return;
                    }
                    if (_contextMap.ContainsKey(channelInfo.PushKey))
                    {
                        return;
                    }

                    FrameContext context = new FrameContext();
                    context.SourceMedia = mediaSourceT;
                    IntPtr contextPtr = CallbackHelper.WrapInstanceToIntPtr(context);
                    context.VideoKey = channelInfo.PushKey;
                    context.DeviceId = channelInfo.DeviceId;
                    context.ChannelId = channelInfo.ChannelId;
                    _contextPtrMap.TryAdd(context.VideoKey, contextPtr);
                    _contextMap.TryAdd(context.VideoKey, context);

                    //创建实时拉流
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

                    int trackCount = mk_events_objects.MkMediaSourceGetTrackCount(mediaSourceT);
                    for (int i = 0; i < trackCount; i++)
                    {
                        MkTrackT mkTrack = mk_events_objects.MkMediaSourceGetTrack(mediaSourceT, i);
                        if (mkTrack == null) { continue; }

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

                            mk_transcode.MkDecoderSetCb(mkDecoder, _onDecodeFrameDelegate, contextPtr);
                            mk_track.MkTrackAddDelegate(mkTrack, _onParseFrameDelegate, contextPtr);

                        }
                        else
                        {
                            mk_media.MkMediaInitTrack(context.Media, mkTrack);
                        }
                    }
                    mk_media.MkMediaInitComplete(context.Media);
                }
                else
                {
                    if (_recordContexts.TryRemove(streamId, out RecordContext tmprec))
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
            }
            else
            {
                if (_mediaDict.TryGetValue(streamId, out var playbackParams))
                {
                    _mediaDict.TryRemove(streamId, out PlaybackParams ch);
                    var storage = _provider.GetService<IDeviceStorage>();
                    var channelList = storage.GetChannelsByDeviceId(playbackParams.DeviceId);
                    var channelInfo = channelList.Where(x => x.ChannelId == playbackParams.ChannelId).FirstOrDefault();
                    if (channelInfo == null)
                    {
                        return;
                    }

                    if (_contextMap.TryRemove(channelInfo.PushKey, out FrameContext context))
                    {
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
                        if (context.Media != null)
                        {
                            mk_media.MkMediaRelease(context.Media);
                            context.Media = null;
                        }
                        FrameBufferPool.ClearCache(channelInfo.PushKey);
                    }
                    if (_contextPtrMap.TryRemove(channelInfo.PushKey, out IntPtr contextPtr))
                    {
                        CallbackHelper.FreeInstancePtr(contextPtr);
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
            bool enablePublish = false;
            if (_cache.TryGetValue<PlaybackParams>(streamId, out var playbackParams))
            {
                enablePublish = true;
            }
            else
            {
                if (_mediaDict.ContainsKey(streamId))
                {
                    enablePublish = true;
                }
            }
            if (enablePublish)
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
            else
            {
                mk_events_objects.MkPublishAuthInvokerDo2((MkPublishAuthInvokerT)invoker, "无发布权限，中断推流", null);
            }
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
            InMemoryDeviceStorage storage = (InMemoryDeviceStorage)_provider.GetService<IDeviceStorage>();
            var channelInfo = storage.GetChannelFrom(streamId);
            if (channelInfo == null)
            {
                return;
            }
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
            InMemoryDeviceStorage storage = (InMemoryDeviceStorage)_provider.GetService<IDeviceStorage>();
            var channelInfo = storage.GetChannelFrom(stream);
            if (channelInfo == null)
            {
                return;
            }
            var device = storage.GetDevice(channelInfo.DeviceId);
            if (device == null)
            {
                return;
            }
            if (device.VideoData == null || device.VideoData.Item == null)
            {
                return;
            }
            _ = _listener.OnSendRecordFile(device.VideoData.Item.Id, stream, fileName, fileSize, startTime, timeLen, channelInfo.StorageWay, 0);

            if (channelInfo.StorageWay == 1)
            {
                string upfilePosition = $"{stream}/{startTime.ToString("yyyy-MM-dd")}/{fileName}";
                _provider.GetService<MinioHelper>().UploadFile(filePath, upfilePosition);
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
            InMemoryDeviceStorage storage = (InMemoryDeviceStorage)_provider.GetService<IDeviceStorage>();
            var channelInfo = storage.GetChannelFrom(stream);
            if (channelInfo == null)
            {
                return;
            }
            var device = storage.GetDevice(channelInfo.DeviceId);
            if (device == null)
            {
                return;
            }
            if (device.VideoData == null || device.VideoData.Item == null)
            {
                return;
            }

            var storageWay = channelInfo.StorageWay;
            var videoId = device.VideoData.Item.Id;
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
        private void On_mk_flow_report(IntPtr url,
                                      ulong total_bytes,
                                      ulong total_seconds,
                                      int is_player,
                                      IntPtr sock)
        {

        }
        public void UpdateCamera(VideoData data)
        {
            RemoveCamera(data.Item.Id);
            Account account = new Account(data.Item.PullAddr, data.Item.UserName, data.Item.Password);
            _cameraAccountDict.TryAdd(data.Item.Id, account);
            var camera = MyCamera.Create(account, ex =>
            {
                Console.WriteLine(ex.Message);
            });

            var profiles = camera.Media.GetProfilesAsync().Result;


            var eventBus = _provider.GetService<ClientBusProxy>();
            foreach (var profile in profiles.Profiles)
            {
                //var streamSetup = new StreamSetup
                //{
                //    // 流类型：单播（最常用）
                //    Stream = StreamType.RTPUnicast,
                //    Transport = new Transport
                //    {
                //        Protocol = TransportProtocol.RTSP,
                //    }
                //};
                //var streamUri = await this.Media.GetStreamUriAsync(streamSetup, profile.token);
                //Console.WriteLine($"RTSP流地址：{streamUri.Uri}");
            }
            _IdToKeys.TryAdd(data.Item.Id, data.Item.PushKey);

            FrameContext context = new FrameContext();
            context.VideoKey = data.Item.PushKey;
            IntPtr contextPtr = CallbackHelper.WrapInstanceToIntPtr(context);
            _contextPtrMap.TryAdd(context.VideoKey, contextPtr);
            _contextMap.TryAdd(context.VideoKey, context);
            _videoKeyItems.TryAdd(data.Item.PushKey, data);

        }
        public void RemoveCamera(string id)
        {
            if (_cameraAccountDict.TryRemove(id, out Account tmpCameraAccount))
            {
                if (_IdToKeys.TryRemove(id, out string tkey))
                {
                    if (_contextMap.TryRemove(tkey, out FrameContext handle))
                    {
                        handle.CanParse = false;
                        Thread.Sleep(200);
                        if (handle.Media != null)
                        {
                            mk_media.MkMediaRelease(handle.Media);
                            handle.Media = null;
                        }
                        FrameBufferPool.ClearCache(tkey);
                    }
                    _videoKeyItems.TryRemove(tkey, out VideoData tmpval);
                    if (_contextPtrMap.TryRemove(tkey, out IntPtr contextPtr))
                    {
                        CallbackHelper.FreeInstancePtr(contextPtr);
                    }
                }
                MyCamera.ReleaseCamera(tmpCameraAccount);
            }
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
    }

    public class FrameContext
    {
        public bool CanParse { get; set; } = true;
        public string VideoKey { get; set; }
        public MkMediaT Media { get; set; }
        public MkDecoderT VideoDecoder { get; set; }
        public MkSwscaleT Swscale { get; set; }
        public MotionDetector Motion { get; set; }
        // 上次触发时间
        public long LastTriggerTime { get; set; } = 0;
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

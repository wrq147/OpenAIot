using ChannelUtility;
using ChannelUtility.Message;
using Microsoft.Extensions.DependencyInjection;

namespace FixVideoChannel
{
    public class FixVideoDeviceEventListener : IVideoDeviceEventListener
    {
        private IServiceProvider _serviceProvider;
        public FixVideoDeviceEventListener(IServiceProvider provider)
        {
            _serviceProvider = provider;
        }
        public async Task OnSendRecordFile(string videoId, string videoKey, string fileName, ulong fileSize, ulong startTime, float timeLen, byte storage, byte saveType)
        {
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            await eventBus.PublishRecordFile(videoId, videoKey, fileName, fileSize, startTime, timeLen, storage, saveType);
        }


        public async Task OnEventOffline(VideoCaptureItem item)
        {
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            await eventBus.Disconnect(item.Id);
            ZLMediaKitServer.Instance.RemovePullProxy(item.Id);
        }

        public async Task OnEventOnline(VideoCaptureItem item)
        {
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            await eventBus.Connected(item.Id);
        }

        public void OnSendAIDetectRequest(string videoId, string videoKey, float motionRatio, byte[] pressData, int width, int height, List<AIConfigData> confs)
        {
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            eventBus.PublishAIDetectRequest(videoId, videoKey, motionRatio, pressData, width, height, confs);
        }

        public async Task OnDeviceDownMessage(BaseDeviceMessage msg)
        {
            if (msg is AIDetectResponseMessage aiResponse)
            {
                ZLMediaKitServer.Instance.UpdateAIDraw(aiResponse.DeviceId, aiResponse.BoxList, aiResponse.NeedConf);
            }
            else if (msg is MediaItemMessage upItemResponse)
            {
                VideoData videoData = new VideoData();
                videoData.NeedUp = true;
                videoData.Item = upItemResponse.Item;
                videoData.Configs = new List<AIConfigData>();
                videoData.NeedUp = true;
                foreach (var it in upItemResponse.Config.Tasks)
                {
                    videoData.Configs.Add(new AIConfigData()
                    {
                        DetType = it.Code,
                        DetParams = it.paramValues
                    });
                }
                videoData.CoolDownMs = upItemResponse.Config.CoolDownMs;
                videoData.MotionRatio = upItemResponse.Config.MotionRatio;
                ZLMediaKitServer.Instance.AddPullProxy(videoData);
            }
            else if (msg is MediaDelItemMessage delItemResponse)
            {
                ZLMediaKitServer.Instance.RemovePullProxy(delItemResponse.DeviceId);
            }
            else if (msg is MediaRecordStartMessage startRec)
            {
                ZLMediaKitServer.Instance.RecorderStart(startRec, 0, (rs, err) =>
                {
                    var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                    _ = eventBus.PublishRecordStartReply(startRec.MessageId, startRec.DeviceId, rs, err);
                });
            }
            else if (msg is MediaRecordStopMessage stopRec)
            {
                ZLMediaKitServer.Instance.RecorderStop(stopRec, (rs, err) =>
                {
                    var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                    _ = eventBus.PublishRecordStopReply(msg.MessageId, msg.DeviceId, rs, err);
                });
            }
            else if (msg is MediaRecordCleanMessage cleanRec)
            {
                //删除文件
                var streamIds = cleanRec.Records.Select(x => x.StreamId).Distinct().ToList();
                var dates = cleanRec.Records.Select(x => x.Date).ToList();
                foreach (var tstreamId in streamIds)
                {
                    foreach (var tdate in dates)
                    {
                        string tMp4Path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "www" + Path.DirectorySeparatorChar + "record" + Path.DirectorySeparatorChar + "live" + tstreamId + Path.DirectorySeparatorChar + tdate;
                        if (Directory.Exists(tMp4Path))
                        {
                            Directory.Delete(tMp4Path, true);
                        }

                        string tHlsPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "www" + Path.DirectorySeparatorChar + "live" + tstreamId + Path.DirectorySeparatorChar + tdate;
                        if (Directory.Exists(tHlsPath))
                        {
                            Directory.Delete(tHlsPath, true);
                        }
                    }
                }

                //删除mino中的文件
                foreach (var rec in cleanRec.Records)
                {
                    string upfilePosition = $"{rec.StreamId}/{rec.Date}/{rec.FileName}";
                    await _serviceProvider.GetService<MinioHelper>().RemoveFile(upfilePosition);
                }
            }
        }

    }
}

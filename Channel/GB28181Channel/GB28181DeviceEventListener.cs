using ChannelUtility;
using ChannelUtility.Message;
using GB28181Channel.GB28181;
using GB28181Channel.GB28181.DTO;
using GB28181Channel.GB28181.Event;
using GB28181Channel.GB28181.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel
{
    public class GB28181DeviceEventListener
    {
        private IServiceProvider _serviceProvider;
        public GB28181DeviceEventListener(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task OnSendAIDetectRequest(string videoId, string videoKey, float motionRatio, byte[] pressData, int width, int height, List<AIConfigData> confs)
        {
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            await eventBus.PublishAIDetectRequest(videoId, videoKey, motionRatio, pressData, width, height, confs);
        }
        public async Task OnSendRecordFile(string videoId, string videoKey, string fileName, ulong fileSize, ulong startTime, float timeLen, byte storage, byte saveType)
        {
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            await eventBus.PublishRecordFile(videoId, videoKey, fileName, fileSize, startTime, timeLen, storage, saveType);
        }

        public async Task OnDeviceDownMessage(BaseDeviceMessage msg, GB28181Server server)
        {
            if (msg is AIDetectResponseMessage aiResponse)
            {
                var storage = _serviceProvider.GetService<IDeviceStorage>() as InMemoryDeviceStorage;
                var deviceId = storage.GetDeviceIdByDtuId(aiResponse.DeviceId);
                if (deviceId == null)
                {
                    return;
                }
                var device = storage.GetDevice(deviceId);
                if (device == null)
                {
                    return;
                }
                if (device.VideoData == null)
                {
                    return;
                }
                if (aiResponse.NeedConf == true)
                {
                    device.VideoData.NeedUp = true;
                }
                else
                {
                    device.VideoData.BoxList = aiResponse.BoxList;
                }
            }
            else if (msg is MediaItemMessage upItemResponse)
            {
                var storage = _serviceProvider.GetService<IDeviceStorage>();

                VideoData videoData = new VideoData();
                videoData.Item = upItemResponse.Item;
                videoData.Configs = new List<AIConfigData>();
                videoData.NeedUp = true;
                foreach (var it in upItemResponse.Config.Tasks)
                {
                    videoData.Configs.Add(new AIConfigData()
                    {
                        DetType = it.Code,
                        IsDraw = it.EnableDraw,
                        DetParams = it.paramValues
                    });
                }
                videoData.CoolDownMs = upItemResponse.Config.CoolDownMs;
                videoData.MotionRatio = upItemResponse.Config.MotionRatio;


                storage.UpdateDeviceMediaInfo(upItemResponse.Item.UserName, videoData);
                var newdevice = storage.GetDevice(upItemResponse.Item.UserName);
                if (newdevice == null)
                {
                    Console.WriteLine($"[异常] 设备{upItemResponse.Item.UserName}不存在");
                    return;
                }

                var device = storage.GetDevice(upItemResponse.Item.UserName);
                if (device == null)
                {
                    return;
                }
                await server.SendCatalogQuery(device);

                var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                await eventBus.Connected(upItemResponse.Item.Id, newdevice.DeviceIp);
            }
            else if (msg is MediaPTZMessage ptzMessage)
            {
                var storage = _serviceProvider.GetService<IDeviceStorage>();
                var channelList = storage.GetChannelsByDeviceId(ptzMessage.UserName);
                if (channelList.Count == 0)
                {
                    return;
                }
                var channinfo = channelList.Where(x => x.PushKey == ptzMessage.VideoKey).FirstOrDefault();
                if (channinfo == null)
                {
                    return;
                }

                await server.SendPTZControl(new GB28181.DTO.PTZControlParams()
                {
                    DeviceId = ptzMessage.UserName,
                    ChannelId = channinfo.ChannelId,
                    MessageId = ptzMessage.MessageId,
                    CommandType = ptzMessage.CommandType,
                    Speed = ptzMessage.Speed,
                    PresetId = ptzMessage.PresetId
                });
            }
            else if (msg is MediaPresetMessage presetMessage)
            {
                var storage = _serviceProvider.GetService<IDeviceStorage>();
                var device = storage.GetDevice(presetMessage.UserName);
                if (device == null)
                {
                    return;
                }
                if (device.PresetList != null)
                {
                    var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                    await eventBus.PublishMediaPresetReply(presetMessage.MessageId, presetMessage.DeviceId, presetMessage.UserName, device.PresetList);
                }
                else
                {
                    await server.GetPresetList(presetMessage.UserName, presetMessage.MessageId);
                }
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

        /// <summary>
        /// 处理设备注册事件
        /// </summary>
        /// <param name="e"></param>
        public async Task OnDeviceRegistered(object? sender, DeviceRegisteredEventArgs e)
        {
            var option = _serviceProvider.GetService<IOptions<GB28181Option>>().Value;
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            eventBus.PublishMediaNotFound(e.Device.DeviceId, 1);
        }

        /// <summary>
        /// 处理设备离线事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task OnDeviceOffline(object? sender, DeviceOfflineEventArgs e)
        {
            var storage = _serviceProvider.GetService<IDeviceStorage>();
            var device = storage.GetDevice(e.DeviceId);
            if (device != null && device.VideoData != null && !string.IsNullOrEmpty(device.VideoData.Item.Id))
            {
                //停止视频
                var channelList = storage.GetChannelsByDeviceId(e.DeviceId);
                foreach (var channel in channelList)
                {
                    ZLMediaKitServer.Instance.CloseMediaSource(channel.PushKey);
                }

                var option = _serviceProvider.GetService<IOptions<GB28181Option>>().Value;
                var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                await eventBus.Disconnect(device.VideoData.Item.Id);
                eventBus.PublishMediaNotReader(device.DeviceId, 1);
            }
        }
        public async Task OnPresetListReceived(object? sender, PresetListReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.MessageId))
            {
                return;
            }
            var storage = _serviceProvider.GetService<IDeviceStorage>();
            var device = storage.GetDevice(e.DeviceId);
            if (device == null)
            {
                return;
            }
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            await eventBus.PublishMediaPresetReply(e.MessageId, device.VideoData.Item.Id, e.DeviceId, device.PresetList);
        }
        public async Task OnPTZEventOk(object? sender, PTZEventOkArgs e)
        {
            if (string.IsNullOrEmpty(e.MessageId))
            {
                return;
            }
            var storage = _serviceProvider.GetService<IDeviceStorage>();
            var device = storage.GetDevice(e.DeviceId);
            if (device == null)
            {
                return;
            }
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            await eventBus.PublishMediaPTZReply(e.MessageId, device.VideoData.Item.Id, e.IsSuccess, e.Reason);
        }
        public async Task OnStreamPlay(object? sender, StreamPlayEventArgs e)
        {
            if (e.IsSuccess == true)
            {
                ZLMediaKitServer.Instance.BindSsrc(e);
            }
        }
    }
}

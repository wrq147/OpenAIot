using ChannelUtility;
using ChannelUtility.Message;
using Microsoft.Extensions.DependencyInjection;
using Onvif.Core.Client.Camera;
using Onvif.Core.Client.Common;
using Onvif.Core.Client.Device;
using Onvif.Core.Client.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OnvifChannel
{
    public class OnvifDeviceEventListener
    {
        private IServiceProvider _serviceProvider;
        public OnvifDeviceEventListener(IServiceProvider provider)
        {
            _serviceProvider = provider;
        }
        public async Task OnSendRecordFile(string videoId, string videoKey, string fileName, ulong fileSize, ulong startTime, float timeLen, byte storage, byte saveType)
        {
            try
            {
                var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                await eventBus.PublishRecordFile(videoId, videoKey, fileName, fileSize, startTime, timeLen, storage, saveType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public void OnSendAIDetectRequest(string videoId, string videoKey, float motionRatio, byte[] pressData, int width, int height, List<AIConfigData> confs, byte dataType)
        {
            try
            {
                var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                eventBus.PublishAIDetectRequest(videoId, videoKey, motionRatio, pressData, width, height, confs, dataType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task OnEventOnline(VideoCaptureItem item)
        {
            try
            {
                var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                await eventBus.Connected(item.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task OnEventOffline(VideoCaptureItem item)
        {
            try
            {
                ZLMediaKitServer.Instance.RemoveCamera(item.Id);
                var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                await eventBus.Disconnect(item.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async Task OnDeviceDownMessage(BaseDeviceMessage msg)
        {
            try
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
                            OrgId = upItemResponse.Config.OrgId,
                            DetParams = it.paramValues
                        });
                    }
                    videoData.CoolDownMs = upItemResponse.Config.CoolDownMs;
                    videoData.MotionRatio = upItemResponse.Config.MotionRatio;

                    ZLMediaKitServer.Instance.RemoveCamera(videoData.Item.Id);
                    var camera = ZLMediaKitServer.Instance.AddAccount(videoData);
                    var profiles = await camera.Media.GetProfilesAsync();
                    var firstPro = profiles.Profiles.FirstOrDefault();
                    if (firstPro == null)
                    {
                        return;
                    }
                    videoData.token = firstPro.token;
               
                    var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                    var streamSetup = new StreamSetup
                    {
                        // 流类型：单播（最常用）
                        Stream = StreamType.RTPUnicast,
                        Transport = new Transport
                        {
                            Protocol = TransportProtocol.RTSP,
                        }
                    };
                    var streamUri = await camera.Media.GetStreamUriAsync(streamSetup, firstPro.token);
                    videoData.url = streamUri.Uri;
                    ZLMediaKitServer.Instance.UpdateCamera(videoData);
                }
                else if (msg is MediaPTZMessage ptzMessage)
                {
                    var camera = ZLMediaKitServer.Instance.GetCamera(ptzMessage.DeviceId);
                    if (camera != null)
                    {
                    }
                }
                else if (msg is MediaPresetMessage presetMessage)
                {

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
                    var fileRecs = cleanRec.Records.Where(x => x.Storage == 0);
                    var streamIds = fileRecs.Select(x => x.StreamId).Distinct().ToList();
                    var dates = fileRecs.Select(x => x.Date).ToList();
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
                    var minoRecs = cleanRec.Records.Where(x => x.Storage == 1);
                    foreach (var rec in minoRecs)
                    {
                        string upfilePosition = $"{rec.StreamId}/{rec.Date}/{rec.FileName}";
                        await _serviceProvider.GetService<MinioHelper>().RemoveFile(upfilePosition);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}

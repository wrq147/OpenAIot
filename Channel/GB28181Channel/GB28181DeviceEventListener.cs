using ChannelUtility;
using ChannelUtility.Message;
using GB28181Channel.GB28181;
using GB28181Channel.GB28181.Event;
using GB28181Channel.GB28181.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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
        public async Task OnSendAIDetectRequest(string videoId, AIDetectItem item, byte[] pressData, int width, int height)
        {
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            await eventBus.PublishAIDetectRequest(videoId, item.Code, item.paramValues, item.EnableDraw, pressData, width, height);
        }

        public async Task OnDeviceDownMessage(BaseDeviceMessage msg, GB28181Server server)
        {
            if (msg is MediaItemMessage upItemResponse)
            {
                var storage = _serviceProvider.GetService<IDeviceStorage>();

                VideoData videoData = new VideoData();
                videoData.Item = upItemResponse.Item;
                videoData.DetectList = new List<AIDetectorTask>();
                foreach (var it in upItemResponse.Config.Tasks)
                {
                    videoData.DetectList.Add(new AIDetectorTask(it));
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
                await server.SendCatalogQuery(device);

                var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                await eventBus.Connected(upItemResponse.Item.Id, newdevice.DeviceIp);
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
            eventBus.PublishMediaNotFound(option.sip_service_id, e.Device.DeviceId, 1);
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
            if (device != null && !string.IsNullOrEmpty(device.VideoData.Item.Id))
            {
                var option = _serviceProvider.GetService<IOptions<GB28181Option>>().Value;
                var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                await eventBus.Disconnect(device.VideoData.Item.Id);
                eventBus.PublishMediaNotReader(option.sip_service_id, device.DeviceId, 1);
            }
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

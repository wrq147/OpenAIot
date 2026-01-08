using ChannelUtility;
using ChannelUtility.Message;
using EasyNetQ;
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

        public async Task OnDeviceDownMessage(BaseDeviceMessage msg)
        {
            if (msg is MediaItemMessage upItemResponse)
            {
                var storage = _serviceProvider.GetService<IDeviceStorage>();
                storage.UpdateDeviceMediaInfo(upItemResponse.Item.UserName, upItemResponse.Item.Id, upItemResponse.Item.PushKey);
                var newdevice = storage.GetDevice(upItemResponse.Item.UserName);
                if (newdevice == null)
                {
                    Console.WriteLine($"[异常] 设备{upItemResponse.Item.UserName}不存在");
                    return;
                }
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
            if (device != null && !string.IsNullOrEmpty(device.DtuId))
            {
                var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                await eventBus.Disconnect(device.DtuId);
            }
        }
    }
}

using ChannelUtility;
using ChannelUtility.Message;
using GB28181Channel.GB28181;
using GB28181Channel.GB28181.Event;
using Microsoft.Extensions.DependencyInjection;
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

        }

        /// <summary>
        /// 处理设备注册事件
        /// </summary>
        /// <param name="e"></param>
        public void OnDeviceRegistered(object? sender, DeviceRegisteredEventArgs e)
        {
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            //eventBus.WaitPublishMediaUserVerify()
        }
        /// <summary>
        /// 处理设备离线事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnDeviceOffline(object? sender, GB28181.Event.DeviceOfflineEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

using ChannelUtility.Message;
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
    }
}

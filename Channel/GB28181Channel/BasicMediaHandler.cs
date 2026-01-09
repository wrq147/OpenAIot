using GB28181Channel.GB28181.DTO;
using GB28181Channel.GB28181.Interface;

namespace GB28181Channel
{
    public class BasicMediaHandler : IMediaHandler
    {
        public bool StartRtpReceiver(PlaybackParams @params)
        {
            return false;
        }

        public bool StopRtpReceiver(DeviceInfo device, ChannelInfo channel)
        {

            return false;
        }

     
    }
}

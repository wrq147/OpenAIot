using GB28181Channel.GB28181.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Interface
{
    /// <summary>
    /// 媒体处理接口
    /// </summary>
    public interface IMediaHandler
    {

        /// <summary>
        /// 启动RTP流接收
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        bool StartRtpReceiver(PlaybackParams @params);


        /// <summary>
        /// 停止RTP流接收
        /// </summary>
        /// <param name="device"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        bool StopRtpReceiver(DeviceInfo device, ChannelInfo channel);
    }
}

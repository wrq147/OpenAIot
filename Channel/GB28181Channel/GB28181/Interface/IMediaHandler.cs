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
        /// <param name="params">点播参数</param>
        /// <returns>会话ID</returns>
        string StartRtpReceiver(PlaybackParams @params);

        /// <summary>
        /// 停止RTP流接收
        /// </summary>
        /// <param name="sessionId">会话ID</param>
        /// <returns>是否成功</returns>
        bool StopRtpReceiver(string sessionId);

        /// <summary>
        /// 生成SDP内容
        /// </summary>
        /// <param name="params">点播参数</param>
        /// <returns>SDP字符串</returns>
        string GenerateSDP(PlaybackParams @params);
    }
}

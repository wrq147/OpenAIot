using ChannelUtility.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Event
{
    /// <summary>
    /// 预置位列表接收事件参数
    /// </summary>
    public class PresetListReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; }
        public string MessageId { get; set; }
        /// <summary>
        /// 预置位列表
        /// </summary>
        public List<PresetInfo> PresetList { get; set; } = new List<PresetInfo>();
    }
}

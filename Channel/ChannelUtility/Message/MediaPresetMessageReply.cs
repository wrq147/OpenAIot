using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class MediaPresetMessageReply : BaseDeviceMessage
    {
        public MediaPresetMessageReply()
        {
            MsgType = "MediaPresReply";
        }
        public string UserName { get; set; }
        public List<PresetInfo> Presets { get; set; }
    }

    /// <summary>
    /// 预置位信息
    /// </summary>
    public class PresetInfo
    {
        /// <summary>
        /// 预置位ID
        /// </summary>
        public string PresetId { get; set; }

        /// <summary>
        /// 预置位名称
        /// </summary>
        public string PresetName { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; }
    }
}

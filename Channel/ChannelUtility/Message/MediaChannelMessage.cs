using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class MediaChannelMessage : BaseDeviceMessage
    {
        public MediaChannelMessage()
        {
            MsgType = "MediaCH";
        }
        public List<ChannelData> Channels { get; set; }
        public int VideoType { get; set; }
        public string UserName { get; set; }
    }
    public class ChannelData
    {
        public int Index { get; set; }
        public string ChannelId { get; set; }
        public string Name { get; set; }
    }
}

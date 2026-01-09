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
        public List<string> ChannelIds { get; set; }
        public List<string> ChannelNames { get; set; }
        public string UserName { get; set; }
        public string NodeGuid { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class MediaPresetMessage : BaseDeviceMessage
    {
        public MediaPresetMessage()
        {
            MsgType = "MediaPres";
        }
        public string UserName { get; set; }
    }
  
}

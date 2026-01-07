using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class MediaUserVerifyMessage : BaseDeviceMessage
    {
        public MediaUserVerifyMessage()
        {
            MsgType = "MediaUser";
        }
        public string MessageId { get; set; }
    }
}

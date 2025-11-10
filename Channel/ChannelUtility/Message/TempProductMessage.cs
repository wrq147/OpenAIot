using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    /// <summary>
    /// 生成临时设备与产品关联缓存
    /// </summary>
    public class TempProductMessage : BaseUpDeviceMessage
    {
        public TempProductMessage()
        {
            MsgType = "TempProduct";
        }
    }
}

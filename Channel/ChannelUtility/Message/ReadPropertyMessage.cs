using System;
using System.Collections.Generic;
namespace ChannelUtility.Message
{
    /// <summary>
    /// 读取设备属性
    /// </summary>
    public class ReadPropertyMessage : RequestMessage
    {
        /// <summary>
        /// 可读取多个属性
        /// </summary>
        public List<string> Properties { get; set; }
        public ReadPropertyMessage()
        {
            MsgType = "ReadProperty";
        }
    }
}

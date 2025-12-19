using ChannelUtility.Tsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    /// <summary>
    /// 有匹配的modbus消息
    /// </summary>
    public class ModbusMatchMessage : BaseDeviceMessage
    {
        public ModbusMatchMessage()
        {
            MsgType = "ModMatch";
        }
        /// <summary>
        /// 匹配的modbus规则
        /// </summary>
        public List<ModbusMatch> MatchList { get; set; }
    }
}

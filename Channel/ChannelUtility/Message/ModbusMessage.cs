using System;

namespace ChannelUtility.Message
{
    public class ModbusMessage : RequestMessage
    {
        /// <summary>
        /// Modbus匹配规则名称
        /// </summary>
        public string MatchName { get; set; }
        public ModbusMessage()
        {
            MsgType = "MobusRequest";
        }
    }
}

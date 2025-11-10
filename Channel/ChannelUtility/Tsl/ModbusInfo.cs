using System;
using System.Collections.Generic;

namespace ChannelUtility.Tsl
{
    public class ModbusInfo
    {
        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; set; }
        /// <summary>
        /// 数据位
        /// </summary>
        public int DataBits { get; set; }
        /// <summary>
        /// 奇偶校验
        /// </summary>
        public string Parity { get; set; }
        /// <summary>
        /// 停止位
        /// </summary>
        public string StopBits { get; set; }
        /// <summary>
        /// Mode
        /// </summary>
        public string Mode { get; set; }
        /// <summary>
        /// 轮询周期时间（单位ms）
        /// </summary>
        public int PollTime { get; set; }
        /// <summary>
        /// 匹配规则
        /// </summary>
        public List<ModbusMatch> Matches { get; set; } = new List<ModbusMatch>();
    }
}

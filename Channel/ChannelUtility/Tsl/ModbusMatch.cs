using System;
using System.Collections.Generic;

namespace ChannelUtility.Tsl
{
    /// <summary>
    /// Modbus匹配规则
    /// </summary>
    public class ModbusMatch
    {
        /// <summary>
        /// 规则名称（唯一）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 从机地址
        /// </summary>
        public byte SlaveId { get; set; }
        /// <summary>
        /// 功能码
        /// </summary>
        public byte FuncCode { get; set; }
        /// <summary>
        /// 起始地址
        /// </summary>
        public ushort StartAddress { get; set; }
        public List<ModbusMatchItem> Items { get; set; } = new List<ModbusMatchItem>();
        /// <summary>
        /// 计算总字节数
        /// </summary>
        /// <returns></returns>
        public int GetByteLength()
        {
            int len = 0;
            foreach (var it in Items)
            {
                len += (it.GetBitLen() + 7) / 8;
            }
            return len;
        }
        /// <summary>
        /// 计算总位数
        /// </summary>
        /// <returns></returns>
        public int GetBitLength()
        {
            int len = 0;
            foreach (var it in Items)
            {
                len += it.GetBitLen();
            }
            return len;
        }
    }
}

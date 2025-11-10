using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Tsl
{
    public class ModbusMatchItem
    {
        /// <summary>
        /// 字节序：H大端、L小端、C字节
        /// </summary>
        public string ByteOrder { get; set; }
        /// <summary>
        /// 数据长度
        /// </summary>
        public string NumRegister { get; set; }
        public int GetBitLen()
        {
            if (NumRegister == "b")
            {
                return 1;
            }
            else
            {
                return int.Parse(NumRegister) * 8;
            }
        }
        public int GetRegisterLen()
        {
            if (NumRegister == "b")
            {
                return 1;
            }
            else
            {
                return int.Parse(NumRegister);
            }
        }
        /// <summary>
        /// 对应的标识符
        /// </summary>
        public string PropertyCode { get; set; }
    }
}

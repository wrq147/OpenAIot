using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class In_DebugText
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 下发文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 是否为Hex格式
        /// </summary>
        public bool IsHex { get; set; }
    }
}

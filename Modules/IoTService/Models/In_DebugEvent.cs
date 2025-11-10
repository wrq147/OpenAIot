using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{

    public class In_DebugEvent
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 事件标识
        /// </summary>
        public string EventId { get; set; }
        /// <summary>
        /// 输入的参数
        /// </summary>
        public Dictionary<string, object> Inputs { get; set; }
    }
}

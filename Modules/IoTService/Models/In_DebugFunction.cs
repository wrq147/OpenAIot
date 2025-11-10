using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class In_DebugFunction
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 功能标识
        /// </summary>
        public string FunctionId { get; set; }
        /// <summary>
        /// 输入的参数
        /// </summary>
        public Dictionary<string, object> Inputs { get; set; }
    }
}

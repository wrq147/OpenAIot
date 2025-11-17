using System;
using System.Collections.Generic;

namespace IoTService.Models
{
    public class In_SyncExeFunc
    {
        /// <summary>
        /// 设备的批次编号
        /// </summary>
        public string Number { get; set; }
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

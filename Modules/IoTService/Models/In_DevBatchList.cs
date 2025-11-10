using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class In_DevBatchList
    {
        /// <summary>
        /// 要查询的通讯编码数组
        /// </summary>
        public string[] DtuIds { get; set; }
        /// <summary>
        /// 要查询的属性标识符数组
        /// </summary>
        public string[] Codes { get; set; }
        /// <summary>
        /// 是否同时发送读取属性消息
        /// </summary>
        public bool needSend { get; set; }
        /// <summary>
        /// 是否需要显示同步到标签的属性
        /// </summary>

        public bool needTag { get; set; }
    }
}

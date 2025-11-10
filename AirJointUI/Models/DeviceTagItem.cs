using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.Models
{
    public class DeviceTagItem
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 标签标识
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标签值
        /// </summary>
        public object Value { get; set; }
    }
}

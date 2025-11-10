using ChannelUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class ExceptProperty: DeviceProperty
    {
        /// <summary>
        /// 异常检测类型：峰值spike、更改change、range范围、SRCNN
        /// </summary>
        public string ExceptType { get; set; }
    }
}

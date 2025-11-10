using ChannelUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class Out_BatchLive
    {
        /// <summary>
        /// 设备通讯编码
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 设备实时数据
        /// </summary>
        public List<DeviceProperty> PropertyList { get; set; }
    }
}

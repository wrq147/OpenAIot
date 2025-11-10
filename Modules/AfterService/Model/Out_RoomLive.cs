using ChannelUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class Out_RoomLive
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 第三方设备编码
        /// </summary>
        public string DeviceNumber { get; set; }
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

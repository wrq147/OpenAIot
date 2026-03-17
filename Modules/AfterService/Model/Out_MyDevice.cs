using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class Out_MyDevice
    {
        /// <summary>
        /// 第三方编码
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 通讯Id
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 设备协议Id
        /// </summary>
        public string ProtocolId { get; set; }
        /// <summary>
        /// 设备所属产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 设备所在房间
        /// </summary>
        public string RoomName { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double? Lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double? Lat { get; set; }
    }

    /// <summary>
    /// 设备功能描述
    /// </summary>
    public class DevFun
    {

    }

    public class DevProp
    {

    }
}

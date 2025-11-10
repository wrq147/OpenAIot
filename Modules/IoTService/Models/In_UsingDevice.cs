using System;
namespace IoTService.Models
{
    public class In_UsingDevice
    {
        /// <summary>
        /// 物联卡Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 绑定的设备Id
        /// </summary>
        public string devId { get; set; }
    }
}

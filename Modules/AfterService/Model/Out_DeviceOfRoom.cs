using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class Out_DeviceOfRoom
    {
        public string Id { get; set; }
        /// <summary>
        /// 来源组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 拥有者组织ID
        /// </summary>
        public long? OwnerOrgId { get; set; }
        /// <summary>
        /// 房间名称（多个,号分隔）
        /// </summary>
        public string RoomNames { get; set; }
        /// <summary>
        /// 设备唯一编号
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 设备通讯编码
        /// </summary>
        public string DeviceId { get; set; }
        public static Out_DeviceOfRoom DemoData()
        {
            Out_DeviceOfRoom info = new Out_DeviceOfRoom();
            info.Id = "示例ID";
            info.Name = "示例设备名称";
            info.RoomNames = "示例所在房间";
            info.DeviceNumber = "示例批次编号";
            info.OwnerOrgId = 2;
            info.OrgId = 2;
            info.DeviceId = "示例通讯编码";
            return info;
        }
    }
}

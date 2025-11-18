using MyAccess.DB.Attr;
using System;

namespace IoTService.Models
{
    [TableName("mz_iot_update")]
    public class Out_UpdateItem : MZ_IotUpdate
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 协议Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 协议名称
        /// </summary>
        public string ProductName { get; set; }
    }
}

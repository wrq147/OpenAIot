using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;

namespace ProducerService.Model
{
    [TableName("mz_iot_device")]
    public class T_IotDevice
    {
        /// <summary>
        /// 编码Id
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 来源组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 设备唯一编号
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 通讯Id
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 所属物联协议Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }
    }
}

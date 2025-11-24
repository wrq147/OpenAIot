using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace IoTService.Models
{
    public class In_SyncDevice
    {
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 所属协议Id
        /// </summary>
        public string ProtocalId { get; set; }
        /// <summary>
        /// 产品Id
        /// </summary>
        public string ProductId { get; set; } = "1";
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 规格编码
        /// </summary>
        public string SkuNumber { get; set; }
        /// <summary>
        /// 设备通讯编码
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 设备生产日期
        /// </summary>
        public DateTime? PD { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 标签列表
        /// </summary>
        [DataIgnore]
        public List<In_TagItem> Tags { get; set; }
    }
}

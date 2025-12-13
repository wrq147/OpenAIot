using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace IoTService.Models
{
    [TableName("mz_iot_device")]
    public class MZ_IotDevice
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
        /// 拥有者组织ID
        /// </summary>
        public long? OwnerOrgId { get; set; }
        /// <summary>
        /// 使用者组织ID（使用者为组织）
        /// </summary>
        public long? UseOrgId { get; set; }
        /// <summary>
        /// 当前使用者（使用者为个人）
        /// </summary>
        public long? UseUserId { get; set; }
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
        /// 所属协议Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 产品Id：为1表示物联产品
        /// </summary>
        public string MesProductId { get; set; }
        /// <summary>
        /// 协议名称
        /// </summary>
        [DataIgnore]
        public string ProductName { get; set; }
        /// <summary>
        /// 0为离线，1为在线，2为未初始化
        /// </summary>
        public byte? Online { get; set; }
        /// <summary>
        /// 运行状态
        /// </summary>
        public string DState { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 设备通讯编码
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double? Lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double? Lat { get; set; }
        /// <summary>
        /// 设备所在区域代码
        /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// 经纬度对应geohash码值(传入经纬度后由后台计算出此值）
        /// </summary>
        [JsonIgnore]
        public string GeoHash { get; set; }
        /// <summary>
        /// 设备创建时间
        /// </summary>
        [JsonConverter(typeof(OnlySeriaize))]
        public DateTime? CreateOn { get; set; }
        /// <summary>
        /// 最后在线时间
        /// </summary>
        public DateTime? LastOnline { get; set; }
        /// <summary>
        /// 协议版本
        /// </summary>
        public int? ProductVer { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 设备经过的拥有者组织路径
        /// </summary>
        public string OwnerOrgPath { get; set; }
        /// <summary>
        /// 设备所属处理节点索引
        /// </summary>
        public int? DeviceUpIdx { get; set; }
        /// <summary>
        /// 设备的关键词
        /// </summary>
        public string KeyWords { get; set; }
        /// <summary>
        /// 设备是否需要更新关键词
        /// </summary>
        public bool? NeedUpdateKey { get; set; }
        /// <summary>
        /// 信号强度
        /// </summary>
        public float? dBm { get; set; }
        /// <summary>
        /// 标签列表
        /// </summary>
        [DataIgnore]
        public List<In_TagItem> Tags { get; set; }
        /// <summary>
        /// 列表展示标签用
        /// </summary>
        [DataIgnore]
        public List<Out_TagItem> TagsView { get; set; }
        /// <summary>
        /// 是否存在报警
        /// </summary>
        [DataIgnore]
        public bool? HavWarn { get; set; }
        /// <summary>
        /// 当前拥有者组织
        /// </summary>
        [DataIgnore]
        public string OwnerOrgName { get; set; }
        /// <summary>
        /// 房间名称
        /// </summary>
        [DataIgnore]
        public string RoomName { get; set; }
        /// <summary>
        /// 区域代码地址名称
        /// </summary>
        [DataIgnore]
        public string AreaCodeName { get; set; }
    }
}

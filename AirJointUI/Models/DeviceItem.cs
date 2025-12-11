using System;
using System.Collections.Generic;
using System.Linq;
namespace AirJointUI.Models
{
    public class DeviceItem
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
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 设备第三方编号
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 所属产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 分组Id
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 0为离线，1为在线，2为未初始化
        /// </summary>
        public byte? Online { get; set; }
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
        public string GeoHash { get; set; }
        /// <summary>
        /// 设备创建时间
        /// </summary>
        public string CreateOn { get; set; }
        /// <summary>
        /// 最后在线时间
        /// </summary>
        public string LastOnline { get; set; }
        /// <summary>
        /// 产品版本
        /// </summary>
        public int? ProductVer { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 标签列表
        /// </summary>
        public List<DeviceTagItem> Tags { get; set; }
        /// <summary>
        /// 是否存在报警
        /// </summary>
        public bool? HavWarn { get; set; }
    }
}

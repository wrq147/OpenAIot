using MyAccess.DB.Attr;
using System.Text.Json.Serialization;

namespace FastTunnelServer
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
        /// 图片地址
        /// </summary>
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 当前使用者（使用者为个人）
        /// </summary>
        public long? UseUserId { get; set; }
        /// <summary>
        /// 设备唯一编号
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 设备规格编码
        /// </summary>
        public string SkuNumber { get; set; }
        /// <summary>
        /// 所属协议Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 分组Id
        /// </summary>
        public string GroupId { get; set; }
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
        /// 设备生产日期
        /// </summary>
        public DateTime? PD { get; set; }
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
        /// 出厂原价
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 设备创建时间
        /// </summary>
        public DateTime? CreateOn { get; set; }
        /// <summary>
        /// 最后在线时间
        /// </summary>
        public DateTime? LastOnline { get; set; }
        /// <summary>
        /// 固件版本
        /// </summary>
        public int? FirmwareVer { get; set; }
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
      
    }
}

using Common.Share;
using MyAccess.DB.Attr;
using System;

namespace IoTService.Models
{
    [TableName("mz_iot_card")]
    public class MZ_IotCard
    {
        /// <summary>
        /// 编码Id
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// ICCID号
        /// </summary>
        public string ICCID { get; set; }
        /// <summary>
        /// IMSI号
        /// </summary>
        public string IMSI { get; set; }
        /// <summary>
        /// 对应的手机号码
        /// </summary>
        public string MSISDN { get; set; }
        /// <summary>
        /// 卡来源：YiDong、SimBoss、Sohan、Unicom、Unknow
        /// </summary>
        public string CardFrom { get; set; }
        /// <summary>
        /// 网络限速值，单位：Kbps，4G上限为150Mbps(153600Kbps)
        /// </summary>
        public double? SpeedLimit { get; set; }
        /// <summary>
        /// 运营商
        /// </summary>
        public string Carrier { get; set; }
        /// <summary>
        /// SINGLE：单卡，POOL：流量池卡
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 流量池id，CardType为POOL的才有此字段
        /// </summary>
        public string CardPoolId { get; set; }
        /// <summary>
        /// 卡当前套餐名称
        /// </summary>
        public string RatePlanName { get; set; }
        /// <summary>
        /// 卡当前套餐ID
        /// </summary>
        public string RatePlanId { get; set; }
        /// <summary>
        /// 卡套餐大小, 单位M
        /// </summary>
        public double? TotalDataVolume { get; set; }
        /// <summary>
        /// 卡套餐用量
        /// </summary>
        public double? UsedDataVolume { get; set; }
        /// <summary>
        /// 卡套餐单位，false表示按流量（MB），true表示按次数
        /// </summary>
        public bool? UseCountAsVolume { get; set; }
        /// <summary>
        /// 激活时间
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime? ExpirationDate { get; set; }
        /// <summary>
        /// 最后一次同步时间
        /// </summary>
        public DateTime? LastSyncDate { get; set; }
        /// <summary>
        /// 使用中的设备Id
        /// </summary>
        public string UsingDevice { get; set; }
        /// <summary>
        /// 卡在运营商的状态, 测试中:testing、库存:inventory、待激活:pending-activation、已激活:activation、已停卡:deactivation、已销卡:retired
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 设备绑定时间
        /// </summary>
        public DateTime? UsingOn { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
        /// <summary>
        /// 绑定的设备名称
        /// </summary>
        [DataIgnore]
        public string UsingDeviceName { get; set; }
    }
}

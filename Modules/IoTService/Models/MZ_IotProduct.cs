using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;

namespace IoTService.Models
{
    /// <summary>
    /// 物联网产品
    /// </summary>
    [TableName("mz_iot_product")]
    public class MZ_IotProduct : BaseEntity
    {
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 所属品类ID
        /// </summary>
        public string ClassifiedId { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        [DataIgnore]
        public string ClassName { get; set; }
        /// <summary>
        /// 设备接入方式：mqtt、http、tcp，为空则无物联
        /// </summary>
        public string NetworkWay { get; set; }
        /// <summary>
        /// 告警通知方式
        /// </summary>
        public string NoticeWay { get; set; }
        /// <summary>
        /// 状态（0未发布、1已发布）
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 设备通信方式：WiFi、以太网、2G网络、3G网络、4G网络、5G网络、NB-IoT（多个,号隔开）
        /// </summary>
        public string PhysicsWay { get; set; }
        /// <summary>
        /// 历史数据存储配置Json
        /// </summary>
        public string StorageConfig { get; set; }
        /// <summary>
        /// 数据解释脚本
        /// </summary>
        public string InterScripts { get; set; }
        /// <summary>
        /// 物模型json
        /// </summary>
        public string ModelTSL { get; set; }
        /// <summary>
        /// 当前版本号
        /// </summary>
        public int? Version { get; set; }
        /// <summary>
        /// 最后一次发布时间
        /// </summary>
        public DateTime? PublicTime { get; set; }
        /// <summary>
        /// 画面监控报表
        /// </summary>
        public string MonitorReportToken { get; set; }
        /// <summary>
        /// 物模型变更时间
        /// </summary>
        public DateTime? TSLUpdated { get; set; }
        /// <summary>
        /// 所属分类路径
        /// </summary>
        [DataIgnore]
        [JsonIgnore]
        public string Path { get; set; }
    }
}

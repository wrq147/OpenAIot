using MyAccess.DB.Attr;
using System;

namespace IoTService.Models
{
    [TableName("mz_iot_update")]
    public class MZ_IotUpdate
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 状态：0待更新，1更新中，2失败（需要人工处理）
        /// </summary>
        public byte? Status { get; set; }
        /// <summary>
        /// 尝试更新次数
        /// </summary>
        public int? UpdateCount { get; set; }
        /// <summary>
        /// 更新失败原因
        /// </summary>
        public string UpdateErr { get; set; }
        /// <summary>
        /// 更新的目标版本
        /// </summary>
        public int? Version { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedOn { get; set; }
        /// <summary>
        /// 设备更新优先级（数字越低越优先）
        /// </summary>
        public int? Level { get; set; }
    }
}

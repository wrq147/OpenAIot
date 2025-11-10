using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;

namespace IoTService.Models
{
    /// <summary>
    /// 历史数据存储源
    /// </summary>
    [TableName("mz_iot_his_source")]
    public class MZ_IotHisSource : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 数据源名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据源类型：influx（默认）
        /// </summary>
        public string StorageType { get; set; }
        /// <summary>
        /// 历史数据存储配置Json
        /// </summary>
        public string StorageConfig { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}

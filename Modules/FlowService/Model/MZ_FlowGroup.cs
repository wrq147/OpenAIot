using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FlowService.Model
{
    /// <summary>
    /// 流程模板的分组信息
    /// </summary>
    [TableName("mz_flow_group")]
    public class MZ_FlowGroup
    {
        /// <summary>
        /// 主键
        /// </summary>
        [ID(true)]
        public long? Id { get; set; }
        /// <summary>
        /// 关联组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 分组名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 流程模板
        /// </summary>
        [DataIgnore]
        public List<MZ_FlowTemplate> Items { get; set; }
    }
}

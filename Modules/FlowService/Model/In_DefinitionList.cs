using System;
namespace FlowService.Model
{
    /// <summary>
    /// 流程定义列表过滤
    /// </summary>
    public class In_DefinitionList
    {
        /// <summary>
        /// 流程定义名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 所属分组
        /// </summary>
        public long? groupId { get; set; }
        /// <summary>
        /// 过滤组织
        /// </summary>
        public long? orgId { get; set; }
        /// <summary>
        /// 是否过滤掉嵌入式流程
        /// </summary>
        public bool? IsEmbed { get; set; }
    }
}

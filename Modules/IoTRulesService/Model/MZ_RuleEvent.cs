using MyAccess.DB.Attr;
using System;

namespace IoTRulesService.Model
{
    /// <summary>
    /// 规则执行事件
    /// </summary>
    [TableName("mz_rule_event")]
    public class MZ_RuleEvent
    {
        /// <summary>
        /// 规则执行实例Id
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 规则模板Id
        /// </summary>
        public long? RuleId { get; set; }
        /// <summary>
        /// 实例Json
        /// </summary>
        public string InstanceJson { get; set; }
        /// <summary>
        /// 规则流json
        /// </summary>
        public string StackJson { get; set; }
        /// <summary>
        /// 源数据Json
        /// </summary>
        public string SourceJson { get; set; }
        /// <summary>
        /// 触发节点索引
        /// </summary>
        public int? Index { get; set; }

    }
}

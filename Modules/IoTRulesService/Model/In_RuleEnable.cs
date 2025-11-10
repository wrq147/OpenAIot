using System;

namespace IoTRulesService.Model
{
    public class In_RuleEnable
    {
        /// <summary>
        /// 规则Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsEnable { get; set;}
    }
}

using System;

namespace IoTRulesService.Flow.Node
{
    public class SetPropNode : RuleBaseNode
    {
        public SetPropProps props { get; set; }
    }
    public class SetPropProps
    {
        /// <summary>
        /// 属性标识
        /// </summary>
        public string PropCode { get; set; }
        /// <summary>
        /// 赋值表达式
        /// </summary>
        public string Express { get; set; }
    }
}

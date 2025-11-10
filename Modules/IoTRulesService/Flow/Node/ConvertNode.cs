using System;

namespace IoTRulesService.Flow.Node
{
    public class ConvertNode : RuleBaseNode
    {
        public ConvertProps props { get; set; }
    }

    public class ConvertProps
    {
        /// <summary>
        /// 自定义转换脚本
        /// </summary>
        public string func { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Node
{
    public class CountNode : RuleBaseNode
    {
        public CountProps props { get; set; }
    }
    public class CountProps
    {
        /// <summary>
        /// 计数方式：0为次数、1为秒计数
        /// </summary>
        public int Way { get; set; }
        /// <summary>
        /// 聚合的字段
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 计数值
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 保留计数（事件触发后保留的数据量，默认为0）
        /// </summary>
        public int CountLen { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Node
{
    public class TagNode : RuleBaseNode
    {
        public TagProps props { get; set; }
    }
    public class TagProps
    {
        /// <summary>
        /// 目标类型：0为当前设备、1为选择设备
        /// </summary>
        public byte TargetType { get; set; }
        /// <summary>
        /// 目标设备Id
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 标签标识
        /// </summary>
        public string TagId { get; set; }
        /// <summary>
        /// 赋值表达式
        /// </summary>
        public string Express { get; set; }
    }
}

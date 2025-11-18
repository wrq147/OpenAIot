using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Node
{
    public class FuncNode : RuleBaseNode
    {
        public RewriteProps props { get; set; }
    }
    public class RewriteProps
    {
        /// <summary>
        /// 目标类型：0为当前设备、1为选择设备、2为选择协议
        /// </summary>
        public byte TargetType { get; set; }
        /// <summary>
        /// 目标设备Id、目标协议
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 功能标识
        /// </summary>
        public string FunctionId { get; set; }
        /// <summary>
        /// EventInput为false时，执行功能传的值
        /// </summary>
        public Dictionary<string,object> InputData { get; set; }
        /// <summary>
        /// 是否将输入数据同步给功能
        /// </summary>
        public bool EventInput { get; set; }
        /// <summary>
        /// 是否将执行功能的结果输出到下一节点
        /// </summary>
        public bool ReturnOutput { get; set; }
    }
}

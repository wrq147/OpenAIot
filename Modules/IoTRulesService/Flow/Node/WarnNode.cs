using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Node
{
    public class WarnNode : RuleBaseNode
    {
        public WarnProps props { get; set; }
    }
    public class WarnProps
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
        /// 执行的报警事件
        /// </summary>
        public string EventId { get; set; }
    }

}

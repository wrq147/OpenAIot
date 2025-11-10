using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Node
{
    /// <summary>
    /// 消息重定向节点
    /// </summary>
    public class RedirectNode : RuleBaseNode
    {
        public RedirectProps props { get; set; }
    }
    public class RedirectProps
    {
        /// <summary>
        /// 转发后的产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 转发后的设备通信Id为:原dtuId+suffix
        /// </summary>
        public string[] TargetDtuIdsList { get; set; }
        /// <summary>
        /// 属性消息、事件消息与转发的产品标识符映射
        /// </summary>
        public Dictionary<string, string> Maping { get; set; }
    }
}

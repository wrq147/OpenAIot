using System;

namespace FlowService.FlowNode
{
    /// <summary>
    /// 延时节点
    /// </summary>
    public class DelayNode : FlowBaseNode
    {
        public DelayProps props { get; set; }
    }
    public class DelayProps
    {
        public string type { get; set; }
        public int time { get; set; }
        public string unit { get; set; }
        public string dateTime { get; set; }
    }
}

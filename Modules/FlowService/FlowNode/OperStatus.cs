using System;

namespace FlowService.FlowNode
{
    /// <summary>
    /// 操作状态
    /// </summary>
    public class OperStatus
    {
        public OperStatus(string name,string key)
        {
            this.Name = name;
            this.Key = key;
        }
        public string Name { get; set; }
        public string Key { get; set; }
    }
}

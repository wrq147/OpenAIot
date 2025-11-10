using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowService.Model
{
    /// <summary>
    /// 表示用户节点
    /// 目前只有根节点与审批节点
    /// </summary>
    [TableName("mz_flow_node")]
    public class MZ_Flow_Node
    {
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 流程实例Id
        /// </summary>
        public long? FlowId { get; set; }
        public string StepId { get; set; }
        public string StepName { get; set; }
        public long? ParentId { get; set; }
        public bool Active { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }


        public string EventKey { get; set; }

        public bool EventPublished { get; set; }


        /// <summary>
        /// 结果
        /// </summary>
        public string Outcome { get; set; }
        /// <summary>
        /// 节点状态
        /// </summary>
        public NodeStatus Status { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        [DataIgnore]
        public List<MZ_Flow_ExtensionAttribute> ExtensionAttributes { get; set; }

        [DataIgnore]
        public object EventAction { get; set; }
        public MZ_Flow_ExtensionAttribute FindAttribute(string key)
        {
            if (ExtensionAttributes != null)
            {
                return ExtensionAttributes.Where(x => x.AttributeKey == key).FirstOrDefault();
            }
            return null;
        }

        public void RemoveAttribute(string key)
        {
            if (ExtensionAttributes != null)
            {
                ExtensionAttributes.RemoveAll(x => x.AttributeKey == key);
            }
        }
        /// <summary>
        /// 添加或修改属性
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void UpdateAttribute(string key, string value)
        {
            if (ExtensionAttributes != null)
            {
                var attr = ExtensionAttributes.Where(x => x.AttributeKey == key).FirstOrDefault();
                if (attr == null)
                {
                    attr = new MZ_Flow_ExtensionAttribute();
                    attr.ExecutionNodeId = this.Id;
                    attr.AttributeKey = key;
                    attr.FlowId = this.FlowId;
                    this.ExtensionAttributes.Add(attr);
                }
                attr.AttributeValue = value;
            }
        }
    }
    /// <summary>
    /// 节点状态
    /// </summary>
    public enum NodeStatus
    {
        /// <summary>
        /// 失败
        /// </summary>
        Failed = 0,
        /// <summary>
        /// 暂停
        /// </summary>
        Pending = 1,
        /// <summary>
        /// 运行中
        /// </summary>
        Running = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        Complete = 3,
        /// <summary>
        /// 取消
        /// </summary>
        Cancelled = 4,
        /// <summary>
        /// 等待事件
        /// </summary>
        WaitingForEvent = 5,
    }
}

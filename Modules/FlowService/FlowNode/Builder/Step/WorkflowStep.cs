using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder.Step
{
    public abstract class WorkflowStep
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int ParentIndex { get; set; }
        public string Type { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 节点是否支持持久化
        /// </summary>
        public bool PersistenceNode { get; set; }
        /// <summary>
        /// 节点所在索引
        /// </summary>
        public int Index { get; set; }
        public List<int> Children { get; set; } = new List<int>();
        public WorkflowStep()
        {
            PersistenceNode = true;
            Type = this.GetType().FullName;
        }
        public abstract Task<ExecutionResult> Run(StepExecutionContext context);
    }
}

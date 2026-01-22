using NPOI.HSSF.Record;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Builder.Step
{
    public class RuleflowStep
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int ParentIndex { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 节点所在索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 节点的激活状态
        /// </summary>
        public bool IsActive { get; set; }
        public List<int> Children { get; set; } = new List<int>();
        public RuleflowStep()
        {
            Type = this.GetType().FullName;
            IsActive = false;
        }
        public virtual Task Run(RuleExecutionContext context) { return Task.CompletedTask; }
    }
}

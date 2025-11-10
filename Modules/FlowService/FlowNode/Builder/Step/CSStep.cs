using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder.Step
{
    /// <summary>
    /// 抄送
    /// </summary>
    public class CSStep : WorkflowStep
    {
        /// <summary>
        /// 是否发起人可添加
        /// </summary>
        public bool ShouldAdd { get; set; }
        /// <summary>
        /// 抄送的目标用户
        /// </summary>
        public string AssignedPrincipal { get; set; }
        /// <summary>
        /// 表单权限
        /// </summary>
        public FormPermsItem[] FormPerms { get; set; }
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            string[] users = this.AssignedPrincipal.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (string u in users)
            {
                context.ExecutionPointer.UpdateAttribute(string.Format("CC/{0}/{1}", u, context.Workflow.OrgId), string.Empty);
            }
            return await ExecutionResult.Next();
        }
    }
}

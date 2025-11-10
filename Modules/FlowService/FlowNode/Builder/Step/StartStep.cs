using System;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder.Step
{
    public class StartStep : WorkflowStep
    {        
        /// <summary>
        /// 表单权限
        /// </summary>
        public FormPermsItem[] FormPerms { get; set; }
        public StartStep()
        {
            this.PersistenceNode = true;
        }
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            return await ExecutionResult.Next();
        }
    }
}

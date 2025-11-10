using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder.Step
{
    /// <summary>
    /// 终结流程
    /// </summary>
    public class EndStep : WorkflowStep
    {
        public EndStep()
        {
            this.PersistenceNode = false;
        }
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            return await ExecutionResult.End();
        }
    }
}

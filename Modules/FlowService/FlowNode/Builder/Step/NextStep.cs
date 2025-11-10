using System;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder.Step
{
    /// <summary>
    /// 什么都不做
    /// </summary>
    public class NextStep : WorkflowStep
    {
        public NextStep()
        {
            this.PersistenceNode = false;
        }
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            return await ExecutionResult.Next();
        }
    }
}

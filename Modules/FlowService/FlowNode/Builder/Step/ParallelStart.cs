using System;
using System.Threading.Tasks;
namespace FlowService.FlowNode.Builder.Step
{
    public class ParallelStart : WorkflowStep
    {
        public ParallelStart()
        {
            this.PersistenceNode = false;
        }
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            var res = new ExecutionResult();
            res.ActiveChildren = true;
            return await Task.FromResult(res);
        }
    }
}

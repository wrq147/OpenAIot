using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder.Step
{
    /// <summary>
    /// 并行节点
    /// </summary>
    public class ParallelStep : WorkflowStep
    {
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            var res = new ExecutionResult();
            res.Directive = ExecutionDirective.Next;
            res.ActiveChildren = true;
            res.Parallel = true;
            return await Task.FromResult(res);
        }
    }
}

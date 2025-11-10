using System;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder.Step
{
    public class GotoStep : WorkflowStep
    {
        public GotoStep()
        {
            this.PersistenceNode = false;
        }
        /// <summary>
        /// 跳转的目标StepId
        /// </summary>
        public string TargetId { get; set; }
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            var link = context.FindStep(TargetId);
            if (link < 0)
            {
                return await ExecutionResult.End();
            }

            int preidx = --link;
            if (preidx < 0)
            {
                context.ExcuteIndex = 0;
            }
            else
            {
                context.ExcuteIndex = preidx;
            }
            return await ExecutionResult.Next();
        }
    }
}

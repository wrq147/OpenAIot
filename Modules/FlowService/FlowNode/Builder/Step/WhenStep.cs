using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder.Step
{
    public class WhenStep : WorkflowStep
    {
        public WhenStep()
        {
            this.PersistenceNode = false;
        }
        /// <summary>
        /// 期望的值
        /// </summary>
        public string ExpectedOutcome { get; set; }
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            string parentId = context.StepNodes[context.Step.ParentIndex].Id;
            var parentNode = context.Workflow.ExecutionNodes.Where(x => x.StepId == parentId).FirstOrDefault();
            if (parentNode == null)
            {
                throw new Exception("when格式错误");
            }
            if (parentNode.Outcome != ExpectedOutcome)
            {
                return await ExecutionResult.Next();
            }

            var res = new ExecutionResult();
            res.ActiveChildren = true;
            return res;
        }
    }
}

using AuthService;
using Common.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder.Step
{
    /// <summary>
    /// 办理人执行
    /// </summary>
    public class OperatorStep : WorkflowStep
    {
        public OperatorStep()
        {
            this.PersistenceNode = false;
        }
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            if (context.ExecutionPointer.EventAction == null)
            {
                throw new Exception("事件参数错误");
            }
            var action = ((UserAction)context.ExecutionPointer.EventAction);
            var steplink = context.FindStep(context.ExecutionPointer.StepId);

            OperatorTask at = context.StepNodes[steplink] as OperatorTask;
            context.ExecutionPointer.UpdateAttribute(string.Format("User/{0}/{1}", action.User.UserId, context.Workflow.OrgId), "Y");

            return await ExecutionResult.Next();
        }
    }
}

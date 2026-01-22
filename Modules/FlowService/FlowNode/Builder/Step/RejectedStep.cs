using Common.DataAc;
using Common.EventBus;
using Common.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder.Step
{
    /// <summary>
    /// 拒绝节点
    /// </summary>
    public class RejectedStep : WorkflowStep
    {
        /// <summary>
        /// 判断是否直接结束
        /// </summary>
        public bool ToEnd = false;
        public RejectedStep()
        {
            this.PersistenceNode = false;
        }
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {

            string fromType;
            string fromNumber;
            if (context.InputParams.TryGetValue("@fromtype", out fromType) && context.InputParams.TryGetValue("@from", out fromNumber))
            {
                ActionChangeData ae = new ActionChangeData();
                ae.TargetName = fromType;
                ae.Action = "Update";
                ae.UpdateId = context.Executor;
                ae.OrgId = context.Workflow.OrgId.Value;
                ae.FlowId = context.Workflow.Id.Value;
                ae.conditions = new List<ActionCondition>();
                ae.conditions.Add(new ActionCondition()
                {
                    TargetField = "@Number",
                    Value = System.Text.Json.JsonSerializer.Serialize(fromNumber, MyDefaultTextJsonConfig.DefaultOptions),
                });

                ae.actions = new List<ActionInfo>();
                ae.actions.Add(new ActionInfo()
                {
                    TargetField = "@Reject"
                });

                var rsp = await BusUtility.Call("ChangeData", ae);
                if (rsp.IsSuccess())
                {
                    return await ExecutionResult.Next();
                }
                else
                {
                    throw new Exception(rsp.Message);
                }
            }

            return await ExecutionResult.Next();
        }
    }
}

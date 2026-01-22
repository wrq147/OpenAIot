using Common.DataAc;
using Common.EventBus;
using Common.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder.Step
{
    /// <summary>
    /// 执行添加、修改、删除指定数据库表的记录
    /// </summary>
    public class DataStep : WorkflowStep
    {
        public DataStep()
        {
            this.PersistenceNode = false;
        }
        public DataProps props { get; set; }

        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            ActionChangeData ae = new ActionChangeData();
            ae.TargetForm = props.targetform;
            ae.Action = props.action;
            ae.UpdateId = context.Executor;
            ae.OrgId = context.Workflow.OrgId.Value;
            ae.FlowId = context.Workflow.Id.Value;
            ae.conditions = new List<ActionCondition>();
            foreach (var condit in props.conditions)
            {
                ActionCondition accond = new ActionCondition();
                accond.TargetField = condit.TargetField;
                switch (condit.ValueType)
                {
                    case "Form":
                        object tmpval = context.GetFormObject(condit.Value);
                        if (tmpval == null)
                        {
                            throw new Exception($"{this.Name}条件数据不能为null");
                        }
                        accond.Value = System.Text.Json.JsonSerializer.Serialize(tmpval, MyDefaultTextJsonConfig.DefaultOptions);
                        break;
                    case "Const":
                        accond.Value = System.Text.Json.JsonSerializer.Serialize(condit.Value, MyDefaultTextJsonConfig.DefaultOptions);
                        break;
                }
                ae.conditions.Add(accond);
            }
            ae.actions = new List<ActionInfo>();
            foreach (var act in props.fields)
            {
                ActionInfo actionInfo = new ActionInfo();
                actionInfo.TargetField = act.TargetField;
                switch (act.ValueType)
                {
                    case "Form":
                        object tmpval = context.GetFormObject(act.Value);
                        if (tmpval == null)
                        {
                            throw new Exception($"{this.Name}执行数据不能为null");
                        }
                        actionInfo.Value = System.Text.Json.JsonSerializer.Serialize(tmpval, MyDefaultTextJsonConfig.DefaultOptions);
                        break;
                    case "Const":
                        actionInfo.Value = System.Text.Json.JsonSerializer.Serialize(act.Value, MyDefaultTextJsonConfig.DefaultOptions);
                        break;
                }
                ae.actions.Add(actionInfo);
            }
            if (ae.conditions.Count == 0)
            {
                throw new Exception($"流程节点[{context.Step.Name}]至少需要一条过滤条件");
            }
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
    }

}

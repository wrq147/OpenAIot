using Common.DataAc;
using Common.EventBus;
using Common.Share;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

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
                        accond.Value = JsonConvert.SerializeObject(tmpval);
                        break;
                    case "Const":
                        accond.Value = JsonConvert.SerializeObject(condit.Value);
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
                        actionInfo.Value = JsonConvert.SerializeObject(tmpval);
                        break;
                    case "Const":
                        actionInfo.Value = JsonConvert.SerializeObject(act.Value);
                        break;
                }
                ae.actions.Add(actionInfo);
            }
            if (ae.conditions.Count == 0)
            {
                throw new Exception($"流程节点[{context.Step.Name}]至少需要一条过滤条件");
            }
            var rsp = await BusUtility.Call("ChangeData", ae);
            if (rsp == null)
            {
                throw new Exception("执行流程超时");
            }


            var neval = rsp.GetResult<BusResponse<int>>();
            if (neval.IsSuccess())
            {
                return await ExecutionResult.Next();
            }
            else
            {
                throw new Exception(neval.Message);
            }
        }
    }

}

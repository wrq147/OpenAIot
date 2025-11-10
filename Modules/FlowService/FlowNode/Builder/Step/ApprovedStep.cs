using AuthService;
using Common.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace FlowService.FlowNode.Builder.Step
{
    /// <summary>
    /// 审批同意
    /// </summary>
    public class ApprovedStep : WorkflowStep
    {

        public ApprovedStep()
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

            ApprovalTask at = context.StepNodes[steplink] as ApprovalTask;
            if (at.Sign && action.User.UserId != 2)
            {
                if (string.IsNullOrEmpty(action.User.SignImg))
                {
                    throw new Exception("签名不能为空");
                }
            }

            if (at.mode == "NEXT")
            {
                //依次审批 （按选择顺序审批，每个人必须同意）
                string curuser = string.Format("User/{0}/{1}", action.User.UserId, context.Workflow.OrgId);
                string[] users = at.AssignedPrincipal.Split(',', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < users.Length; i++)
                {
                    string u = users[i];
                    var tmpattr = context.ExecutionPointer.FindAttribute(string.Format("User/{0}/{1}", u, context.Workflow.OrgId));
                    if (tmpattr == null)
                    {
                        context.ExecutionPointer.UpdateAttribute(string.Format("User/{0}/{1}", u, context.Workflow.OrgId), string.Empty);

                        var adminInfo = await context.ServiceProvider.GetService<UserDAL>().GetAdminById(long.Parse(u));
                        List<TargetUser> targets = new List<TargetUser>();
                        targets.Add(new TargetUser()
                        {
                            uid = adminInfo.Id.Value,
                            email = adminInfo.Email,
                            phone = adminInfo.Mobile
                        });

                        var nt = new NoticeEvent(2, targets.ToArray(), context.Workflow.notifyModel.types);
                        nt.OrgId = context.Workflow.OrgId.Value;
                        nt.TargetType = "Approval";
                        nt.TargetUrl = "/flowable/todo?id=" + context.ExecutionPointer.Id.Value;
                        nt.Label = context.Workflow.notifyModel.GetTitleOfParsed(context);
                        nt.Content = context.Workflow.FlowName + "需要您的审批，请尽快处理";
                        context.NoticeList.Add(nt);

                        //退出
                        var eventKey = context.ExecutionPointer.Id.ToString();
                        return await ExecutionResult.WaitForEvent(eventKey);
                    }
                    else if (string.IsNullOrEmpty(tmpattr.AttributeValue))
                    {
                        if (curuser == tmpattr.AttributeValue)
                        {
                            context.ExecutionPointer.UpdateAttribute(curuser, "Y");
                            continue;
                        }
                        else
                        {
                            throw new Exception("请按顺序执行审批");
                        }
                    }
                }
            }
            else if (at.mode == "AND")
            {
                //会签（可同时审批，每个人必须同意）
                context.ExecutionPointer.UpdateAttribute(string.Format("User/{0}/{1}", action.User.UserId, context.Workflow.OrgId), "Y");
                string[] users = at.AssignedPrincipal.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (string u in users)
                {
                    var tmpattr = context.ExecutionPointer.FindAttribute(string.Format("User/{0}/{1}", u, context.Workflow.OrgId));
                    if (string.IsNullOrEmpty(tmpattr.AttributeValue))
                    {
                        //退出
                        var eventKey = context.ExecutionPointer.Id.ToString();
                        return await ExecutionResult.WaitForEvent(eventKey);
                    }
                }
            }
            else if (at.mode == "OR")
            {
                //或签（有一人同意即可）
                context.ExecutionPointer.UpdateAttribute(string.Format("User/{0}/{1}", action.User.UserId, context.Workflow.OrgId), "Y");
            }


            return await ExecutionResult.Next();
        }
    }
}

using AuthService;
using Common.EventBus;
using FlowService.Business;
using MonitorService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace FlowService.FlowNode.Builder
{
    public class ApprovalTask : UserTask
    {
        /// <summary>
        /// 多人审批时审批方式
        /// </summary>
        public string mode { get; set; }
        /// <summary>
        /// 审批时限
        /// </summary>
        public ExamineTimeLimit timeLimit { get; set; }
        /// <summary>
        /// 审批人
        /// </summary>
        public string AssignedPrincipal { get; set; }
        /// <summary>
        /// 是否可自选
        /// </summary>
        public bool CanAdd { get; set; }
        /// <summary>
        /// 表单内联系人
        /// </summary>
        public string formUser { get; set; }
        /// <summary>
        /// 是否需要签名
        /// </summary>
        public bool Sign { get; set; }

        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            if (!context.ExecutionPointer.EventPublished)
            {
                //表单内联系人转到到审批人
                if (!string.IsNullOrEmpty(formUser))
                {
                    if (context.FormItems.TryGetValue(formUser, out object outval))
                    {
                        var selectedlist = outval as IList<object>;
                        if (selectedlist != null)
                        {
                            foreach (var selectUser in selectedlist)
                            {
                                var selectObj = selectUser as IDictionary<string, object>;
                                var jk = selectObj["id"];
                                if (jk == null)
                                {
                                    continue;
                                }
                                string newuid = jk.ToString();
                                if (string.IsNullOrEmpty(this.AssignedPrincipal))
                                {
                                    this.AssignedPrincipal = newuid;
                                }
                                else
                                {
                                    this.AssignedPrincipal = this.AssignedPrincipal + "," + newuid;
                                }
                            }
                        }
                    }
                }
                if (mode == "NEXT")
                {
                    string[] users = this.AssignedPrincipal.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (users.Length > 0)
                    {
                        string s = users[0];
                        context.ExecutionPointer.UpdateAttribute(string.Format("User/{0}/{1}", s, context.Workflow.OrgId), string.Empty);


                        var adminInfo = await context.ServiceProvider.GetService<UserDAL>().GetAdminById(long.Parse(s));
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
                    }

                }
                else
                {
                    string[] users = AssignedPrincipal.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in users)
                    {
                        context.ExecutionPointer.UpdateAttribute(string.Format("User/{0}/{1}", s, context.Workflow.OrgId), string.Empty);
                    }

                    long[] tmpxx = Array.ConvertAll(users, x => long.Parse(x));
                    var adminList = await context.ServiceProvider.GetService<UserDAL>().GetUserListByIds(tmpxx.ToList());
                    List<TargetUser> targets = new List<TargetUser>();
                    foreach (var adminInfo in adminList)
                    {
                        targets.Add(new TargetUser()
                        {
                            uid = adminInfo.Id.Value,
                            email = adminInfo.Email,
                            phone = adminInfo.Mobile
                        });
                    }
                    var nt = new NoticeEvent(2, targets.ToArray(), context.Workflow.notifyModel.types);
                    nt.OrgId = context.Workflow.OrgId.Value;
                    nt.TargetType = "Approval";
                    nt.TargetUrl = "/flowable/todo?id=" + context.ExecutionPointer.Id.Value;
                    nt.Label = context.Workflow.notifyModel.GetTitleOfParsed(context);
                    nt.Content = context.Workflow.FlowName + "需要您的审批，请尽快处理";
                    context.NoticeList.Add(nt);
                }

                //超时处理定时
                if (timeLimit.timeout.value > 0)
                {
                    DateTime overTime = DateTime.Now;
                    if (timeLimit.timeout.unit == "H")
                    {
                        overTime = overTime.AddHours(timeLimit.timeout.value);
                    }
                    else
                    {
                        overTime = overTime.AddDays(timeLimit.timeout.value);
                    }
                    string jobname = "ApprovalOver_" + context.ExecutionPointer.Id;
                    string group = "DEFAULT";
                    MZ_Job job = new MZ_Job();
                    job.concurrent = "0";
                    job.createId = 0;
                    job.create_time = DateTime.Now;
                    job.updateId = 0;
                    job.update_time = DateTime.Now;
                    job.cron_expression = string.Format("{0} {1} {2} {3} {4} ? {5}", overTime.Second, overTime.Minute, overTime.Hour, overTime.Day, overTime.Month, overTime.Year);
                    if (timeLimit.handler.type == "PASS")
                    {
                        //自动通过
                        job.invoke_target = typeof(TaskBLL).FullName + ".ScheduleExcute(L" + context.ExecutionPointer.Id + ",'同意')";
                    }
                    else if (timeLimit.handler.type == "REFUSE")
                    {
                        //自动驳回
                        job.invoke_target = typeof(TaskBLL).FullName + ".ScheduleExcute(L" + context.ExecutionPointer.Id + ",'驳回')";
                    }
                    else
                    {
                        //发送提醒
                        job.invoke_target = typeof(TaskBLL).FullName + ".ScheduleNotice(L" + context.ExecutionPointer.Id + ",0,0)";
                    }

                    job.job_group = group;
                    job.job_name = jobname;
                    job.misfire_policy = "1";
                    job.status = "0";
                    context.JobList.Add(job);
                }

            }
            return await base.Run(context);
        }
    }
}

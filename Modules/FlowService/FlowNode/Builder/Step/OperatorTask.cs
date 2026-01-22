using AuthService;
using Common.EventBus;
using FlowService.Business;
using FlowService.DAL;
using MonitorService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;
namespace FlowService.FlowNode.Builder.Step
{
    /// <summary>
    /// 办理人节点
    /// </summary>
    public class OperatorTask : UserTask
    {
        /// <summary>
        /// 办理时限
        /// </summary>
        public OperatorTimeLimit timeLimit { get; set; }
        /// <summary>
        /// 驳回文本
        /// </summary>
        public string refuseTxt { get; set; }
        /// <summary>
        /// 办理人
        /// </summary>
        public string AssignedPrincipal { get; set; }
        /// <summary>
        /// 表单内联系人
        /// </summary>
        public string formUser { get; set; }
        /// <summary>
        /// 表单内设备责任人
        /// </summary>
        public string formDevice { get; set; }
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            long targetOrgId = context.Workflow.OrgId.Value;
            List<MZ_AdminInfo> adminList = new List<MZ_AdminInfo>();
            if (!context.ExecutionPointer.EventPublished)
            {
                if (!string.IsNullOrEmpty(formDevice))
                {
                    if (context.FormItems.TryGetValue(formDevice, out object outval))
                    {
                        var selectedlist = outval as IEnumerable<object>;
                        if (selectedlist != null)
                        {
                            foreach (var selectUser in selectedlist)
                            {
                                var nodeobj = selectUser as IDictionary<string, object>;
                                var jk = nodeobj["id"];
                                if (jk == null)
                                {
                                    continue;
                                }
                                string newdevid = Convert.ToString(jk);
                                var flowDeviceDAL = context.ServiceProvider.GetService<FlowDeviceDAL>();
                                var devinfo = await flowDeviceDAL.QueryDeviceInfo(newdevid);
                                if (devinfo != null && devinfo.OwnerOrgId > 0)
                                {
                                    targetOrgId = devinfo.OwnerOrgId.Value;
                                    var devUids = await flowDeviceDAL.QueryLeadersByDevice(devinfo.OwnerOrgId.Value, newdevid);
                                    if (devUids.Count > 0)
                                    {
                                        var newtmpadmins = await context.ServiceProvider.GetService<UserDAL>().GetUserListByIds(devUids);
                                        adminList.AddRange(newtmpadmins);
                                    }
                                    else
                                    {
                                        var userDAL = context.ServiceProvider.GetService<UserDAL>();
                                        var ownusers = await userDAL.SelectManUsers(devinfo.OwnerOrgId.Value);
                                        adminList.AddRange(ownusers);
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {
                    //表单内联系人转到到审批人
                    if (!string.IsNullOrEmpty(formUser))
                    {
                        if (context.FormItems.TryGetValue(formUser, out object outval))
                        {
                            var selectedlist = outval as IEnumerable<object>;
                            if (selectedlist != null)
                            {
                                foreach (var selectUser in selectedlist)
                                {
                                    var nodeobj = selectUser as IDictionary<string, object>;
                                    var jk = nodeobj["id"];
                                    if (jk == null)
                                    {
                                        continue;
                                    }
                                    string newuid = Convert.ToString(jk);
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


                    string[] users = AssignedPrincipal.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    long[] tmpxx = Array.ConvertAll(users, x => long.Parse(x));
                    var tmpadmins = await context.ServiceProvider.GetService<UserDAL>().GetUserListByIds(tmpxx.ToList());
                    adminList.AddRange(tmpadmins);
                }


                foreach (var tmpu in adminList)
                {
                    context.ExecutionPointer.UpdateAttribute(string.Format("User/{0}/{1}", tmpu.Id, targetOrgId), string.Empty);
                }
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
                nt.OrgId = targetOrgId;
                nt.TargetType = "OperatorTask";
                nt.TargetUrl = "/flowable/todo?id=" + context.ExecutionPointer.Id.Value;
                nt.Label = context.Workflow.notifyModel.GetTitleOfParsed(context);
                nt.Content = context.Workflow.FlowName + "需要您的办理，请尽快处理";
                context.NoticeList.Add(nt);



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
                    string jobname = "OperatorOver_" + context.ExecutionPointer.Id;
                    string group = "DEFAULT";
                    MZ_Job job = new MZ_Job();
                    job.concurrent = "0";
                    job.createId = 0;
                    job.create_time = DateTime.Now;
                    job.updateId = 0;
                    job.update_time = DateTime.Now;
                    job.cron_expression = string.Format("{0} {1} {2} {3} {4} ? {5}", overTime.Second, overTime.Minute, overTime.Hour, overTime.Day, overTime.Month, overTime.Year);
                    if (timeLimit.handler.type == "REFUSE")
                    {
                        //自动驳回
                        job.invoke_target = typeof(TaskBLL).FullName + ".ScheduleExcute(L" + context.ExecutionPointer.Id + ",'" + refuseTxt.Replace("'", "").Replace("\"", "") + "')";
                    }
                    else
                    {
                        //发送提醒
                        job.invoke_target = typeof(TaskBLL).FullName + ".ScheduleNotice(L" + context.ExecutionPointer.Id + ",1,0)";
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

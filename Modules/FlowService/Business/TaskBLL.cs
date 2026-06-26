using AuthService;
using Common.Share;
using FlowService.DAL;
using FlowService.FlowNode;
using FlowService.FlowNode.FormFields;
using FlowService.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using System.Linq;
using FlowService.FlowNode.Builder;
using Common.EventBus;
using MonitorService.Model;
using MonitorService.Business;
using Common;
using FlowService.FlowNode.Builder.Step;
using Microsoft.Extensions.Logging;
using Common.Json;

namespace FlowService.Business
{
    public class TaskBLL
    {
        private FlowTemplateDAL _template;
        private FormDAL _form;
        private UserDAL _user;
        private DeptDAL _dept;
        private FlowDAL _flowDAL;
        private FlowNodeDAL _flowNodeDAL;
        private ITAServiceProvider _provider;
        private DAL.OrgDAL _orgDAL;
        private GroupDAL _group;
        private FlowQueryDAL _flowQueryDAL;
        private ILogger _logger;
        public TaskBLL(ITAServiceProvider provider, DAL.OrgDAL orgDAL, FlowDAL flow, FlowNodeDAL flowNodeDAL, FlowTemplateDAL template,
          FormDAL form, UserDAL user, DeptDAL dept, GroupDAL groupDAL, FlowQueryDAL flowQueryDAL, ILoggerFactory factory)
        {
            _orgDAL = orgDAL;
            _flowNodeDAL = flowNodeDAL;
            _flowDAL = flow;
            _dept = dept;
            _user = user;
            _provider = provider;
            _template = template;
            _form = form;
            _group = groupDAL;
            _flowQueryDAL = flowQueryDAL;
            _logger = factory.CreateLogger<PluginConfig>();
        }
        public virtual async Task<string> GenerateFFNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("FF");
        }
        /// <summary>
        /// 删除我的流程
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<string>> DeleteFlow(long flowId)
        {
            try
            {
                WorkflowExecutor exe = _provider.GetService<WorkflowExecutor>();
                await exe.DeleteWorkflow(flowId);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(12, ex.Message);
            }
        }
        /// <summary>
        /// 取消流程
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<string>> CancelFlow(long flowId)
        {
            try
            {
                WorkflowExecutor exe = _provider.GetService<WorkflowExecutor>();
                await exe.TerminateWorkflow(flowId);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(28, ex.Message);
            }
        }

        public virtual async Task<BusResponse<UserActionForm>> GetUserActionForm(long nodeId)
        {
            var exe = _provider.GetService<WorkflowExecutor>();
            return await exe.GetUserActionForm(nodeId);
        }
        public virtual async Task ScheduleNotice(long nodeId, int t, int count)
        {
            MZ_Flow_Node node = await _flowNodeDAL.SelecFlowNodeById(nodeId);
            if (node == null)
            {
                return;
            }
            if (node.Status != NodeStatus.WaitingForEvent)
            {
                return;
            }
            //最多推送10次
            if (count >= 10)
            {
                return;
            }
            List<MZ_Flow_Node> nodelist = new List<MZ_Flow_Node>();
            nodelist.Add(node);
            await _flowNodeDAL.InitNodeExtension(nodelist);

            var exe = _provider.GetService<WorkflowExecutor>();
            var nodeRsp = await exe.GetUserActionForm(node);
            if (!nodeRsp.IsSuccess())
            {
                _logger.LogError(nodeRsp.Message);
                return;
            }
            if (nodeRsp.Data.Flow.Status != FlowStatus.Runnable)
            {
                return;
            }
            BuilderContext context = new BuilderContext();
            context.FormItems = nodeRsp.Data.Model;
            context.Fields = nodeRsp.Data.NodeField;
            var nodeForm = nodeRsp.Data;
            if (t == 0)
            {
                ApprovalTask approval = nodeForm.Step as ApprovalTask;
                if (approval != null)
                {
                    FlowTemplateNotice notice = System.Text.Json.JsonSerializer.Deserialize<FlowTemplateNotice>(nodeForm.Flow.notify, MyDefaultTextJsonConfig.DefaultOptions);
                    if (notice == null)
                    {
                        return;
                    }
                    var tmpnodelist = node.ExtensionAttributes.Where(x => x.AttributeKey.StartsWith("User/") && x.AttributeValue == "").ToList();
                    string[] users = tmpnodelist.Select(x => x.AttributeKey.Split('/', StringSplitOptions.RemoveEmptyEntries)[1]).ToArray();
                    if (users.Length > 0)
                    {
                        long[] tmpxx = Array.ConvertAll(users, x => long.Parse(x));
                        var adminList = await _provider.GetService<UserDAL>().GetUserListByIds(tmpxx.ToList());
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
                        var nt = new NoticeEvent(2, targets.ToArray(), notice.types);
                        nt.TargetType = "Approval";
                        nt.TargetUrl = "/flowable/todo?id=" + node.Id;
                        nt.Label = notice.GetTitleOfParsed(context);
                        nt.Content = "第" + (count + 2) + "次提示，" + nodeForm.FormName + "需要您的审批，请尽快处理";
                        nt.OrgId = nodeForm.Flow.OrgId.Value;
                        await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
                    }

                    if (!approval.timeLimit.handler.notify.once)
                    {
                        DateTime overTime = DateTime.Now.AddHours(approval.timeLimit.handler.notify.hour);
                        string jobname = "ApprovalOver_" + nodeId + "_" + count;
                        string group = "DEFAULT";
                        MZ_Job job = new MZ_Job();
                        job.concurrent = "0";
                        job.createId = 0;
                        job.create_time = DateTime.Now;
                        job.updateId = 0;
                        job.update_time = DateTime.Now;
                        job.cron_expression = string.Format("{0} {1} {2} {3} {4} ? {5}", overTime.Second, overTime.Minute, overTime.Hour, overTime.Day, overTime.Month, overTime.Year);
                        job.invoke_target = typeof(TaskBLL).FullName + ".ScheduleNotice(L" + nodeId + ",0," + (++count) + ")";
                        job.job_group = group;
                        job.job_name = jobname;
                        job.misfire_policy = "2";
                        job.status = "0";
                        await _provider.GetService<JobBLL>().InsertJob(job);
                    }
                }
            }
            else
            {
                OperatorTask operatorTask = nodeForm.Step as OperatorTask;
                if (operatorTask != null)
                {
                    FlowTemplateNotice notice = System.Text.Json.JsonSerializer.Deserialize<FlowTemplateNotice>(nodeForm.Flow.notify, MyDefaultTextJsonConfig.DefaultOptions);
                    if (notice == null)
                    {
                        return;
                    }

                    var tmpnodelist = node.ExtensionAttributes.Where(x => x.AttributeKey.StartsWith("User/") && x.AttributeValue == "").ToList();
                    string[] users = tmpnodelist.Select(x => x.AttributeKey.Split('/', StringSplitOptions.RemoveEmptyEntries)[1]).ToArray();
                    if (users.Length > 0)
                    {
                        long[] tmpxx = Array.ConvertAll(users, x => long.Parse(x));

                        var adminList = await _provider.GetService<UserDAL>().GetUserListByIds(tmpxx.ToList());
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
                        var nt = new NoticeEvent(2, targets.ToArray(), notice.types);
                        nt.TargetType = "OperatorTask";
                        nt.TargetUrl = "/flowable/todo?id=" + node.Id;
                        nt.Label = notice.GetTitleOfParsed(context);
                        nt.Content = "第" + (count + 2) + "次提醒，" + nodeForm.FormName + "需要您的办理，请尽快处理";
                        nt.OrgId = nodeForm.Flow.OrgId.Value;
                        await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
                    }

                    if (!operatorTask.timeLimit.handler.notify.once)
                    {
                        DateTime overTime = DateTime.Now.AddHours(operatorTask.timeLimit.handler.notify.hour);
                        string jobname = "OperatorOver_" + nodeId + "_" + count;
                        string group = "DEFAULT";
                        MZ_Job job = new MZ_Job();
                        job.concurrent = "0";
                        job.createId = 0;
                        job.create_time = DateTime.Now;
                        job.updateId = 0;
                        job.update_time = DateTime.Now;
                        job.cron_expression = string.Format("{0} {1} {2} {3} {4} ? {5}", overTime.Second, overTime.Minute, overTime.Hour, overTime.Day, overTime.Month, overTime.Year);
                        job.invoke_target = typeof(TaskBLL).FullName + ".ScheduleNotice(L" + nodeId + ",1," + (++count) + ")";
                        job.job_group = group;
                        job.job_name = jobname;
                        job.misfire_policy = "2";
                        job.status = "0";
                        await _provider.GetService<JobBLL>().InsertJob(job);
                    }
                }
            }

        }
        /// <summary>
        /// 延时执行指定流程
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public virtual async Task ScheduleDelay(long nodeId)
        {
            MZ_Flow_Node node = await _flowNodeDAL.SelecFlowNodeById(nodeId);
            if (node == null)
            {
                return;
            }

            if (node.Status != NodeStatus.WaitingForEvent)
            {
                return;
            }
            var exe = _provider.GetService<WorkflowExecutor>();
            UserAction userac = new UserAction();
            userac.Model = new Dictionary<string, object>();
            userac.OutcomeValue = string.Empty;
            userac.User = new ActionUser();
            userac.User.UserId = 2;
            userac.User.ActionName = userac.OutcomeValue;
            userac.User.OrgId = 1;
            userac.User.ActionDate = DateTime.Now;
            await exe.ExcuteAction(node.FlowId.Value, nodeId.ToString(), userac);
        }
        /// <summary>
        /// 自动执行指定流程
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public virtual async Task ScheduleExcute(long nodeId, string actionName)
        {
            MZ_Flow_Node node = await _flowNodeDAL.SelecFlowNodeById(nodeId);
            if (node == null)
            {
                return;
            }

            if (node.Status != NodeStatus.WaitingForEvent)
            {
                return;
            }
            var exe = _provider.GetService<WorkflowExecutor>();
            UserAction userac = new UserAction();
            userac.Model = new Dictionary<string, object>();
            userac.OutcomeValue = actionName;
            userac.User = new ActionUser();
            userac.User.UserId = 2;
            userac.User.ActionName = userac.OutcomeValue;
            userac.User.OrgId = 1;
            userac.User.ActionDate = DateTime.Now;
            await exe.ExcuteAction(node.FlowId.Value, nodeId.ToString(), userac);
        }

        /// <summary>
        /// 执行指定流程
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<string>> ExcuteFlow(In_TaskExcute data, IUserInfo user)
        {
            try
            {
                //过滤参数
                Dictionary<string, object> newModel = new Dictionary<string, object>();
                foreach (var kvp in data.model)
                {
                    if (!kvp.Key.StartsWith("@"))
                    {
                        newModel.Add(kvp.Key, kvp.Value);
                    }
                }
                data.model = newModel;
                var exe = _provider.GetService<WorkflowExecutor>();
                UserAction userac = new UserAction();
                userac.Model = data.model;
                userac.OutcomeValue = data.action;
                userac.User = new ActionUser();
                userac.User.UserId = user.UserId;
                userac.User.ActionName = userac.OutcomeValue;
                object tmpsign;
                if (data.model.TryGetValue("$ApprovalSign", out tmpsign))
                {
                    userac.User.SignImg = (string)tmpsign;
                    data.model.Remove("$ApprovalSign");
                }
                object tmpremark;
                if (data.model.TryGetValue("$Remark", out tmpremark))
                {
                    userac.User.Remark = (string)tmpremark;
                    data.model.Remove("$Remark");
                }
                userac.User.OrgId = user.OrgId;
                userac.User.ActionDate = DateTime.Now;
                await exe.ExcuteAction(data.flowId, data.key, userac);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(35, ex.Message);

            }

        }


        /// <summary>
        /// 使用指定工作流模板创建工作流实例
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="model"></param>
        /// <param name="assign"></param>
        /// <param name="state">0表示预生成,1表示保存，2表示提交</param>
        /// <param name="isEmbed">是否为嵌入式流程</param>
        /// <param name="uid">发起人</param>
        /// <param name="flowId">流程Id</param>
        /// <returns></returns>
        public virtual async Task<BusResponse<object>> CreateFlow(long templateId, Dictionary<string, object> model, Dictionary<string, List<Out_UserItem>> assign,
            int state, bool isEmbed, long uid, long flowId = 0)
        {
            string flownumber = null;
            //过滤参数
            Dictionary<string, string> inputParams = new Dictionary<string, string>();
            Dictionary<string, object> newModel = new Dictionary<string, object>();
            foreach (var kvp in model)
            {
                if (!kvp.Key.StartsWith("@"))
                {
                    newModel.Add(kvp.Key, kvp.Value);
                }
                else
                {
                    switch (kvp.Key)
                    {
                        case "@FlowNumber":
                            flownumber = (kvp.Value ?? "").ToString();
                            break;
                        default:
                            inputParams.Add(kvp.Key, (kvp.Value ?? "").ToString());
                            break;
                    }
                }
            }
            model = newModel;

            MZ_FlowTemplate template = await _template.SelecFlowTemplateById(templateId);
            if (template == null)
            {
                return BusResponse<object>.Error(28, "模板不存在");
            }

            if (template.startlimit == true && isEmbed == false)
            {
                return BusResponse<object>.Error(29, "当前流程无法直接发起");
            }

            RootNode rootNode = System.Text.Json.JsonSerializer.Deserialize<RootNode>(template.FlowJson, FlowJsonSerializerConfig.NodeOptions);
            if (rootNode == null)
            {
                return BusResponse<object>.Error(21, "模板流程错误");
            }


            if (state > 0)
            {
                #region 校验发布权限
                if (uid != 2)
                {
                    ObjData[] commitObjs = rootNode.props.assignedUser;
                    MZ_AdminInfo userInfo = await _user.GetAdminByOrgId(uid, template.OrgId.Value);
                    if (userInfo == null)
                    {
                        return BusResponse<object>.Error(29, "发起人不存在");
                    }
                    MZ_Dept deptInfo = await _dept.SelectById(userInfo.dept_id.Value);
                    bool canCommit = false;
                    string[] depids = deptInfo.ancestors.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    HashSet<long> dephs = new HashSet<long>();
                    foreach (string depid in depids)
                    {
                        dephs.Add(long.Parse(depid));
                    }

                    foreach (ObjData obj in commitObjs)
                    {
                        if (obj.type == "user" && obj.id == uid)
                        {
                            canCommit = true;
                            break;
                        }
                        else if (obj.type == "dept" && dephs.Contains(obj.id))
                        {
                            canCommit = true;
                            break;
                        }
                    }
                    if (!canCommit && commitObjs.Length > 0)
                    {
                        return BusResponse<object>.Error(30, "无权限发布当前流程");
                    }
                }
                #endregion
            }



            MZ_Form form = await _form.SelecFormById(template.FormId.Value);
            if (form == null)
            {
                return BusResponse<object>.Error(22, "表单不存在");
            }
            FormField[] fields = System.Text.Json.JsonSerializer.Deserialize<FormField[]>(form.FormFields, FlowJsonSerializerConfig.FieldOptions);


            if (state == 2)
            {
                #region 开始校验表单
                Dictionary<string, string> commitOperates = new Dictionary<string, string>();
                foreach (var t in rootNode.props.formPerms)
                {
                    commitOperates.Add(t.id, t.perm);
                }
                string msg;
                foreach (var f in fields)
                {
                    if (!f.Check(commitOperates, model, inputParams, out msg))
                    {
                        return BusResponse<object>.Error(27, msg);
                    }
                }
                foreach (var f in fields)
                {
                    f.FieldRelated(model);
                }
                #endregion
            }


            List<FormField> userField = new List<FormField>();
            foreach (var f in fields)
            {
                f.FindTo((x) => x.name == "UserPicker", userField);
            }
            List<long> uids = new List<long>();
            foreach (var u in userField)
            {
                object val;
                if (model.TryGetValue(u.id, out val))
                {
                    var selectedlist = val as IList<object>;
                    if (selectedlist != null)
                    {
                        foreach (var node in selectedlist)
                        {
                            var nodeobj = node as IDictionary<string, object>;
                            var jk = nodeobj["id"];
                            if (jk == null)
                            {
                                continue;
                            }
                            uids.Add(Convert.ToInt64(jk));
                        }
                    }

                }
            }
            uids.Add(uid);

            MZ_FormData formData = new MZ_FormData();
            formData.flowId = flowId;
            formData.FlowNumber = flownumber;
            formData.CreateUserId = uid;
            formData.Model = model;
            formData.inputParams = inputParams;
            formData.FormFields = form.FormFields;
            formData.Fields = fields;
            formData.Template = template;
            formData.Assign = assign;
            formData.UserDeptList = await _orgDAL.SelectDeptByUserId(uids, template.OrgId.Value);
            formData.isEmbed = isEmbed;
            var flowExcutor = await new ExecutorBuilder(_provider, formData).Build(rootNode, state);
            if (state == 0)
            {
                return BusResponse<object>.Success(await flowExcutor.GenerateUserNodes(formData));
            }
            else
            {
                long flowid = await flowExcutor.StartWorkflow(formData, state == 2);
                formData.flowId = flowid;
                return BusResponse<object>.Success(formData);
            }
        }

        public virtual async Task<PageObject<Out_FlowTodoItem>> GetTodoTaskList(In_TodoList query)
        {
            var user = _provider.GetUser();
            var tlist = await _flowDAL.GetTodoTaskList(query, user);
            if (tlist.List.Count > 0)
            {
                HashSet<long> ulist = new HashSet<long>();
                foreach (var it in tlist.List)
                {
                    if (!ulist.Contains(it.createId))
                    {
                        ulist.Add(it.createId);
                    }
                }
                if (ulist.Count > 0)
                {
                    var umlist = await _orgDAL.SelectUserIn(ulist.ToList(), user.OrgId);
                    Dictionary<long, MZ_AdminInfo> dict = new Dictionary<long, MZ_AdminInfo>();
                    foreach (var um in umlist)
                    {
                        dict.Add(um.Id.Value, um);
                    }
                    foreach (var it in tlist.List)
                    {
                        it.startRealName = dict[it.createId].RealName;
                        it.startDeptName = dict[it.createId].dept_name;
                    }
                }

                //初始化背景和图标
                List<long> templist = tlist.List.Select(x => x.TemplateId).ToList();
                var tempObjList = await _template.SelecFlowTemplateByIds(templist);
                foreach (var titem in tlist.List)
                {
                    var tempFirst = tempObjList.Where(x => x.Id == titem.TemplateId).FirstOrDefault();
                    if (tempFirst != null)
                    {
                        titem.Icon = tempFirst.Icon;
                        titem.Background = tempFirst.Background;
                    }
                }
            }

            return tlist;
        }
        public virtual async Task<PageObject<MZ_Flow>> GetMyProcessList(In_MyProcess query)
        {
            IUserInfo userInfo = _provider.GetUser();
            var tlist = await _flowDAL.GetMyProcessList(query, userInfo);
            if (tlist.List.Count > 0)
            {
                //初始化背景和图标
                List<long> templist = tlist.List.Select(x => x.TemplateId.Value).ToList();
                var tempObjList = await _template.SelecFlowTemplateByIds(templist);
                foreach (var titem in tlist.List)
                {
                    var tempFirst = tempObjList.Where(x => x.Id == titem.TemplateId).FirstOrDefault();
                    if (tempFirst != null)
                    {
                        titem.Icon = tempFirst.Icon;
                        titem.Background = tempFirst.Background;
                    }
                }
            }
            return tlist;
        }
        public virtual async Task<PageObject<Out_CSItem>> GetCSList(In_CSList query)
        {
            var user = _provider.GetUser();
            var tlist = await _flowDAL.GetCSList(query, user);
            if (tlist.List.Count > 0)
            {
                HashSet<long> ulist = new HashSet<long>();
                foreach (var it in tlist.List)
                {
                    if (!ulist.Contains(it.createId))
                    {
                        ulist.Add(it.createId);
                    }
                }
                if (ulist.Count > 0)
                {
                    var umlist = await _orgDAL.SelectUserIn(ulist.ToList(), user.OrgId);
                    if (umlist.Count > 0)
                    {
                        Dictionary<long, MZ_AdminInfo> dict = new Dictionary<long, MZ_AdminInfo>();
                        foreach (var um in umlist)
                        {
                            dict.Add(um.Id.Value, um);
                        }
                        foreach (var it in tlist.List)
                        {
                            it.startRealName = dict[it.createId].RealName;
                            it.startDeptName = dict[it.createId].dept_name;
                        }
                    }

                }

                //初始化背景和图标
                List<long> templist = tlist.List.Select(x => x.TemplateId).ToList();
                var tempObjList = await _template.SelecFlowTemplateByIds(templist);
                foreach (var titem in tlist.List)
                {
                    var tempFirst = tempObjList.Where(x => x.Id == titem.TemplateId).FirstOrDefault();
                    if (tempFirst != null)
                    {
                        titem.Icon = tempFirst.Icon;
                        titem.Background = tempFirst.Background;
                    }
                }
            }
            return tlist;
        }
        public virtual async Task<PageObject<Out_FinishedItem>> GetFinishedList(In_FinishedList query)
        {
            var user = _provider.GetUser();
            var tlist = await _flowDAL.GetFinishedList(query, user);
            if (tlist.List.Count > 0)
            {
                HashSet<long> ulist = new HashSet<long>();
                foreach (var it in tlist.List)
                {
                    if (!ulist.Contains(it.createId))
                    {
                        ulist.Add(it.createId);
                    }
                }
                if (ulist.Count > 0)
                {
                    var umlist = await _orgDAL.SelectUserIn(ulist.ToList(), user.OrgId);
                    Dictionary<long, MZ_AdminInfo> dict = new Dictionary<long, MZ_AdminInfo>();
                    foreach (var um in umlist)
                    {
                        dict.Add(um.Id.Value, um);
                    }
                    foreach (var it in tlist.List)
                    {
                        it.startRealName = dict[it.createId].RealName;
                        it.startDeptName = dict[it.createId].dept_name;
                    }
                }

                //初始化背景和图标
                List<long> templist = tlist.List.Select(x => x.TemplateId).ToList();
                var tempObjList = await _template.SelecFlowTemplateByIds(templist);
                foreach (var titem in tlist.List)
                {
                    var tempFirst = tempObjList.Where(x => x.Id == titem.TemplateId).FirstOrDefault();
                    if (tempFirst != null)
                    {
                        titem.Icon = tempFirst.Icon;
                        titem.Background = tempFirst.Background;
                    }
                }
            }

            return tlist;
        }

        public virtual async Task<List<MZ_FlowGroup>> Select(In_DefinitionList query)
        {
            ITAContext context = _provider.GetService<ITAContext>();
            var dsti = Data_ServerTokenInfo.From(context);
            query.orgId = dsti.OrgId;
            List<MZ_FlowTemplate> flowtemplist = await _template.Select(query);
            In_GroupList groupquery = new In_GroupList();
            groupquery.orgId = query.orgId;
            List<MZ_FlowGroup> grouplist = await _group.SelectGroupList(groupquery);
            Dictionary<long, MZ_FlowGroup> groupDict = new Dictionary<long, MZ_FlowGroup>();
            foreach (var g in grouplist)
            {
                groupDict.Add(g.Id.Value, g);
            }
            foreach (var flowTemp in flowtemplist)
            {
                MZ_FlowGroup flowGroup;
                if (groupDict.TryGetValue(flowTemp.GroupId.Value, out flowGroup))
                {
                    if (flowGroup.Items == null)
                    {
                        flowGroup.Items = new List<MZ_FlowTemplate>();
                    }
                    flowGroup.Items.Add(flowTemp);
                }
            }

            return grouplist.Where(x => x.Items != null).ToList();
        }


        public virtual async Task<BusResponse<UserActionForm>> GetUserRootForm(long id)
        {
            var node = await _flowNodeDAL.SelecRootNode(id);
            var exe = _provider.GetService<WorkflowExecutor>();
            if (node == null)
            {
                return BusResponse<UserActionForm>.Error(211, "流程节点不存在");
            }
            return await exe.GetUserActionForm(node);
        }

        public virtual async Task<BusResponse<List<MZ_FlowQuery>>> GetQueryList(In_FlowQuery query)
        {
            return BusResponse<List<MZ_FlowQuery>>.Success(await _flowQueryDAL.SelectQueryList(query));
        }
    }
}

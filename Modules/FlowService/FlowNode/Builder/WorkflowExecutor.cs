using AuthService;
using Common;
using Common.EventBus;
using Common.IdGenerator;
using Common.Json;
using Common.Share;
using FlowService.DAL;
using FlowService.FlowNode.Builder.Step;
using FlowService.FlowNode.FormFields;
using FlowService.Model;
using MonitorService.Business;
using MyAccess.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace FlowService.FlowNode.Builder
{
    public class WorkflowExecutor
    {
        private ITAServiceProvider _provider;
        private FlowDAL _flowDAL;
        private FlowNodeDAL _nodeDAL;
        private FormDataDAL _formDataDAL;
        private SnowflakeHelper _snowflake;
        private DAL.OrgDAL _orgDAL;
        private JobBLL _jobBLL;
        private List<WorkflowStep> _steps;
        private FlowQueryDAL _queryDAL;
        public WorkflowExecutor(ITAServiceProvider provider, SnowflakeHelper snowflake, FlowDAL flowDAL, FlowNodeDAL nodeDAL
            , FormDataDAL formDataDAL, DAL.OrgDAL orgDAL, JobBLL jobBLL, FlowQueryDAL queryDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _flowDAL = flowDAL;
            _nodeDAL = nodeDAL;
            _formDataDAL = formDataDAL;
            _orgDAL = orgDAL;
            _jobBLL = jobBLL;
            _queryDAL = queryDAL;
        }
        public void Init(List<WorkflowStep> steps)
        {
            _steps = steps;
        }
        /// <summary>
        /// 生成审批与抄送流程节点
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_UserNode>> GenerateUserNodes(MZ_FormData formData)
        {
            List<long> uids = new List<long>();
            List<Out_UserNode> tlist = new List<Out_UserNode>();
            foreach (WorkflowStep tmpitem in _steps)
            {
                if (tmpitem is ApprovalTask at)
                {
                    //判断是否为审批节点
                    Out_UserNode un = new Out_UserNode();
                    if (at.mode == "NEXT")
                    {
                        un.Tip = "依次审批";
                    }
                    else if (at.mode == "AND")
                    {
                        un.Tip = "会签";
                    }
                    else if (at.mode == "OR")
                    {
                        un.Tip = "或签";
                    }
                    un.Id = at.Id;
                    un.Name = at.Name;
                    un.CanAdd = at.CanAdd;
                    un.Type = "Approval";
                    un.Value = new List<Out_UserItem>();
                    if (!string.IsNullOrEmpty(at.AssignedPrincipal))
                    {
                        string[] users = at.AssignedPrincipal.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (string u in users)
                        {
                            var outItem = new Out_UserItem();
                            outItem.Id = u;
                            un.Value.Add(outItem);
                            uids.Add(Convert.ToInt64(outItem.Id));
                        }
                    }
                    tlist.Add(un);
                }
                else if (tmpitem is CSStep cs)
                {
                    //是否为抄送节点
                    Out_UserNode un = new Out_UserNode();
                    un.Id = cs.Id;
                    un.Tip = string.Empty;
                    un.Value = new List<Out_UserItem>();
                    un.Name = cs.Name;
                    un.Type = "CS";
                    un.CanAdd = cs.ShouldAdd;
                    if (!string.IsNullOrEmpty(cs.AssignedPrincipal))
                    {
                        string[] users = cs.AssignedPrincipal.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (string u in users)
                        {
                            var outItem = new Out_UserItem();
                            outItem.Id = u;
                            un.Value.Add(outItem);
                            uids.Add(Convert.ToInt64(outItem.Id));
                        }
                    }
                    tlist.Add(un);
                }
            }

            if (uids.Count > 0)
            {
                var rrusers = await _orgDAL.SelectUserIn(uids, formData.Template.OrgId.Value);
                Dictionary<long, MZ_AdminInfo> userDict = new Dictionary<long, MZ_AdminInfo>();
                foreach (MZ_AdminInfo u in rrusers)
                {
                    userDict.Add(u.Id.Value, u);
                }
                foreach (Out_UserNode oun in tlist)
                {
                    foreach (var tmpui in oun.Value)
                    {
                        MZ_AdminInfo tmpadinfo;
                        if (userDict.TryGetValue(Convert.ToInt64(tmpui.Id), out tmpadinfo))
                        {
                            tmpui.Avatar = tmpadinfo.Avatar;
                            tmpui.RealName = tmpadinfo.RealName;
                        }
                    }
                }
            }

            return tlist;
        }

        /// <summary>
        /// 执行创建流程
        /// </summary>
        /// <param name="formdata"></param>
        /// <param name="excute">是否立即执行</param>
        /// <returns></returns>
        public virtual async Task<long> StartWorkflow(MZ_FormData formdata, bool excute = true)
        {
            MZ_Flow flow;
            if (formdata.flowId > 0)
            {
                flow = await _flowDAL.Select(formdata.flowId);
            }
            else
            {
                flow = new MZ_Flow();
                flow.Id = _snowflake.NextId();
                if (string.IsNullOrEmpty(formdata.FlowNumber))
                {
                    flow.FlowNumber = "FFX" + flow.Id;
                }
                else
                {
                    flow.FlowNumber = formdata.FlowNumber;
                }
                flow.createId = formdata.CreateUserId;
                flow.create_time = DateTime.Now;
            }
            flow.IsEmbed = formdata.isEmbed;
            flow.GroupId = formdata.Template.GroupId;
            flow.OrgId = formdata.Template.OrgId;
            flow.FlowName = formdata.Template.Name;
            flow.del_flag = "0";
            flow.updateId = formdata.CreateUserId;
            flow.update_time = DateTime.Now;
            flow.ExecutionNodes = new List<MZ_Flow_Node>();
            flow.Description = formdata.Template.remark;
            flow.PersistenceData = System.Text.Json.JsonSerializer.Serialize(_steps, FlowJsonSerializerConfig.StepOptions);
            flow.FormFields = formdata.FormFields;
            flow.TemplateId = formdata.Template.Id;
            flow.notify = formdata.Template.notify;
            flow.notifyModel = System.Text.Json.JsonSerializer.Deserialize<FlowTemplateNotice>(flow.notify, MyDefaultTextJsonConfig.DefaultOptions);
            if (formdata.Assign != null)
            {
                flow.Assign = System.Text.Json.JsonSerializer.Serialize(formdata.Assign, MyDefaultTextJsonConfig.DefaultOptions);
            }
            else
            {
                flow.Assign = string.Empty;
            }
            StepExecutionContext context = new StepExecutionContext();
            if (excute)
            {

                if (formdata.Template.sublimit > 0)
                {
                    int cc = await _flowDAL.SelectCount(flow.TemplateId.Value, flow.createId.Value);
                    if (cc >= formdata.Template.sublimit)
                    {
                        throw new Exception("当前流程可提交次数:" + cc + "，您已超过!");
                    }
                }
                if (formdata.flowId > 0)
                {
                    flow.ExecutionNodes = await _nodeDAL.SelectByFlowId(flow.Id.Value);
                    await _nodeDAL.InitNodeExtension(flow.ExecutionNodes);
                }
                flow.Status = FlowStatus.Runnable;
                context.InputParams = formdata.inputParams;
                context.FormItems = formdata.Model;
                context.Fields = formdata.Fields;
                context.ServiceProvider = _provider;
                context.Workflow = flow;
                context.UserDeptList = formdata.UserDeptList;
                context.Creator = flow.createId.Value;
                context.Executor = flow.createId.Value;
                context.StepNodes = _steps;
                context.ExcuteIndex = 0;

                await this.ExcuteStep(context);

                //持久化
                if (context.Workflow.Status != FlowStatus.Complete)
                {
                    foreach (var job in context.JobList)
                    {
                        await _jobBLL.InsertJob(job);
                    }
                }
            }
            else
            {
                context.InputParams = formdata.inputParams;
                context.FormItems = formdata.Model;
                context.Fields = formdata.Fields;
                if (formdata.flowId == 0)
                {
                    flow.Status = FlowStatus.Suspended;
                    var nextPointer = new MZ_Flow_Node();
                    nextPointer.Active = true;
                    nextPointer.Id = _snowflake.NextId();
                    nextPointer.ExtensionAttributes = new List<MZ_Flow_ExtensionAttribute>();
                    nextPointer.FlowId = flow.Id;
                    nextPointer.StartTime = DateTime.Now;
                    nextPointer.Status = NodeStatus.Pending;
                    nextPointer.StepId = _steps[0].Id;
                    nextPointer.StepName = _steps[0].Name;
                    nextPointer.ParentId = 0;
                    nextPointer.ExtensionAttributes = new List<MZ_Flow_ExtensionAttribute>();
                    flow.ExecutionNodes.Add(nextPointer);
                }
            }

            //保存表单数据
            List<MZ_FormDataItem> datalist = new List<MZ_FormDataItem>();
            var formDict = FormField.ToFieldDict(context.Fields);
            foreach (var kvp in context.FormItems)
            {
                FormField ff;
                if (formDict.TryGetValue(kvp.Key, out ff))
                {
                    datalist.AddRange(ff.ToSaveItems(flow.Id.Value, kvp.Value, this._provider));
                }
            }
            //重新生成实例名称
            var newname = flow.notifyModel.GetTitleOfParsed(context);
            if (!string.IsNullOrEmpty(newname))
            {
                flow.FlowName = newname;
            }

            if (formdata.flowId > 0)
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    if (datalist.Count > 0)
                    {
                        await _formDataDAL.UpdateFormData(datalist);
                    }
                    await _flowDAL.UpdateFlowAsync(flow);
                    await scope.CompleteAsync();
                }
            }
            else
            {
                List<MZ_FlowQuery> queryList = new List<MZ_FlowQuery>();
                foreach (var queryItem in formdata.inputParams)
                {
                    queryList.Add(new MZ_FlowQuery()
                    {
                        FlowId = flow.Id,
                        Name = queryItem.Key,
                        Value = queryItem.Value,
                        TemplateId = flow.TemplateId
                    });
                }
                using (BLLTranScope scope = new BLLTranScope())
                {
                    if (datalist.Count > 0)
                    {
                        await _formDataDAL.AddFormData(datalist);
                    }
                    await _flowDAL.InsertFlowAsync(flow);
                    if (queryList.Count > 0)
                    {
                        await _queryDAL.Insert(queryList);
                    }
                    await scope.CompleteAsync();
                }
            }


            foreach (var notice in context.NoticeList)
            {
                await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, notice);
            }

            return flow.Id.Value;
        }
        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public async Task DeleteWorkflow(long flowId)
        {
            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            string wkLock = "WorkflowLock:" + flowId;
            if (await redis.WaitLockTakeAsync(wkLock))
            {
                try
                {
                    MZ_Flow flow = await _flowDAL.Select(flowId);
                    if (flow == null)
                    {
                        throw new Exception("指定流程不存在");
                    }
                    if (flow.Status != FlowStatus.Complete && flow.Status != FlowStatus.Terminated)
                    {
                        throw new Exception("操作失败,流程未结束");
                    }
                    await _flowDAL.DeleteFlowAsync(flowId);
                }
                finally
                {
                    redis.LockRelease(wkLock);
                }
            }

        }
        /// <summary>
        /// 取消流程
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public async Task TerminateWorkflow(long flowId)
        {
            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            string wkLock = "WorkflowLock:" + flowId;
            if (await redis.WaitLockTakeAsync(wkLock))
            {
                try
                {
                    MZ_Flow flow = await _flowDAL.Select(flowId);
                    if (flow == null)
                    {
                        return;
                    }
                    if (flow.Status != FlowStatus.Runnable && flow.Status != FlowStatus.Suspended)
                    {
                        return;
                    }

                    MZ_Flow newFlow = new MZ_Flow();
                    newFlow.Id = flowId;
                    newFlow.Status = FlowStatus.Terminated;
                    await _flowDAL.UpdateFlowAsync(newFlow);
                }
                finally
                {
                    redis.LockRelease(wkLock);
                }
            }
        }
        /// <summary>
        /// 获取指定节点的操作表单信息
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task<BusResponse<UserActionForm>> GetUserActionForm(MZ_Flow_Node node)
        {
            var flow = await _flowDAL.Select(node.FlowId.Value);
            if (flow == null)
            {
                return BusResponse<UserActionForm>.Error(311, "流程不存在");
            }
            var steplist = System.Text.Json.JsonSerializer.Deserialize<List<WorkflowStep>>(flow.PersistenceData, FlowJsonSerializerConfig.StepOptions);
            var step = steplist.Where(x => x.Id == node.StepId).FirstOrDefault();
            FormField[] fields = System.Text.Json.JsonSerializer.Deserialize<FormField[]>(flow.FormFields, FlowJsonSerializerConfig.FieldOptions);

            UserActionForm info = new UserActionForm();
            info.Flow = flow;
            info.TemplateId = flow.TemplateId.Value;
            info.FormName = flow.FlowName;
            info.NodeField = fields;
            info.Step = step;
            info.Model = new Dictionary<string, object>();
            info.FlowId = flow.Id.Value;
            info.NodeStatus = node.Status;
            if (!string.IsNullOrEmpty(flow.Assign))
            {
                info.Assign = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<Out_UserItem>>>(flow.Assign, MyDefaultTextJsonConfig.DefaultOptions);
            }
            else
            {
                info.Assign = new Dictionary<string, List<Out_UserItem>>();
            }


            var tlist = await _formDataDAL.SelecFormData(flow.Id.Value);
            var fieldDict = FormField.ToFieldDict(fields);
            foreach (var fkvp in fieldDict)
            {
                var vals = tlist.Where(x => x.FieldId == fkvp.Value.id).FirstOrDefault();
                if (vals != null)
                {
                    if (!info.Model.ContainsKey(fkvp.Value.id))
                    {
                        info.Model.Add(fkvp.Value.id, fkvp.Value.FromSave(vals));
                    }
                }
            }

            var inputDict = await _queryDAL.SelectFlowQuerys(flow.Id.Value);
            foreach (var iptkvp in inputDict)
            {
                if (!info.Model.ContainsKey(iptkvp.Key))
                {
                    info.Model.Add(iptkvp.Key, iptkvp.Value);
                }
            }


            info.NodeList = new List<Out_Flow_Node>();
            var nodelist = await _nodeDAL.SelectByFlowId(flow.Id.Value);
            if (nodelist.Count > 0)
            {
                await _nodeDAL.InitNodeExtension(nodelist);
                Dictionary<long, Out_Flow_Node> nodeDict = new Dictionary<long, Out_Flow_Node>();
                List<Out_ActionUser> allusers = new List<Out_ActionUser>();
                foreach (var n in nodelist)
                {
                    Out_Flow_Node outnode = new Out_Flow_Node();
                    outnode.Id = n.Id;
                    if (n.StepId == "root")
                    {
                        outnode.ActionUsers = new List<Out_ActionUser>();
                        Out_ActionUser rootuser = new Out_ActionUser();
                        rootuser.UserId = flow.createId.Value;
                        rootuser.ActionDate = n.StartTime.Value;
                        outnode.ActionUsers.Add(rootuser);
                    }
                    else
                    {
                        var nstep = steplist.Where(x => x.Id == n.StepId).FirstOrDefault();
                        if (nstep is UserTask)
                        {
                            var exeuserattr = n.ExtensionAttributes.Where(x => x.AttributeKey == UserTask.ActionUser).FirstOrDefault();
                            if (exeuserattr != null)
                            {
                                outnode.ActionUsers = System.Text.Json.JsonSerializer.Deserialize<List<Out_ActionUser>>(exeuserattr.AttributeValue, MyDefaultTextJsonConfig.DefaultOptions);
                            }
                            else
                            {
                                if (n.Status == NodeStatus.WaitingForEvent)
                                {
                                    if (outnode.WaitUsers == null)
                                    {
                                        outnode.WaitUsers = new List<Out_ActionUser>();
                                    }
                                    var waitlist = n.ExtensionAttributes.Where(x => x.AttributeKey.StartsWith("User/")).ToList();
                                    foreach (var waititem in waitlist)
                                    {
                                        var tukeyarr = waititem.AttributeKey.Split('/', StringSplitOptions.RemoveEmptyEntries);
                                        if (tukeyarr.Length > 1)
                                        {
                                            Out_ActionUser tmpuser = new Out_ActionUser();
                                            tmpuser.UserId = Convert.ToInt64(tukeyarr[1]);
                                            tmpuser.ActionDate = n.StartTime.Value;
                                            outnode.WaitUsers.Add(tmpuser);
                                        }
                                    }
                                }
                            }
                        }
                        else if (nstep is CSStep cstep)
                        {
                            outnode.ActionUsers = new List<Out_ActionUser>();
                            string[] csuids = cstep.AssignedPrincipal.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            foreach (string csu in csuids)
                            {
                                Out_ActionUser tmpuser = new Out_ActionUser();
                                tmpuser.UserId = Convert.ToInt64(csu);
                                tmpuser.ActionDate = n.StartTime.Value;
                                outnode.ActionUsers.Add(tmpuser);
                            }
                        }

                    }
                    if (outnode.ActionUsers != null)
                    {
                        foreach (var acu in outnode.ActionUsers)
                        {
                            allusers.Add(acu);
                        }
                    }
                    if (outnode.WaitUsers != null)
                    {
                        foreach (var wau in outnode.WaitUsers)
                        {
                            allusers.Add(wau);
                        }
                    }

                    outnode.Active = n.Active;
                    outnode.EndTime = n.EndTime;
                    outnode.EventKey = n.EventKey;
                    outnode.EventPublished = n.EventPublished;
                    outnode.FlowId = n.FlowId;
                    outnode.Outcome = n.Outcome;
                    outnode.ParentId = n.ParentId;
                    outnode.StartTime = n.StartTime;
                    outnode.Status = n.Status;
                    outnode.StepId = n.StepId;
                    outnode.StepName = n.StepName;

                    if (!nodeDict.ContainsKey(n.Id.Value))
                    {
                        nodeDict.Add(n.Id.Value, outnode);
                    }
                }


                foreach (var kvp in nodeDict)
                {
                    if (kvp.Value.ParentId.Value == 0)
                    {
                        info.NodeList.Add(kvp.Value);
                    }
                    Out_Flow_Node val;
                    if (nodeDict.TryGetValue(kvp.Value.ParentId.Value, out val))
                    {
                        val.Children.Add(kvp.Value);
                    }
                }

                if (allusers.Count > 0)
                {
                    var rrusers = await _orgDAL.SelectUserIn(allusers.Select(x => x.UserId).Distinct().ToList(), flow.OrgId.Value);
                    Dictionary<long, MZ_AdminInfo> resuser = new Dictionary<long, MZ_AdminInfo>();
                    foreach (MZ_AdminInfo u in rrusers)
                    {
                        resuser.Add(u.Id.Value, u);
                    }

                    foreach (var ukvp in allusers)
                    {
                        MZ_AdminInfo tmpu;
                        if (resuser.TryGetValue(ukvp.UserId, out tmpu))
                        {
                            ukvp.Avatar = tmpu.Avatar;
                            ukvp.RealName = tmpu.RealName;
                        }
                    }
                }

            }

            return BusResponse<UserActionForm>.Success(info);
        }
        /// <summary>
        /// 获取指定节点的操作表单信息
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public async Task<BusResponse<UserActionForm>> GetUserActionForm(long nodeId)
        {
            var node = await _nodeDAL.SelecFlowNodeById(nodeId);
            if (node == null)
            {
                return BusResponse<UserActionForm>.Error(211, "流程节点不存在");
            }
            return await GetUserActionForm(node);
        }
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="eventKey"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual async Task ExcuteAction(long flowId, string eventKey, UserAction action)
        {
            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            string wkLock = "WorkflowLock:" + flowId;
            if (await redis.WaitLockTakeAsync(wkLock))
            {
                try
                {
                    MZ_Flow flow = await _flowDAL.Select(flowId);
                    if (flow == null)
                    {
                        throw new Exception("流程不存在");
                    }
                    if (flow.Status != FlowStatus.Runnable)
                    {
                        throw new Exception("流程状态错误，无法执行");
                    }
                    flow.notifyModel = System.Text.Json.JsonSerializer.Deserialize<FlowTemplateNotice>(flow.notify, MyDefaultTextJsonConfig.DefaultOptions);
                    FormField[] fields = System.Text.Json.JsonSerializer.Deserialize<FormField[]>(flow.FormFields, FlowJsonSerializerConfig.FieldOptions);

                    var nodelist = await _nodeDAL.SelectByFlowId(flow.Id.Value);
                    await _nodeDAL.InitNodeExtension(nodelist);
                    flow.ExecutionNodes = nodelist;

                    List<MZ_FormDataItem> formItems = await _formDataDAL.SelecFormData(flow.Id.Value);
                    //更新表单数据
                    Dictionary<string, object> dataDict = new Dictionary<string, object>();
                    var fieldDict = FormField.ToFieldDict(fields);
                    foreach (var fkvp in fieldDict)
                    {
                        var vals = formItems.Where(x => x.FieldId == fkvp.Value.id).FirstOrDefault();
                        if (vals != null)
                        {
                            dataDict.Add(fkvp.Value.id, fkvp.Value.FromSave(vals));
                        }
                    }

                    //有修改的数据
                    Dictionary<string, object> editMode = new Dictionary<string, object>();
                    foreach (var kvp in action.Model)
                    {
                        editMode.Add(kvp.Key, kvp.Value);
                    }

                    var eventPointer = flow.ExecutionNodes.Where(x => x.EventKey == eventKey).FirstOrDefault();
                    if (eventPointer == null)
                    {
                        throw new Exception("事件不存在");
                    }
                    if (eventPointer.Status != NodeStatus.WaitingForEvent)
                    {
                        throw new Exception("节点状态错误");
                    }
                    eventPointer.EventAction = action;
                    eventPointer.EventPublished = true;


                    //初始化节点数据
                    var steplist = System.Text.Json.JsonSerializer.Deserialize<List<WorkflowStep>>(flow.PersistenceData, FlowJsonSerializerConfig.StepOptions);
                    this.Init(steplist);

                    int stepIdx = -1;
                    for (int i = 0; i < _steps.Count; i++)
                    {
                        if (_steps[i].Id != eventPointer.StepId)
                        {
                            continue;
                        }
                        stepIdx = i;
                    }


                    var inputDict = await _queryDAL.SelectFlowQuerys(flow.Id.Value);

                    #region 开始校验表单
                    if (stepIdx < 0)
                    {
                        throw new Exception("执行校验失败01");
                    }
                    var tmpitem = _steps[stepIdx];
                    if (tmpitem is UserTask ut)
                    {
                        if (ut.optionInit != null)
                        {
                            foreach (var initModel in ut.optionInit)
                            {
                                if (initModel.optionName == action.OutcomeValue)
                                {
                                    string tmpinitval = initModel.InitValue as string;
                                    if (tmpinitval == "@办理人姓名" || tmpinitval == "@审批人姓名")
                                    {
                                        MZ_AdminInfo tmpuserInfo = await _provider.GetService<UserDAL>().GetAdminById(action.User.UserId);
                                        if (tmpuserInfo != null)
                                        {
                                            editMode[initModel.fieldid] = tmpuserInfo.RealName;
                                        }
                                        else
                                        {
                                            editMode[initModel.fieldid] = string.Empty;
                                        }
                                    }
                                    else
                                    {
                                        editMode[initModel.fieldid] = initModel.InitValue;
                                    }
                                }
                            }
                        }

                        Dictionary<string, string> commitOperates = new Dictionary<string, string>();
                        foreach (var t in ut.FormPerms)
                        {
                            commitOperates.Add(t.id, t.perm);
                        }
                        string msg;
                        foreach (var f in fields)
                        {
                            if (!f.Check(commitOperates, editMode, inputDict, out msg))
                            {
                                throw new Exception(msg);
                            }
                        }
                        foreach (var f in fields)
                        {
                            f.FieldRelated(editMode);
                        }
                    }
                    #endregion
                    //更新编辑的数据
                    foreach (var kvp in editMode)
                    {
                        dataDict[kvp.Key] = kvp.Value;
                    }

                    //生成执行上下文
                    StepExecutionContext context = new StepExecutionContext();
                    context.FormItems = dataDict;
                    context.InputParams = inputDict;
                    context.Fields = fields;
                    context.ServiceProvider = _provider;
                    context.Workflow = flow;
                    context.ExcuteIndex = stepIdx;
                    context.StepNodes = _steps;
                    context.Creator = flow.createId.Value;
                    context.Executor = action.User.UserId;
                    context.Workflow.SetUpdateBy(action.User);
                    List<long> uids = new List<long>();
                    uids.Add(context.Creator);
                    uids.Add(context.Executor);
                    List<FormField> userField = new List<FormField>();
                    foreach (var f in fields)
                    {
                        f.FindTo((x) => x.name == "UserPicker", userField);
                    }
                    foreach (var u in userField)
                    {
                        object outval;
                        if (context.FormItems.TryGetValue(u.id, out outval))
                        {
                            var selectedlist = outval as IEnumerable<object>;
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

                    if (uids.Count > 0)
                    {
                        context.UserDeptList = await _orgDAL.SelectDeptByUserId(uids, flow.OrgId.Value);
                    }
                    else
                    {
                        context.UserDeptList = new List<MZ_UserDept>();
                    }


                    await this.ExcuteStep(context);



                    //持久化
                    if (context.Workflow.Status != FlowStatus.Complete)
                    {
                        foreach (var job in context.JobList)
                        {
                            await _jobBLL.InsertJob(job);
                        }
                    }

                    List<MZ_FormDataItem> datalist = new List<MZ_FormDataItem>();
                    var formDict = FormField.ToFieldDict(context.Fields);
                    foreach (var kvp in context.FormItems)
                    {
                        FormField ff;
                        if (formDict.TryGetValue(kvp.Key, out ff))
                        {
                            datalist.AddRange(ff.ToSaveItems(flow.Id.Value, kvp.Value, this._provider));
                        }
                    }

                    using (BLLTranScope scope = new BLLTranScope())
                    {
                        if (datalist.Count > 0)
                        {
                            await _formDataDAL.UpdateFormData(datalist);
                        }
                        await _flowDAL.UpdateFlowAsync(flow);
                        await scope.CompleteAsync();
                    }

                    foreach (var notice in context.NoticeList)
                    {
                        await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, notice);
                    }
                }
                finally
                {
                    redis.LockRelease(wkLock);
                }
            }


        }

        /// <summary>
        /// 执行完成就返回true
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task ExcuteStep(StepExecutionContext context)
        {
            if (context.ExcuteIndex >= context.StepNodes.Count)
            {
                context.Workflow.FinishTime = DateTime.Now;
                context.Workflow.Status = FlowStatus.Complete;
                return;
            }
            if (context.Step.PersistenceNode)
            {
                var nextPointer = context.Workflow.ExecutionNodes.Where(x => x.StepId == context.Step.Id).FirstOrDefault();
                if (nextPointer == null)
                {
                    nextPointer = new MZ_Flow_Node();
                    nextPointer.Id = _snowflake.NextId();
                    nextPointer.ExtensionAttributes = new List<MZ_Flow_ExtensionAttribute>();
                    nextPointer.FlowId = context.Workflow.Id;
                    nextPointer.StartTime = DateTime.Now;
                    nextPointer.Status = NodeStatus.Running;
                    nextPointer.StepId = context.Step.Id;
                    nextPointer.StepName = context.Step.Name;
                    nextPointer.Active = true;
                    if (context.ExecutionPointer == null)
                    {
                        nextPointer.ParentId = 0;
                    }
                    else
                    {
                        nextPointer.ParentId = context.ExecutionPointer.Id;
                    }
                    context.Workflow.ExecutionNodes.Add(nextPointer);
                }
                context.ExecutionPointer = nextPointer;
            }

            var result = await context.Step.Run(context);
            if (result.Directive == ExecutionDirective.EndWorkflow)
            {
                if (context.Step.Id == context.ExecutionPointer.StepId)
                {
                    context.ExecutionPointer.Status = NodeStatus.Complete;
                    context.ExecutionPointer.EndTime = DateTime.Now;
                    context.ExecutionPointer.Outcome = result.OutcomeValue;
                }
                context.Workflow.FinishTime = DateTime.Now;
                context.Workflow.Status = FlowStatus.Complete;
                return;
            }
            else if (result.Directive == ExecutionDirective.Defer)
            {
                if (context.Step.Id == context.ExecutionPointer.StepId)
                {
                    context.ExecutionPointer.EventKey = result.OutcomeValue;
                    context.ExecutionPointer.Status = NodeStatus.WaitingForEvent;
                }
                return;
            }
            else
            {

                if (context.Step.Id == context.ExecutionPointer.StepId)
                {
                    context.ExecutionPointer.Status = NodeStatus.Complete;
                    context.ExecutionPointer.EndTime = DateTime.Now;
                    context.ExecutionPointer.Outcome = result.OutcomeValue;
                }

                if (result.ActiveChildren)
                {
                    if (result.Parallel)
                    {
                        var children = context.Step.Children;
                        var exePointer = context.ExecutionPointer;
                        foreach (var idx in children)
                        {
                            context.ExecutionPointer = exePointer;
                            context.ExcuteIndex = idx;
                            await ExcuteStep(context);
                            if (context.Workflow.Status == FlowStatus.Complete)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        context.ExcuteIndex = context.ExcuteIndex + 1;
                        await ExcuteStep(context);
                    }
                }
                else
                {
                    var nextlink = context.ExcuteIndex + 1;
                    while (nextlink < context.StepNodes.Count && context.StepNodes[nextlink].Level > context.Step.Level)
                    {
                        nextlink++;
                    }
                    context.ExcuteIndex = nextlink;
                    await ExcuteStep(context);
                }

            }
        }

    }
}

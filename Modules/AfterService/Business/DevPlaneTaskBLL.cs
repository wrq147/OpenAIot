using AuthService;
using Common;
using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using AfterService.DAL;
using AfterService.Model;
using FlowService.Business;
using FlowService.DAL;
using FlowService.FlowNode.Builder;
using FlowService.Model;
using IoTService.DAL;
using TemplateAction.Core;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using FlowService.FlowNode.FormFields;
using Newtonsoft.Json;

namespace AfterService.Business
{
    public class DevPlaneTaskBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private DevPlaneTaskDAL _devPlaneTaskDAL;
        public DevPlaneTaskBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, DevPlaneTaskDAL devPlaneTaskDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _devPlaneTaskDAL = devPlaneTaskDAL;
        }
        public virtual async Task<Out_PlaneStatis> GetPlaneStatistics(IUserInfo user, string typeId)
        {
            var tmptt = await _devPlaneTaskDAL.GetPlaneStatistics(user, typeId); ;
            tmptt.total = tmptt.doing + tmptt.wait_work + tmptt.working + tmptt.finish + tmptt.expired + tmptt.accepted + tmptt.noaccept + tmptt.invalid;
            return tmptt;
        }
        public virtual async Task<List<Out_PlaneStatisItem>> GetPlaneTaskStatisList(IUserInfo user, In_PlaneTaskStatisList data)
        {
            var tmplist = await _devPlaneTaskDAL.GetPlaneStatisticsList(user, data);
            foreach (var tmpitem in tmplist)
            {
                tmpitem.total = tmpitem.doing + tmpitem.wait_work + tmpitem.working + tmpitem.finish + tmpitem.expired + tmpitem.accepted + tmpitem.noaccept + tmpitem.invalid;
            }
            return tmplist;
        }
        public virtual async Task<PageObject<Out_PlaneDay>> SelectDayPage(In_DevPlaneTask query)
        {
            var user = _provider.GetUser();
            var tpage = await _devPlaneTaskDAL.QueryDayPage(query, user);
            List<Out_DeviceWithRoome> roomList;
            var dvIds = tpage.List.Select(x => x.TargetId).ToList();
            if (dvIds.Count > 0)
            {
                roomList = await _provider.GetService<RoomDeviceDAL>().QueryDeivceWithRoomList(dvIds);
            }
            else
            {
                roomList = new List<Out_DeviceWithRoome>();
            }

            if (tpage.List.Count > 0)
            {
                List<string> taskIdList = new List<string>();
                foreach (var item in tpage.List)
                {
                    var taskids = item.TaskIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    item.TaskIdList = taskids;
                    taskIdList.AddRange(taskids);
                    item.RoomNames = roomList.Where(x => x.TargetId == item.TargetId).Select(x => x.RoomName).ToList();
                }
                taskIdList = taskIdList.Distinct().ToList();
                var tasklist = await _devPlaneTaskDAL.SelectList(x => taskIdList.Contains(x.Id));
                foreach (var item in tpage.List)
                {
                    item.TaskList = tasklist.Where(x => item.TaskIdList.Contains(x.Id)).ToList();
                }
            }
            return tpage;
        }
        public virtual async Task<PageObject<MZ_PlaneTask>> SelectList(In_DevPlaneTask query)
        {
            var user = _provider.GetUser();
            var tpagelist = await _devPlaneTaskDAL.QueryPlanePage(query, user);

            var tflowIds = tpagelist.List.Select(x => x.FlowId.Value).ToList();
            List<Out_FlowTemplate> templates = new List<Out_FlowTemplate>();
            List<Out_FlowTodoItem> todoList = new List<Out_FlowTodoItem>();
            if (tflowIds.Count > 0)
            {
                var flowDAL = _provider.GetService<FlowDAL>();
                var formDAL = _provider.GetService<FormDAL>();
                var formDataDAL = _provider.GetService<FormDataDAL>();
                todoList = await flowDAL.GetTodoTaskListByIds(tflowIds, user);
                templates = await flowDAL.SelectTemplateByFlowIdList(tflowIds);
                var dataItems = await formDataDAL.SelecFormDataByIds(tflowIds);
                List<Out_FlowForm> out_FlowForms = await formDAL.SelectFormByFlowId(tflowIds);
                foreach (var tmpflowForm in out_FlowForms)
                {
                    FormField[] fields = JsonConvert.DeserializeObject<FormField[]>(tmpflowForm.FormFields, new JsonFieldConvert());
                    var fieldDict = FormField.ToFieldDict(fields);
                    var tmpdatas = dataItems.Where(x => x.FlowId == tmpflowForm.FlowId);
                    Dictionary<string, string> dictrs = new Dictionary<string, string>();
                    foreach (var fkvp in fieldDict)
                    {
                        var vals = tmpdatas.Where(x => x.FieldId == fkvp.Value.id).FirstOrDefault();
                        if (vals != null)
                        {
                            object tmpval = fkvp.Value.FromSave(vals);
                            if (tmpval == null)
                            {
                                continue;
                            }
                            if (tmpval is Array arr)
                            {
                                dictrs.Add(fkvp.Value.title, string.Join(',', arr));
                            }
                            else
                            {
                                dictrs.Add(fkvp.Value.title, tmpval.ToString());
                            }
                        }
                    }
                    var tmpitem = tpagelist.List.Where(x => x.FlowId == tmpflowForm.FlowId).FirstOrDefault();
                    if (tmpitem != null)
                    {
                        tmpitem.DataItems = dictrs;
                    }
                }


            }
            List<Out_DeviceWithRoome> roomList;
            var dvIds = tpagelist.List.Select(x => x.TargetId).ToList();
            if (dvIds.Count > 0)
            {
                roomList = await _provider.GetService<RoomDeviceDAL>().QueryDeivceWithRoomList(dvIds);
            }
            else
            {
                roomList = new List<Out_DeviceWithRoome>();
            }
            if (tpagelist.List.Count > 0)
            {
                var userDict = await _provider.GetService<UserDAL>().NavigateDict(tpagelist.List, x => true, x => x.UserId);
                foreach (var item in tpagelist.List)
                {
                    if (userDict.TryGetValue(item.UserId, out MZ_AdminInfo uinfo))
                    {
                        item.UserInfo = uinfo;
                    }

                    item.RoomNames = roomList.Where(x => x.TargetId == item.TargetId).Select(x => x.RoomName).ToList();
                    item.TodoTasks = todoList.Where(x => x.FlowId == item.FlowId).ToList();
                    var tm = templates.Where(x => x.FlowId == item.FlowId).FirstOrDefault();
                    if (tm != null)
                    {
                        item.Icon = tm.Icon;
                        item.Background = tm.Background;
                    }
                }
            }

            return tpagelist;
        }
        public virtual async Task<BusResponse<MZ_PlaneTask>> Info(string id)
        {
            var info = await _devPlaneTaskDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_PlaneTask>.Error(123, "计划任务不存在");
            }
            var user = _provider.GetUser();
            var tmpuser = await _provider.GetService<UserDAL>().GetAdminById(info.UserId.Value);
            if (tmpuser != null)
            {
                info.UserInfo = tmpuser;
            }
            List<Out_DeviceWithRoome> roomList = await _provider.GetService<RoomDeviceDAL>().QueryDeivceWithRoom(info.TargetId);
            info.RoomNames = roomList.Select(x => x.RoomName).ToList();
            var tflowIds = new List<long>();
            tflowIds.Add(info.FlowId.Value);
            var flowDAL = _provider.GetService<FlowDAL>();
            info.TodoTasks = await flowDAL.GetTodoTaskListByIds(tflowIds, user);
            info.TargetDevice = await _provider.GetService<IotDeviceDAL>().Select(info.TargetId);
            MZ_FlowTemplate template = await flowDAL.SelectTemplateByFlowId(info.FlowId.Value);
            if (template != null)
            {
                info.Icon = template.Icon;
                info.Background = template.Background;
                foreach (var todoItem in info.TodoTasks)
                {
                    todoItem.Icon = template.Icon;
                    todoItem.Background = template.Background;
                }
            }
            return BusResponse<MZ_PlaneTask>.Success(info);
        }
        public async Task<string> GeneratePlaneNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("PT");
        }
        public virtual async Task<BusResponse<Dictionary<string, object>>> CreateTaskForm(string devId, string planeId)
        {
            var devPlaneDAL = _provider.GetService<DevPlaneDAL>();
            var planeType = await devPlaneDAL.Select(planeId);
            if (planeType == null)
            {
                return BusResponse<Dictionary<string, object>>.Error(110, "计划不存在");
            }
            var dev = await _provider.GetService<IotDeviceDAL>().Select(devId);
            if (dev == null)
            {
                return BusResponse<Dictionary<string, object>>.Error(113, "计划的设备不存在");
            }

            var flowitems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DevFlowItem>>(planeType.FlowInitJson);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            var user = _provider.GetUser();
            MZ_AdminInfo submitUser = await _provider.GetService<UserDAL>().GetAdminById(user.UserId);
            List<Out_DeviceWithRoome> roomList = null;
            List<MZ_RoomCategory> categoryList = null;
            List<MZ_AdminInfo> leaders = null;
            foreach (var fitem in flowitems)
            {
                dict.Add(fitem.id, await fitem.GetRealValue(_provider, dev, planeType, null, submitUser, roomList, categoryList, leaders));
            }
            return BusResponse<Dictionary<string, object>>.Success(dict);
        }
        public virtual async Task<BusResponse<string>> Add(In_AddPlaneTask addform)
        {
            var data = addform.task;
            var user = _provider.GetUser();
            data.Id = _snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            data.TaskStatus = 0;
            data.DispatchUserId = 0;
            data.ExeUserId = 0;
            data.CheckUserId = 0;
            data.StartOn = data.CreatedOn = DateTime.Now;
            data.NoticeCount = 0;
            MZ_AdminInfo submitUser = await _provider.GetService<UserDAL>().GetAdminById(user.UserId);
            if (submitUser == null)
            {
                return BusResponse<string>.Error(108, "发起人不存在");
            }
            data.DeptId = submitUser.dept_id;
            data.UserId = user.UserId;
            if (string.IsNullOrEmpty(data.PlaneNumber))
            {
                data.PlaneNumber = await GeneratePlaneNumber();
            }
            else
            {
                if (await _devPlaneTaskDAL.Some(x => x.PlaneNumber == data.PlaneNumber))
                {
                    return BusResponse<string>.Error(117, "编号已被使用");
                }
            }
            var devPlaneDAL = _provider.GetService<DevPlaneDAL>();
            var planeType = await devPlaneDAL.Select(data.PlanTypeId);
            if (planeType == null)
            {
                return BusResponse<string>.Error(110, "计划不存在");
            }

            var dev = await _provider.GetService<IotDeviceDAL>().Select(data.TargetId);
            if (dev == null)
            {
                return BusResponse<string>.Error(113, "计划的设备不存在");
            }
            var troolList = await _provider.GetService<RoomDeviceDAL>().SelectList(x => x.OrgId == user.OrgId && x.TargetId == dev.Id);
            if (troolList.Count > 0)
            {
                bool isInRoom = false;
                foreach (var troom in troolList)
                {
                    if (await devPlaneDAL.ExistPlaneRoom(data.PlanTypeId, troom.Id))
                    {
                        isInRoom = true;
                        break;
                    }
                }
                if (!isInRoom)
                {
                    if (!await devPlaneDAL.ExistPlaneDevice(data.PlanTypeId, data.TargetId) && !await devPlaneDAL.ExistPlaneProduct(data.PlanTypeId, dev.ProductId))
                    {
                        return BusResponse<string>.Error(111, "计划目标不存在");
                    }
                }
            }
            else
            {
                if (!await devPlaneDAL.ExistPlaneDevice(data.PlanTypeId, data.TargetId) && !await devPlaneDAL.ExistPlaneProduct(data.PlanTypeId, dev.ProductId))
                {
                    return BusResponse<string>.Error(111, "计划目标不存在");
                }
            }

            if (planeType.StartWay != 0)
            {
                return BusResponse<string>.Error(112, $"无法添加 {planeType.Name} 的任务");
            }

            if (planeType.OrgId != user.OrgId && user.OrgId != dev.OrgId && user.OrgId != dev.UseOrgId && user.OrgId != dev.OwnerOrgId)
            {
                return BusResponse<string>.Error(121, $"无权添加 {planeType.Name} 的任务");
            }

            try
            {
                data.EndOn = planeType.PlaneDays == 0 ? data.StartOn.Value.Date.AddDays(1).AddTicks(-1) : data.StartOn.Value.Date.AddDays(planeType.PlaneDays.Value);
                DevFlowCreate flowcreate = new DevFlowCreate();
                flowcreate.templateId = planeType.FlowTemplateId.Value;

                //对未初始化数据重新初始化
                List<DevFlowItem> flowitems;
                if (string.IsNullOrEmpty(planeType.FlowInitJson))
                {
                    flowitems = new List<DevFlowItem>();
                }
                else
                {
                    flowitems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DevFlowItem>>(planeType.FlowInitJson);
                }
                flowcreate.model = addform.model;
                flowcreate.assign = addform.assign;
                if (!addform.model.ContainsKey("@from"))
                {
                    flowcreate.model.Add("@from", data.PlaneNumber);
                }
                if (!addform.model.ContainsKey("@fromtype"))
                {
                    flowcreate.model.Add("@fromtype", "计划任务单");
                }
                if (!addform.model.ContainsKey("@FlowNumber"))
                {
                    flowcreate.model.Add("@FlowNumber", data.PlaneNumber);
                }
                flowcreate.UserId = user.UserId;
                List<Out_DeviceWithRoome> roomList = null;
                List<MZ_RoomCategory> categoryList = null;
                List<MZ_AdminInfo> leaders = null;
                foreach (var fitem in flowitems)
                {
                    if (!addform.model.ContainsKey(fitem.id))
                    {
                        addform.model.Add(fitem.id, await fitem.GetRealValue(_provider, dev, planeType, null, submitUser, roomList, categoryList, leaders));
                    }
                }
                await _devPlaneTaskDAL.Insert(data);
                var rsp = await BusUtility.Call("NewFlowTask", flowcreate);
                var brs = rsp.GetResult<BusResponse<long>>();
                if (brs.IsSuccess())
                {
                    data.FlowId = brs.Data;
                }
                else
                {
                    await _devPlaneTaskDAL.Delete(data.Id);
                    return BusResponse<string>.Error(114, "流程创建失败:" + brs.Message);
                }

                MZ_PlaneTask uptask = new MZ_PlaneTask();
                uptask.Id = data.Id;
                uptask.FlowId = data.FlowId;
                await _devPlaneTaskDAL.Update(uptask);

                return BusResponse<string>.Success(data.Id);
            }
            catch (Exception ex)
            {
                await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(data.FlowId.Value);
                return BusResponse<string>.Error(116, ex.Message);
            }
        }

        public virtual async Task<BusResponse<string>> Cancel(string id)
        {
            MZ_PlaneTask old = await _devPlaneTaskDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "任务不存在");
            }
            var user = _provider.GetUser();
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }
            try
            {
                await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(old.FlowId.Value);
                MZ_PlaneTask pt = new MZ_PlaneTask();
                pt.Id = id;
                pt.TaskStatus = 7;
                await _devPlaneTaskDAL.Update(pt);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }
        /// <summary>
        /// 定期执行到期处理
        /// </summary>
        /// <returns></returns>
        public virtual async System.Threading.Tasks.Task ExpireExecute()
        {
            var deviceDAL = _provider.GetService<IotDeviceDAL>();
            var devPlaneDAL = _provider.GetService<DevPlaneDAL>();
            var taskBLL = _provider.GetService<TaskBLL>();
            var tasklist = await _devPlaneTaskDAL.SelectList(x => x.TaskStatus < 3 && x.NoticeCount > -1);
            var groupTaskList = tasklist.GroupBy(x => x.PlanTypeId);
            HashSet<string> gptip = new HashSet<string>();
            foreach (var groupTask in groupTaskList)
            {
                var planeType = await devPlaneDAL.Select(groupTask.Key);
                if (planeType == null)
                {
                    MZ_PlaneTask newtask = new MZ_PlaneTask();
                    newtask.TaskStatus = 7;
                    int rs = await _devPlaneTaskDAL.Update(newtask, x => x.PlanTypeId == groupTask.Key && x.TaskStatus < 3);
                    if (rs <= 0)
                    {
                        continue;
                    }
                    //中断流程
                    foreach (var task in groupTask)
                    {
                        try
                        {
                            await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(task.FlowId.Value);
                        }
                        catch { }
                    }
                    return;
                }
                else
                {

                    foreach (var task in groupTask)
                    {
                        MZ_PlaneTask newtask = new MZ_PlaneTask();
                        if (planeType.PlaneDays > 0 && DateTime.Now > task.EndOn)
                        {
                            newtask.TaskStatus = 4;
                            int rs = await _devPlaneTaskDAL.Update(newtask, x => x.Id == task.Id && x.TaskStatus < 3);
                            if (rs <= 0)
                            {
                                continue;
                            }
                            //中断流程
                            try
                            {
                                await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(task.FlowId.Value);
                            }
                            catch { }
                            return;
                        }

                        //超期通知
                        if (!string.IsNullOrEmpty(planeType.ExpireNotices))
                        {
                            var expireNotices = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExpireNoticeItem>>(planeType.ExpireNotices);
                            int curidx = task.NoticeCount.Value;
                            if (expireNotices.Count > curidx)
                            {
                                var item = expireNotices[curidx];
                                if (task.StartOn.Value.AddDays(item.day) < DateTime.Now)
                                {
                                    newtask.NoticeCount = task.NoticeCount + 1;
                                    int rs = await _devPlaneTaskDAL.Update(newtask, x => x.Id == task.Id && x.TaskStatus < 3);
                                    if (rs <= 0)
                                    {
                                        continue;
                                    }
                                    //执行通知
                                    List<long> userlist;
                                    if (item.way == 1)
                                    {
                                        userlist = item.target.Select(x => x.userid).ToList();
                                    }
                                    else if (item.way == 2)
                                    {
                                        userlist = await _provider.GetService<DevPlaneTaskDAL>().QueryLeadersByDevice(task.OrgId.Value, task.TargetId);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    var recvList = await _provider.GetService<UserDAL>().GetUserListByIds(userlist);
                                    if (item.ccway == 1)
                                    {
                                        List<TargetUser> targets = new List<TargetUser>();
                                        foreach (var recvId in recvList)
                                        {
                                            string tmpkk = groupTask.Key + "|" + recvId.Id;
                                            if (gptip.Contains(tmpkk))
                                            {
                                                continue;
                                            }
                                            targets.Add(new TargetUser()
                                            {
                                                uid = recvId.Id.Value,
                                                email = recvId.Email,
                                                phone = recvId.Mobile
                                            });
                                            gptip.Add(tmpkk);
                                        }
                                        if (targets.Count > 0)
                                        {
                                            var nt = new NoticeEvent(2, targets.ToArray(), new string[] { "APP", "WX" });
                                            nt.OrgId = task.OrgId.Value;
                                            nt.TargetType = "PlaneGroupNotice";
                                            nt.TargetUrl = "/after/devplane/task?id=0";
                                            nt.Content = $"新增了{groupTask.Count()}条超期任务【{planeType.Name}】";
                                            nt.Label = "设备计划消息";
                                            await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
                                        }
                                    }
                                    else
                                    {
                                        List<TargetUser> targets = new List<TargetUser>();
                                        foreach (var recvId in recvList)
                                        {
                                            targets.Add(new TargetUser()
                                            {
                                                uid = recvId.Id.Value,
                                                email = recvId.Email,
                                                phone = recvId.Mobile
                                            });
                                        }
                                        string tipName = task.PlanName;
                                        task.TargetDevice = await deviceDAL.Select(task.TargetId);
                                        if (task.TargetDevice != null)
                                        {
                                            tipName += "-" + task.TargetDevice.Name;
                                        }
                                        var nt = new NoticeEvent(2, targets.ToArray(), new string[] { "APP", "WX" });
                                        nt.OrgId = task.OrgId.Value;
                                        nt.TargetType = "PlaneTaskNotice";
                                        nt.TargetUrl = "/after/devplane/task?id=" + task.Id;
                                        nt.Content = $"【{tipName}】任务已超期";
                                        nt.Label = "设备计划消息";
                                        await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
                                    }
                                }
                                continue;
                            }
                        }
                        if (planeType.PlaneDays <= 0)
                        {
                            newtask.NoticeCount = -1;
                            await _devPlaneTaskDAL.Update(newtask, x => x.Id == task.Id && x.TaskStatus < 3);
                        }
                    }
                }

            }

        }
    }
}

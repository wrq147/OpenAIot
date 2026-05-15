using Common.Share;
using AfterService.DAL;
using AfterService.Model;
using TemplateAction.Core;
using AuthService;
using System.Linq.Expressions;
using Common.IdGenerator;
using MonitorService.Business;
using MonitorService.Model;
using Quartz;
using Common;
using Common.EventBus;
using JiebaNet.Segmenter.Common;
using FlowService.DAL;
using FlowService.FlowNode.Builder;
using MyAccess.DB.Builder.WhereToSql;
using FlowService.Model;
using MonitorService.Util;
using IoTService.DAL;
using IoTService.Models;
using MonitorService.DAL;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using Common.Json;
using AfterService.PlanUtil;

namespace AfterService.Business
{
    public class DevPlaneBLL
    {
        private ITAServiceProvider _provider;
        private DevPlaneDAL _devPlaneDAL;
        private SnowflakeHelper _snowflake;
        public DevPlaneBLL(ITAServiceProvider provider, DevPlaneDAL devPlaneDAL, SnowflakeHelper snowflake)
        {
            _provider = provider;
            _devPlaneDAL = devPlaneDAL;
            _snowflake = snowflake;
        }
        public async Task<string> GeneratePlaneNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("JH");
        }
        public virtual async Task<List<Out_PlaneFlowItem>> DeviceFlowList(string id)
        {
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return new List<Out_PlaneFlowItem>();
            }
            var device = await _provider.GetService<IotDeviceDAL>().Select(id);
            if (device == null)
            {
                return new List<Out_PlaneFlowItem>();
            }
            var roomList = await _provider.GetService<RoomDeviceDAL>().QueryDeivceWithRoom(device.Id);
            string inroomIds = "";
            foreach (var roomItem in roomList)
            {
                inroomIds += "'" + roomItem.RoomId + "',";
            }
            if (roomList.Count > 0)
            {
                inroomIds = inroomIds.TrimEnd(',');
                inroomIds = " or (TargetId in (" + inroomIds + ") and TargetType=2)";
            }
            string appsql = " and EXISTS(select 1 from mz_plane_target where PlaneId=t.Id and ((TargetId='" + id + "' and TargetType=0) or (TargetId='" + device.ProductId + "' and TargetType=1)" + inroomIds + "))";
            var scope = await user.GetScope(this._provider, "/AfterService/DevPlane/List");
            string scopestr = string.Empty;
            if (scope != null)
            {
                List<string> keys = new List<string>();
                keys.Add(user.UserId.ToString());
                string tsqlmatch = _devPlaneDAL.GetUsingDbHelp().CreateCompatible().FullSearch("rd.Helper", keys);
                scopestr = scope.GenerateFilter("rd.DeptId", "rd.LeaderId", "rd.LeaderId=" + user.UserId + " or " + tsqlmatch, false, false, false);
            }
            string isExistStart = "IsFilterLeader=0 or (IsFilterLeader=1 and EXISTS(select 1 from mz_iot_device d left join mz_room_device_v rd on d.Id=rd.TargetId where " + scopestr + " and d.Id in (select TargetId from mz_plane_target where TargetType=0 and PlaneId=t.Id) or d.ProductId in (select TargetId FROM mz_plane_target where TargetType=1 and PlaneId=t.Id) or rd.Id in (select TargetId FROM mz_plane_target where TargetType=2 and PlaneId=t.Id)))";
            appsql = appsql + " and (" + isExistStart + ")";

            var tlist = await _devPlaneDAL.DeviceFlowList(appsql, user);
            var tcountList = await _provider.GetService<DevPlaneTaskDAL>().QueryDevTaskCount(id, user);
            foreach (var titem in tlist)
            {
                var tmpitem = tcountList.Where(x => x.TemplateId == titem.FlowTemplateId).FirstOrDefault();
                if (tmpitem != null)
                {
                    titem.WaitDeal = tmpitem.Total;
                }
                else
                {
                    titem.WaitDeal = 0;
                }
            }
            return tlist;
        }
        public virtual async Task<List<ObjectItem>> SelectNameList(IUserInfo user)
        {
            return await _devPlaneDAL.SelectPlaneTypeNames(user);
        }
        public virtual async Task<PageObject<MZ_PlaneType>> SelectList(In_DevPlaneType query)
        {
            var user = _provider.GetUser();
            Expression<Func<MZ_PlaneType, bool>> expression;
            if (!string.IsNullOrEmpty(query.DeviceId))
            {
                var device = await _provider.GetService<IotDeviceDAL>().Select(query.DeviceId);
                if (device == null)
                {
                    return new PageObject<MZ_PlaneType>();
                }
                var roomList = await _provider.GetService<RoomDeviceDAL>().QueryDeivceWithRoom(device.Id);
                string inroomIds = "";
                foreach (var roomItem in roomList)
                {
                    inroomIds += "'" + roomItem.RoomId + "',";
                }
                if (roomList.Count > 0)
                {
                    inroomIds = inroomIds.TrimEnd(',');
                    inroomIds = " or (TargetId in (" + inroomIds + ") and TargetType=2)";
                }
                string isExistDevice = "EXISTS(select 1 from mz_plane_target where PlaneId=mz_plane_type.Id and ((TargetId='" + query.DeviceId + "' and TargetType=0) or (TargetId='" + device.ProductId + "' and TargetType=1)" + inroomIds + "))";
                expression = x => x.OrgId == user.OrgId || x.IsFilterLeader == false;
                expression = expression.And(x => SonSqlFun.SqlCondition(isExistDevice));
            }
            else
            {
                expression = x => x.OrgId == user.OrgId;
            }
            if (!string.IsNullOrEmpty(query.Key))
            {
                expression = expression.And(x => x.Name.Contains(query.Key));
            }
            if (query.FlowTemplateId != null)
            {
                expression = expression.And(x => x.FlowTemplateId == query.FlowTemplateId);
            }

            if (query.StartWay != null)
            {
                expression = expression.And(x => x.StartWay == query.StartWay);
            }

            if (query.CanStart == true)
            {
                var scope = await user.GetScope(this._provider, "/AfterService/DevPlane/List");
                string scopestr = string.Empty;
                if (scope != null)
                {
                    List<string> keys = new List<string>();
                    keys.Add(user.UserId.ToString());
                    string tsqlmatch = _devPlaneDAL.GetUsingDbHelp().CreateCompatible().FullSearch("rd.Helper", keys);

                    scopestr = scope.GenerateFilter("rd.DeptId", "rd.LeaderId", "rd.LeaderId=" + user.UserId + " or " + tsqlmatch, false, false, false);
                }
                string isExistStart = "IsFilterLeader=0 or (IsFilterLeader=1 and EXISTS(select 1 from mz_iot_device d left join mz_room_device_v rd on d.Id=rd.TargetId where " + scopestr + " and d.Id in (select TargetId from mz_plane_target where TargetType=0 and PlaneId=mz_plane_type.Id) or d.ProductId in (select TargetId FROM mz_plane_target where TargetType=1 and PlaneId=mz_plane_type.Id) or rd.Id in (select TargetId FROM mz_plane_target where TargetType=2 and PlaneId=mz_plane_type.Id)))";
                expression = expression.And(x => SonSqlFun.SqlCondition(isExistStart));
            }
            if (query.beginTime != null)
            {
                expression = expression.And(x => x.create_time >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And(x => x.create_time <= query.endTime);
            }
            var tpage = await _devPlaneDAL.SelectPage(expression, query, "");
            List<long> alluserIds = new List<long>();
            List<long> templateIds = new List<long>();
            foreach (var item in tpage.List)
            {
                if (item.FlowCreatedUserId > 0)
                {
                    alluserIds.Add(item.FlowCreatedUserId.Value);
                }
                templateIds.Add(item.FlowTemplateId.Value);
            }
            Dictionary<long, MZ_AdminInfo> userDicts = new Dictionary<long, MZ_AdminInfo>();
            if (alluserIds.Count > 0)
            {
                var userInfos = await _provider.GetService<UserDAL>().GetUserListByIds(alluserIds.Distinct().ToList());
                foreach (var userInfo in userInfos)
                {
                    userDicts.Add(userInfo.Id.Value, userInfo);
                }
            }
            Dictionary<long, MZ_FlowTemplate> tempDicts = new Dictionary<long, MZ_FlowTemplate>();
            if (templateIds.Count > 0)
            {
                var templates = await _provider.GetService<FlowTemplateDAL>().SelecFlowTemplateByIds(templateIds);
                foreach (var template in templates)
                {
                    tempDicts.Add(template.Id.Value, template);
                }
            }

            foreach (var item in tpage.List)
            {
                if (string.IsNullOrEmpty(item.TimerCron))
                {
                    item.TimerName = string.Empty;
                }
                else
                {
                    item.TimerName = ScheduleUtils.ToChineseDescription(item.TimerCron);
                }
                if (item.FlowCreatedUserId > 0)
                {
                    if (userDicts.TryGetValue(item.FlowCreatedUserId.Value, out MZ_AdminInfo usr))
                    {
                        item.FlowCreatedUser = usr;
                    }
                }
                if (item.FlowTemplateId > 0)
                {
                    if (tempDicts.TryGetValue(item.FlowTemplateId.Value, out MZ_FlowTemplate tp))
                    {
                        item.FlowTemplateName = tp.Name;
                        item.Icon = tp.Icon;
                        item.Background = tp.Background;
                    }
                }
            }

            return tpage;
        }
        public async Task EventToTask(DeviceEventData data)
        {
            var planeEventList = await _devPlaneDAL.QueryPlaneEventsByEvt(data.OrgId, data.OwnerOrgId, data.UseOrgId, data.EventCode);
            foreach (var planeEvent in planeEventList)
            {
                var planeType = await _devPlaneDAL.Select(planeEvent.PlaneId);
                if (planeType == null)
                {
                    continue;
                }
                var dev = await _provider.GetService<IotDeviceDAL>().Select(data.DevId);
                List<MZ_IotDevice> tlist = new List<MZ_IotDevice>();
                tlist.Add(dev);
                await _createTask(planeType, tlist, data, DateTime.Now);
            }
        }
        private async Task _createTask(MZ_PlaneType planeType, List<MZ_IotDevice> devlist, DeviceEventData data, DateTime triggerTime)
        {
            var userDAL = _provider.GetService<UserDAL>();
            var deptDAL = _provider.GetService<DeptDAL>();
            DevPlaneTaskDAL taskDAL = _provider.GetService<DevPlaneTaskDAL>();
            List<MZ_PlaneTask> planTaskList = new List<MZ_PlaneTask>();
            List<string> flowerr = new List<string>();
            try
            {
                foreach (var dev in devlist)
                {
                    if (string.IsNullOrEmpty(dev.Id))
                    {
                        continue;
                    }
                    long submitUserId = planeType.FlowCreatedUserId.Value;
                    if (submitUserId == 0)
                    {
                        var userlist = await taskDAL.QueryLeadersByDevice(planeType.OrgId.Value, dev.Id);
                        if (userlist.Count <= 0)
                        {
                            flowerr.Add("【" + dev.Name + "】找不到责任人");
                            continue;
                        }
                        submitUserId = userlist[0];
                    }
                    MZ_AdminInfo submitUser = await userDAL.GetAdminById(submitUserId);
                    long? deptId;
                    if (submitUser == null)
                    {
                        submitUserId = 2;
                        submitUser = await userDAL.GetAdminById(submitUserId);
                        var topDept = await deptDAL.SelectRoot(planeType.OrgId.Value);
                        flowerr.Add("【" + dev.Name + "】设备负责人已不存在，暂由系统用户负责！");
                        deptId = topDept.dept_id;
                    }
                    else
                    {
                        deptId = submitUser.dept_id;
                    }



                    MZ_PlaneTask task = new MZ_PlaneTask();
                    task.Id = _snowflake.NextId().ToString();
                    task.OrgId = planeType.OrgId;
                    task.PlaneNumber = await GeneratePlaneNumber();
                    task.PlanName = planeType.Name;
                    task.PlanTypeId = planeType.Id;
                    task.TaskStatus = 0;
                    task.TargetId = dev.Id;
                    task.NoticeCount = 0;
                    task.DeptId = deptId;
                    task.UserId = submitUserId;
                    task.StartOn = task.CreatedOn = triggerTime;
                    task.EndOn = planeType.PlaneDays == 0 ? task.StartOn.Value.Date.AddDays(1).AddTicks(-1) : task.StartOn.Value.Date.AddDays(planeType.PlaneDays.Value);
                    task.DispatchUserId = 0;
                    task.ExeUserId = 0;
                    task.CheckUserId = 0;

                    var flowitems = System.Text.Json.JsonSerializer.Deserialize<List<DevFlowItem>>(planeType.FlowInitJson, MyDefaultTextJsonConfig.DefaultOptions);
                    DevFlowCreate flowcreate = new DevFlowCreate();
                    flowcreate.templateId = planeType.FlowTemplateId.Value;
                    flowcreate.model = new Dictionary<string, object>();
                    flowcreate.model.Add("@from", task.PlaneNumber);
                    flowcreate.model.Add("@fromtype", "计划任务单");
                    flowcreate.model.Add("@FlowNumber", task.PlaneNumber);
                    flowcreate.UserId = submitUserId;
                    List<Out_DeviceWithRoome> roomList = null;
                    List<MZ_RoomCategory> categoryList = null;
                    List<MZ_AdminInfo> leaders = null;
                    foreach (var fitem in flowitems)
                    {
                        if (!flowcreate.model.ContainsKey(fitem.id))
                        {
                            flowcreate.model.Add(fitem.id, await fitem.GetRealValue(_provider, dev, planeType, data, submitUser, roomList, categoryList, leaders));
                        }
                    }

                    var rsp = await BusUtility.Call("NewFlowTask", flowcreate);
                    if (!rsp.IsSuccess())
                    {
                        flowerr.Add($"【{dev.Name}】流程创建异常：{rsp.Message}");
                        continue;
                    }
                    task.FlowId = rsp.GetResult<long>();
                    planTaskList.Add(task);
                }
                if (planTaskList.Count > 0)
                {
                    await taskDAL.Insert(planTaskList);
                }
            }
            catch (Exception ex)
            {
                var workflowExe = _provider.GetService<WorkflowExecutor>();
                foreach (var t in planTaskList)
                {
                    if (t.FlowId > 0)
                    {
                        await workflowExe.TerminateWorkflow(t.FlowId.Value);
                    }
                }
                flowerr.Add(ex.Message);
            }
            if (flowerr.Count > 0)
            {
                var recvList = await userDAL.SelectManUsers(planeType.OrgId.Value);
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
                var nt = new NoticeEvent(2, targets.ToArray(), new string[] { "APP" });
                nt.OrgId = planeType.OrgId.Value;
                nt.TargetType = "DevPlaneFlow";
                nt.TargetUrl = string.Empty;
                nt.Content = $"设备计划任务【{planeType.Name}】生成异常,错误内容：{flowerr.Join()}";
                nt.Label = "设备计划消息";
                await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
            }
        }
        public virtual async Task Execute(string id, DateTime triggerTime)
        {
            var planeType = await _devPlaneDAL.Select(id);
            if (planeType == null)
            {
                await PlanSchedule.DeleteJob(id);
                return;
            }
            if (planeType.ExcludeHoliday == true)
            {
                var curdate = triggerTime;
                string daystr = curdate.ToString("yyyyMMdd");
                var holidays = await _provider.GetService<HolidayOrgDAL>().SelectList(x => x.OrgId == planeType.OrgId.Value && x.DayStr == daystr);
                if (holidays.Count > 0)
                {
                    if (holidays[0].TimeWay == 0)
                    {
                        return;
                    }
                    if (holidays.Count == 2)
                    {
                        if (holidays[0].TimeWay == 1 && holidays[1].TimeWay == 2 && holidays[0].Holiday <= curdate && curdate <= holidays[1].Holiday)
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (holidays[0].TimeWay == 1 && holidays[0].Holiday <= curdate)
                        {
                            return;
                        }
                        else if (holidays[0].TimeWay == 2 && curdate <= holidays[1].Holiday)
                        {
                            return;
                        }
                    }
                }
            }

            var devlist = await _devPlaneDAL.SelectPlaneDeviceList(id);
            var proDeviceList = await _devPlaneDAL.SelectPlaneProductDeviceList(id);
            if (proDeviceList.Count > 0)
            {
                devlist.AddRange(proDeviceList);
            }
            var roomDeviceList = await _devPlaneDAL.SelectPlaneRoomDeviceList(id);
            if (roomDeviceList.Count > 0)
            {
                devlist.AddRange(roomDeviceList);
            }
            //去除重复设备
            devlist = devlist.Distinct((a, b) => a.Id == b.Id).ToList();
            await _createTask(planeType, devlist, null, triggerTime);

        }
        public virtual async Task<BusResponse<string>> Add(MZ_PlaneType data)
        {
            var user = _provider.GetUser();
            data.Id = _snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            data.ExpireNotices ??= "";
            data.IsFilterLeader ??= false;
            if (data.FlowTemplateId == null || data.FlowTemplateId < 0)
            {
                return BusResponse<string>.Error(112, "请选择流程模板");
            }
            if (data.Targets == null || data.Targets.Count == 0)
            {
                return BusResponse<string>.Error(123, "请选择计划目标");
            }
            if (data.StartWay == 1)
            {
                if (string.IsNullOrEmpty(data.TimerCron))
                {
                    return BusResponse<string>.Error(131, "Cron表达式不能为空");
                }

                await PlanSchedule.CreateJob(data.Id, new List<string>() { data.TimerCron });
            }
            else if (data.StartWay == 2)
            {
                data.TimerCron = string.Empty;
                if (data.Events == null)
                {
                    return BusResponse<string>.Error(122, "请选择设备事件");
                }
                foreach (var evt in data.Events)
                {
                    evt.PlaneId = data.Id;
                    evt.OrgId = data.OrgId;
                }
                await _devPlaneDAL.AddPlaneEvent(data.Events);
            }
            else
            {
                data.TimerCron = string.Empty;
                data.FlowCreatedUserId = 0;
            }
            data.SetCreateBy(user);

            foreach (var item in data.Targets)
            {
                item.PlaneId = data.Id;
            }
            await _devPlaneDAL.AddPlaneDevice(data.Targets);
            await _devPlaneDAL.Insert(data);
            return BusResponse<string>.Success();
        }

        public virtual async Task<BusResponse<string>> Edit(MZ_PlaneType data)
        {
            MZ_PlaneType old = await _devPlaneDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "计划不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }

            data.OrgId = null;
            data.StartWay = null;
            data.FlowTemplateId = null;
            data.SetUpdateBy(user);

            if (old.StartWay == 1 && !string.IsNullOrEmpty(data.TimerCron) && old.TimerCron != data.TimerCron)
            {
                await PlanSchedule.DeleteJob(data.Id);
                await PlanSchedule.CreateJob(data.Id, new List<string>() { data.TimerCron });
            }
            if (data.Targets != null && data.Targets.Count > 0)
            {
                await _devPlaneDAL.RemovePlaneDevice(old.Id);
                foreach (var item in data.Targets)
                {
                    item.PlaneId = data.Id;
                }
                await _devPlaneDAL.AddPlaneDevice(data.Targets);
            }
            await _devPlaneDAL.Update(data);
            return BusResponse<string>.Success();
        }

        public virtual async Task<BusResponse<string>> Delete(string id)
        {
            MZ_PlaneType old = await _devPlaneDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "计划不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }

            await _devPlaneDAL.Delete(id);
            await _devPlaneDAL.RemovePlaneDevice(id);
            await _devPlaneDAL.RemovePlaneEvent(id);
            await PlanSchedule.DeleteJob(old.Id);
            return BusResponse<string>.Success();
        }
        public virtual async Task<PageObject<MZ_IotDevice>> DevListPage(In_PlaneDevList query)
        {
            return await _devPlaneDAL.DevListPage(query);
        }
        public virtual async Task<BusResponse<MZ_PlaneType>> Info(string id, bool showTarget)
        {
            var info = await _devPlaneDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_PlaneType>.Error(123, "计划不存在");
            }
            if (showTarget)
            {
                info.Targets = await _devPlaneDAL.QueryPlaneTargets(id);
                var tmpdevs = await _devPlaneDAL.SelectPlaneDeviceList(id);
                var tmpprods = await _devPlaneDAL.SelectPlaneProductList(id);
                var tmprooms = await _devPlaneDAL.SelectPlaneRoomList(id);
                foreach (var target in info.Targets)
                {
                    if (target.TargetType == 0)
                    {
                        var tmp = tmpdevs.Where(x => x.Id == target.TargetId).FirstOrDefault();
                        if (tmp != null)
                        {
                            target.TargetName = tmp.Name;
                            target.PhotoUrl = tmp.PhotoUrl;
                        }
                    }
                    else if (target.TargetType == 1)
                    {
                        var tmp = tmpprods.Where(x => x.Id == target.TargetId).FirstOrDefault();
                        if (tmp != null)
                        {
                            target.TargetName = tmp.Name;
                            target.PhotoUrl = tmp.PhotoUrl;
                        }
                    }
                    else if (target.TargetType == 2)
                    {
                        var tmp = tmprooms.Where(x => x.Id == target.TargetId).FirstOrDefault();
                        if (tmp != null)
                        {
                            target.TargetName = tmp.Name;
                            target.PhotoUrl = string.Empty;
                        }
                    }
                }
            }

            var flowTemplate = await _provider.GetService<FlowTemplateDAL>().SelecFlowTemplateById(info.FlowTemplateId.Value);
            if (flowTemplate != null)
            {
                info.FlowTemplateName = flowTemplate.Name;
            }
            else
            {
                info.FlowTemplateName = string.Empty;
            }

            var creator = await _provider.GetService<UserDAL>().GetAdminById(info.FlowCreatedUserId.Value);
            if (creator != null)
            {
                info.FlowCreatedUser = creator;
            }

            if (info.StartWay == 2)
            {
                info.Events = await _devPlaneDAL.QueryPlaneEvents(id);
            }
            return BusResponse<MZ_PlaneType>.Success(info);
        }
    }
}

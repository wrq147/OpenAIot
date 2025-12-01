using AuthService;
using AuthService.Fields;
using Common;
using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using FlowService.Controller;
using FlowService.FlowNode;
using FlowService.FlowNode.Builder;
using FlowService.FlowNode.FormFields;
using MESService.DAL;
using MESService.Model;
using Newtonsoft.Json;
using NPOI.HSSF.Record;
using NPOI.SS.Formula.Functions;
using ProducerService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MESService.Business
{
    public class ReportBLL
    {
        private ITAServiceProvider _provider;
        public ReportBLL(ITAServiceProvider provider)
        {
            _provider = provider;
        }

        public virtual async Task<PageObject<MZ_WorkReport>> SelectByPage(In_ReportList query, IUserInfo user)
        {
            WorkReportDAL reportDAL = _provider.GetService<WorkReportDAL>();

            if (query.Items != null && query.Items.Length > 0)
            {
                var batchNoItem = query.Items.Where(x => x.field == "BatchNo" && x.compare == "等于").FirstOrDefault();
                if (batchNoItem != null)
                {
                    batchNoItem.field = "Id";
                }
            }
            var tmppage = await reportDAL.SelectByPage(query, user);
            if (tmppage.List.Count > 0)
            {
                var repIds = tmppage.List.Select(x => x.Id).ToList();
                var defectDAL = _provider.GetService<WorkDefectDAL>();
                var tmpDefects = await defectDAL.SelectList(x => repIds.Contains(x.ReportId));
                foreach (var tmpitem in tmppage.List)
                {
                    tmpitem.DefectList = tmpDefects.Where(x => x.ReportId == tmpitem.Id).ToList();
                }
            }

            return tmppage;
        }
        public virtual async Task<BusResponse<MZ_WorkReport>> Info(string id)
        {
            WorkReportDAL reportDAL = _provider.GetService<WorkReportDAL>();
            var info = await reportDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_WorkReport>.Error(111, "报工单不存在");
            }
            info.RepBat = await _provider.GetService<WorkBatchDAL>().Select(info.BatchNo);
            var wkorder = await _provider.GetService<WorkOrderDAL>().Select(info.WorkOrderId);
            if (wkorder != null)
            {
                info.WorkOrder = wkorder;
            }
            var wkoper = await _provider.GetService<OperDAL>().Select(info.OperId);
            if (wkoper != null)
            {
                info.Oper = wkoper;
            }
            var wktask = await _provider.GetService<WorkTaskDAL>().Select(info.WorkTaskId);
            if (wktask != null)
            {
                info.TaskInfo = wktask;
            }
            var workDefectDAL = _provider.GetService<WorkDefectDAL>();
            info.DefectList = await workDefectDAL.SelectList(x => x.ReportId == id);
            return BusResponse<MZ_WorkReport>.Success(info);
        }
        public virtual async Task<string> GenerateNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("WR");
        }
        public virtual async Task<BusResponse<int>> Update(MZ_WorkReport data, IUserInfo user, TAAction action)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(111, "非企业用户无法修改生产报工");
            }
            if (data.Status != 0)
            {
                return BusResponse<int>.Error(121, "状态错误，无法修改");
            }

            WorkReportDAL reportDAL = _provider.GetService<WorkReportDAL>();
            var old = await reportDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(112, "生产报工不存在");
            }



            if (data.WorkTaskId != null)
            {
                var task = await _provider.GetService<WorkTaskDAL>().Select(data.WorkTaskId);
                if (task == null)
                {
                    return BusResponse<int>.Error(122, "生产任务不存在");
                }
                else
                {
                    data.WorkOrderId = task.WorkOrderId;
                    data.OperId = task.OperId;
                    data.RouteOperId = task.RouteOperId;
                    if (task.IsFinish == true)
                    {
                        return BusResponse<int>.Error(153, "已完成的任务，无法报工");
                    }
                }
            }
            else
            {
                var task = await _provider.GetService<WorkTaskDAL>().Select(old.WorkTaskId);
                if (task == null)
                {
                    return BusResponse<int>.Error(122, "生产任务不存在");
                }
                else
                {
                    if (task.IsFinish == true)
                    {
                        return BusResponse<int>.Error(153, "已完成的任务，无法报工");
                    }
                }
            }


            if (data.RepBat != null)
            {
                var tmpOper = await _provider.GetService<OperDAL>().Select(string.IsNullOrEmpty(data.OperId) ? old.OperId : data.OperId);
                if (tmpOper == null)
                {
                    return BusResponse<int>.Error(122, "工序不存在");
                }
                HashSet<string> filterIds;
                if (!string.IsNullOrEmpty(tmpOper.ReportFields))
                {
                    var permsItems = JsonConvert.DeserializeObject<FieldPermsItem[]>(tmpOper.ReportFields);
                    filterIds = permsItems.Where(x => x.perm != "E").Select(x => x.id).ToHashSet();
                }
                else
                {
                    filterIds = new HashSet<string>();
                }
                var checkRsp = await FieldUtility.CheckEditForm(_provider, data.RepBat, user.OrgId, "报工", action, filterIds);
                if (!checkRsp.IsSuccess())
                {
                    return checkRsp;
                }

                if (string.IsNullOrEmpty(data.BatchNo))
                {
                    data.RepBat.Id = old.BatchNo;
                }
                else
                {
                    data.RepBat.Id = data.BatchNo;
                }

                data.RepBat.WorkOrderId = old.WorkOrderId;
                data.RepBat.OrgId = null;
                data.RepBat.UpdatedOn = DateTime.Now;
                await _provider.GetService<WorkBatchDAL>().Update(data.RepBat);
            }
            data.SetUpdateBy(user);
            data.DefectNum = 0;
            var workDefectDAL = _provider.GetService<WorkDefectDAL>();
            if (data.DefectList != null && data.DefectList.Count > 0)
            {
                await workDefectDAL.Delete(x => x.ReportId == data.Id);
                var snowflake = _provider.GetService<SnowflakeHelper>();
                foreach (var defect in data.DefectList)
                {
                    defect.Id = snowflake.NextId().ToString();
                    defect.OrgId = old.OrgId;
                    defect.WorkOrderId = data.WorkOrderId ?? old.WorkOrderId;
                    defect.WorkTaskId = data.WorkTaskId ?? old.WorkTaskId;
                    defect.OperId = data.OperId ?? old.OperId;
                    defect.ReportId = data.Id;
                    data.DefectNum += defect.DefectNum;
                }
                await workDefectDAL.Insert(data.DefectList);
            }

            return BusResponse<int>.Success(await reportDAL.Update(data));
        }
        public virtual async Task<BusResponse<string>> Insert(MZ_WorkReport data, IUserInfo user, TAAction action)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(111, "非企业用户无法添加生产报工");
            }
            if (string.IsNullOrEmpty(data.WorkTaskId))
            {
                return BusResponse<string>.Error(121, "生产任务不能为空");
            }

            var task = await _provider.GetService<WorkTaskDAL>().Select(data.WorkTaskId);
            if (task == null)
            {
                return BusResponse<string>.Error(122, "生产任务不存在");
            }
            if (task.IsFinish == true)
            {
                return BusResponse<string>.Error(123, "已完成的任务，无法报工");
            }

            var tmpOper = await _provider.GetService<OperDAL>().Select(data.OperId);
            if (tmpOper == null)
            {
                return BusResponse<string>.Error(124, "工序不存在");
            }

            WorkReportDAL reportDAL = _provider.GetService<WorkReportDAL>();
            var snowflake = _provider.GetService<SnowflakeHelper>();
            data.Id = snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            data.FlowId = 0;
            data.WorkOrderId = task.WorkOrderId;
            data.OperId = task.OperId;
            data.RouteOperId = task.RouteOperId;


            #region 校验发布权限
            if (!string.IsNullOrEmpty(tmpOper.AssignedUser))
            {
                var userDAL = _provider.GetService<UserDAL>();
                var deptDAL = _provider.GetService<DeptDAL>();
                ObjData[] commitObjs = Newtonsoft.Json.JsonConvert.DeserializeObject<ObjData[]>(tmpOper.AssignedUser);
                MZ_AdminInfo userInfo = await userDAL.GetAdminByOrgId(user.UserId, user.OrgId);
                if (userInfo == null)
                {
                    return BusResponse<string>.Error(299, "报工人不存在");
                }
                MZ_Dept deptInfo = await deptDAL.SelectById(userInfo.dept_id.Value);
                bool canCommit = false;
                string[] depids = deptInfo.ancestors.Split(",", StringSplitOptions.RemoveEmptyEntries);
                HashSet<long> dephs = new HashSet<long>();
                foreach (string depid in depids)
                {
                    dephs.Add(long.Parse(depid));
                }

                foreach (ObjData obj in commitObjs)
                {
                    if (obj.type == "user" && obj.id == user.UserId)
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
                    return BusResponse<string>.Error(300, $"无权限报工工序'{tmpOper.OperName}'");
                }
            }

            #endregion


            if (data.RepBat != null)
            {
                HashSet<string> filterIds;
                if (!string.IsNullOrEmpty(tmpOper.ReportFields))
                {
                    var permsItems = JsonConvert.DeserializeObject<FieldPermsItem[]>(tmpOper.ReportFields);
                    filterIds = permsItems.Where(x => x.perm != "E").Select(x => x.id).ToHashSet();
                }
                else
                {
                    filterIds = new HashSet<string>();
                }
                var checkRsp = await FieldUtility.CheckAddForm(_provider, data.RepBat, user.OrgId, "报工", action, filterIds);
                if (!checkRsp.IsSuccess())
                {
                    return checkRsp;
                }
            }
            else
            {
                data.RepBat = new MZ_WorkBatch();

            }
            data.RepBat.Id = data.BatchNo;
            data.RepBat.WorkOrderId = data.WorkOrderId;
            data.RepBat.OrgId = user.OrgId;
            var wkbatchDAL = _provider.GetService<WorkBatchDAL>();
            if (await wkbatchDAL.Some(x => x.Id == data.BatchNo && x.OrgId == data.RepBat.OrgId))
            {
                data.RepBat.UpdatedOn = DateTime.Now;
                await wkbatchDAL.Update(data.RepBat);
            }
            else
            {
                data.RepBat.CreatedOn = DateTime.Now;
                data.RepBat.UpdatedOn = data.RepBat.CreatedOn;
                await wkbatchDAL.Insert(data.RepBat);
            }
            data.SetCreateBy(user);
            data.Status = 0;
            data.DefectNum = 0;
            if (data.DefectList != null && data.DefectList.Count > 0)
            {
                foreach (var defect in data.DefectList)
                {
                    defect.Id = snowflake.NextId().ToString();
                    defect.OrgId = data.OrgId;
                    defect.WorkOrderId = data.WorkOrderId;
                    defect.WorkTaskId = data.WorkTaskId;
                    defect.OperId = data.OperId;
                    defect.ReportId = data.Id;
                    data.DefectNum += defect.DefectNum;
                }
                await _provider.GetService<WorkDefectDAL>().Insert(data.DefectList);
            }

            await reportDAL.Insert(data);

            return BusResponse<string>.Success(data.Id);
        }
        public virtual async Task<BusResponse<int>> Delete(string id, IUserInfo user)
        {
            WorkReportDAL reportDAL = _provider.GetService<WorkReportDAL>();
            var old = await reportDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(111, "生产报工不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<int>.Error(112, "所属组织错误");
            }
            if (old.Status != 0 && old.Status != 3 && old.Status != 4)
            {
                return BusResponse<int>.Error(125, "状态错误");
            }
            var rs = await reportDAL.Delete(id);
            if (old.FlowId > 0)
            {
                await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(old.FlowId.Value);
            }
            await _provider.GetService<WorkDefectDAL>().Delete(x => x.ReportId == id);
            return BusResponse<int>.Success(rs);
        }


        public virtual async Task<BusResponse<string>> SubmitModel(In_SubmitReport data, IUserInfo user)
        {
            WorkReportDAL reportDAL = _provider.GetService<WorkReportDAL>();
            var old = await reportDAL.Select(data.id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "生产报工不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "所属组织错误");
            }
            if (old.Status != 0)
            {
                return BusResponse<string>.Error(125, "状态错误");
            }

            MZ_ProductOper operInfo = await _provider.GetService<OperDAL>().Select(old.OperId);
            if (operInfo == null)
            {
                return BusResponse<string>.Error(126, "工序不存在");
            }
            MZ_ProductRouteOper routeOper = await _provider.GetService<RouteOperDAL>().Select(old.RouteOperId);
            if (routeOper == null)
            {
                return BusResponse<string>.Error(127, "工艺路线未指定正确工序");
            }
            var needWorkTime = routeOper.WorkTime.Value * routeOper.PropOf.Value * (old.GoodNum.Value + old.DefectNum.Value);
            if (needWorkTime < old.WorkTime && string.IsNullOrEmpty(old.OverReason))
            {
                return BusResponse<string>.Error(128, $"报工时长超过标准时间{old.WorkTime - needWorkTime}分钟，请填写超时原因");
            }
            var mesConfig = await _provider.GetService<FactoryMesDAL>().Select(user.OrgId);
            if (mesConfig != null && mesConfig.PlanTemplateId > 0)
            {
                List<ProdReportFlowItem> flowItems;
                if (string.IsNullOrEmpty(mesConfig.ReportFlowInitJson))
                {
                    flowItems = new List<ProdReportFlowItem>();
                }
                else
                {
                    flowItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProdReportFlowItem>>(mesConfig.ReportFlowInitJson);
                }

                ProdReportFlowCreate flowcreate = new ProdReportFlowCreate();
                flowcreate.templateId = mesConfig.ReportTemplateId.Value;
                flowcreate.model = data.model;
                flowcreate.assign = data.assign;
                if (!data.model.ContainsKey("@from"))
                {
                    flowcreate.model.Add("@from", old.Number);
                }
                if (!data.model.ContainsKey("@fromtype"))
                {
                    flowcreate.model.Add("@fromtype", "生产报工");
                }
                if (!data.model.ContainsKey("@FlowNumber"))
                {
                    flowcreate.model.Add("@FlowNumber", old.Number);
                }

                flowcreate.UserId = user.UserId;
                flowcreate.flowId = data.flowId;
                var sumbitUser = await _provider.GetService<UserDAL>().Select(user.UserId);
                MZ_Dept sumbitDept = null;
                if (sumbitUser != null)
                {
                    sumbitDept = await _provider.GetService<DeptDAL>().Select(sumbitUser.dept_id);
                }
                foreach (var fitem in flowItems)
                {
                    if (!flowcreate.model.ContainsKey(fitem.id))
                    {
                        flowcreate.model.Add(fitem.id, fitem.GetRealValue(old, operInfo, sumbitUser, sumbitDept));
                    }
                }
                var fcrsp = await BusUtility.Call("NewFlowTask", flowcreate);
                var brs = fcrsp.GetResult<BusResponse<long>>();
                if (!brs.IsSuccess())
                {
                    return BusResponse<string>.Error(144, brs.Message);
                }
                old.FlowId = brs.Data;
            }

            try
            {
                if (old.FlowId > 0)
                {

                    MZ_WorkReport newReport = new MZ_WorkReport();
                    newReport.Id = old.Id;
                    newReport.FlowId = old.FlowId;
                    newReport.Status = 1;
                    newReport.submitTime = DateTime.Now;
                    await reportDAL.Update(newReport);
                }
                else
                {
                    MZ_WorkReport newReport = new MZ_WorkReport();
                    newReport.Id = old.Id;
                    newReport.Status = 2;
                    newReport.submitTime = DateTime.Now;
                    await reportDAL.Update(newReport);

                    //报工成功，计算任务进度
                    old.Status = 2;
                    await this._provider.GetService<WorkTaskBLL>().ResetTaskInfo(old.WorkTaskId, old);
                }
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                if (old.FlowId > 0)
                {
                    await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(old.FlowId.Value);
                }
                return BusResponse<string>.Error(322, ex.Message);
            }
        }

        public virtual async Task<BusResponse<string>> Cancel(string id, IUserInfo user)
        {
            WorkReportDAL reportDAL = _provider.GetService<WorkReportDAL>();
            var old = await reportDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "生产报工不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<string>.Error(124, "所属组织错误");
            }
            if (old.Status != 1)
            {
                return BusResponse<string>.Error(125, "状态错误");
            }
            if (old.FlowId > 0)
            {
                await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(old.FlowId.Value);
            }
            MZ_WorkReport newReport = new MZ_WorkReport();
            newReport.Id = old.Id;
            newReport.Status = 5;
            await reportDAL.Update(newReport);
            return BusResponse<string>.Success();
        }

        public virtual async Task<BusResponse<Dictionary<string, object>>> CreateFlowForm(MZ_WorkReport data, IUserInfo user)
        {
            var mesConfig = await _provider.GetService<FactoryMesDAL>().Select(user.OrgId);
            if (mesConfig == null || mesConfig.ReportTemplateId < 1)
            {
                return BusResponse<Dictionary<string, object>>.Error(1, "未设置生产报工审核流程");
            }

            if (string.IsNullOrEmpty(mesConfig.ReportFlowInitJson))
            {
                return BusResponse<Dictionary<string, object>>.Success(new Dictionary<string, object>());
            }
            var sumbitUser = await _provider.GetService<UserDAL>().Select(user.UserId);
            MZ_Dept sumbitDept = null;
            if (sumbitUser != null)
            {
                sumbitDept = await _provider.GetService<DeptDAL>().Select(sumbitUser.dept_id);
            }
            MZ_ProductOper operInfo = await _provider.GetService<OperDAL>().Select(data.OperId);
            if (operInfo == null)
            {
                return BusResponse<Dictionary<string, object>>.Error(112, "工序不存在");
            }
            var flowitems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProdReportFlowItem>>(mesConfig.ReportFlowInitJson);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var fitem in flowitems)
            {
                dict.Add(fitem.id, fitem.GetRealValue(data, operInfo, sumbitUser, sumbitDept));
            }
            return BusResponse<Dictionary<string, object>>.Success(dict);
        }
    }
}

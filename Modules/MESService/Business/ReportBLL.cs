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
            return await reportDAL.SelectByPage(query, user);
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
                data.RepBat.OrgId = old.OrgId;
                await _provider.GetService<WorkBatchDAL>().CreateOrUpdate(data.RepBat);
            }
            data.SetUpdateBy(user);
            return BusResponse<int>.Success(await reportDAL.Update(data));
        }
        public virtual async Task<BusResponse<string>> Insert(MZ_WorkReport data, IUserInfo user, TAAction action)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(111, "非企业用户无法添加生产报工");
            }

            if (string.IsNullOrEmpty(data.OperId))
            {
                return BusResponse<string>.Error(121, "工序不能为空");
            }

            var tmpOper = await _provider.GetService<OperDAL>().Select(data.OperId);
            if (tmpOper == null)
            {
                return BusResponse<string>.Error(122, "工序不存在");
            }

            WorkReportDAL reportDAL = _provider.GetService<WorkReportDAL>();
            var snowflake = _provider.GetService<SnowflakeHelper>();
            data.Id = snowflake.NextId().ToString();
            data.OrgId = user.OrgId;




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

                data.RepBat.Id = data.BatchNo;
                data.RepBat.WorkOrderId = data.WorkOrderId;
                data.RepBat.OrgId = user.OrgId;
                await _provider.GetService<WorkBatchDAL>().CreateOrUpdate(data.RepBat);
            }
            data.SetCreateBy(user);
            data.Status = 0;
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
                        flowcreate.model.Add(fitem.id, fitem.GetRealValue(old, sumbitUser, sumbitDept));
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
                    await reportDAL.Update(newReport);
                }
                else
                {
                    MZ_WorkReport newReport = new MZ_WorkReport();
                    newReport.Id = old.Id;
                    newReport.Status = 2;
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

            var flowitems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProdReportFlowItem>>(mesConfig.ReportFlowInitJson);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var fitem in flowitems)
            {
                dict.Add(fitem.id, fitem.GetRealValue(data, sumbitUser, sumbitDept));
            }
            return BusResponse<Dictionary<string, object>>.Success(dict);
        }
    }
}

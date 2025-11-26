using AuthService;
using Common;
using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using FlowService.FlowNode.Builder;
using MESService.DAL;
using MESService.Model;
using MyAccess.Aop;
using ProducerService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MESService.Business
{
    public class PlanBLL
    {
        private ITAServiceProvider _provider;
        public PlanBLL(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        public virtual async Task<string> GenerateNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("PP");
        }
        public virtual async Task<PageObject<MZ_ProductPlan>> SelectList(In_PlanList query, IUserInfo user)
        {
            ProductPlanDAL productPlaneDAL = _provider.GetService<ProductPlanDAL>();
            return await productPlaneDAL.SelectByPage(query, user.OrgId);
        }

        public virtual async Task<BusResponse<MZ_ProductPlan>> Info(string id)
        {
            ProductPlanDAL productPlanDAL = _provider.GetService<ProductPlanDAL>();
            ProductPlanItemDAL productPlanItemDAL = _provider.GetService<ProductPlanItemDAL>();
            var info = await productPlanDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_ProductPlan>.Error(111, "生产计划不存在");
            }
            info.Items = await productPlanItemDAL.SelectList(x => x.PlanId == id);
            var prodIds = info.Items.Select(x => x.ProductId).ToList();

            if (prodIds.Count > 0)
            {
                var prolist = await _provider.GetService<ProductDAL>().SelectList(x => prodIds.Contains(x.Id));
                foreach (var pro in prolist)
                {
                    var proitem = info.Items.Where(x => x.ProductId == pro.Id).FirstOrDefault();
                    if (proitem != null)
                    {
                        proitem.ProdInfo = pro;
                    }
                }
            }


            return BusResponse<MZ_ProductPlan>.Success(info);
        }
        public virtual async Task<BusResponse<int>> Update(MZ_ProductPlan data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(111, "非企业用户无法修改生产计划");
            }
            if (data.Status != 0)
            {
                return BusResponse<int>.Error(121, "状态错误，无法修改");
            }
            ProductPlanDAL productPlanDAL = _provider.GetService<ProductPlanDAL>();
            var old = await productPlanDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(112, "生产计划不存在");
            }

            var snowflake = _provider.GetService<SnowflakeHelper>();
            ProductPlanItemDAL productPlanItemDAL = _provider.GetService<ProductPlanItemDAL>();
            await productPlanItemDAL.Delete(x => x.PlanId == data.Id);
            if (data.Items != null && data.Items.Count > 0)
            {
                foreach (var item in data.Items)
                {
                    item.OrgId = user.OrgId;
                    item.Id = snowflake.NextId().ToString();
                    item.PlanId = data.Id;
                }
                await productPlanItemDAL.Insert(data.Items);
            }
            data.Status = null;
            data.OrgId = null;
            data.SetUpdateBy(user);
            return BusResponse<int>.Success(await productPlanDAL.Update(data));
        }
        public virtual async Task<BusResponse<string>> Insert(MZ_ProductPlan data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(111, "非企业用户无法添加生产计划");
            }

            var snowflake = _provider.GetService<SnowflakeHelper>();
            data.Id = snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            if (data.Items != null && data.Items.Count > 0)
            {
                ProductPlanItemDAL productPlanItemDAL = _provider.GetService<ProductPlanItemDAL>();
                foreach (var item in data.Items)
                {
                    item.OrgId = user.OrgId;
                    item.Id = snowflake.NextId().ToString();
                    item.PlanId = data.Id;
                    if (item.PlannedStartOn == null)
                    {
                        return BusResponse<string>.Error(121, "计划开始时间不能为空");
                    }
                    if (item.PlannedEndOn == null)
                    {
                        return BusResponse<string>.Error(121, "计划结束时间不能为空");
                    }
                }
                await productPlanItemDAL.Insert(data.Items);
            }
            data.SetCreateBy(user);
            data.Status = 0;
            await _provider.GetService<ProductPlanDAL>().Insert(data);
            return BusResponse<string>.Success(data.Id);
        }
        public virtual async Task<BusResponse<int>> Delete(string id, IUserInfo user)
        {
            ProductPlanDAL productPlanDAL = _provider.GetService<ProductPlanDAL>();
            var old = await productPlanDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(111, "生产计划不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<int>.Error(112, "所属组织错误");
            }
            if (old.Status != 0 && old.Status != 6 && old.Status != 5)
            {
                return BusResponse<int>.Error(125, "状态错误");
            }
            var rs = await productPlanDAL.Delete(id);
            if (old.FlowId > 0)
            {
                await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(old.FlowId.Value);
            }
            ProductPlanItemDAL productPlanItemDAL = _provider.GetService<ProductPlanItemDAL>();
            await productPlanItemDAL.Delete(x => x.PlanId == id);
            return BusResponse<int>.Success(rs);
        }


        public virtual async Task<BusResponse<string>> SubmitModel(In_SubmitPlan data, IUserInfo user)
        {
            ProductPlanDAL productPlanDAL = _provider.GetService<ProductPlanDAL>();
            var old = await productPlanDAL.Select(data.id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "生产计划不存在");
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
                List<ProdPlanFlowItem> flowItems;
                if (string.IsNullOrEmpty(mesConfig.PlanFlowInitJson))
                {
                    flowItems = new List<ProdPlanFlowItem>();
                }
                else
                {
                    flowItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProdPlanFlowItem>>(mesConfig.PlanFlowInitJson);
                }
                ProdPlanFlowCreate flowcreate = new ProdPlanFlowCreate();
                flowcreate.templateId = mesConfig.PlanTemplateId.Value;
                flowcreate.model = data.model;
                flowcreate.assign = data.assign;
                if (!data.model.ContainsKey("@from"))
                {
                    flowcreate.model.Add("@from", old.Number);
                }
                if (!data.model.ContainsKey("@fromtype"))
                {
                    flowcreate.model.Add("@fromtype", "生产计划");
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

                    MZ_ProductPlan plan = new MZ_ProductPlan();
                    plan.Id = old.Id;
                    plan.FlowId = old.FlowId;
                    plan.Status = 1;
                    await productPlanDAL.Update(plan);
                }
                else
                {
                    MZ_ProductPlan plan = new MZ_ProductPlan();
                    plan.Id = old.Id;
                    plan.Status = 2;
                    await productPlanDAL.Update(plan);

                    //生成生产工单
                    old.Items = await _provider.GetService<ProductPlanItemDAL>().SelectList(x => x.PlanId == old.Id);
                    await _provider.GetService<WorkOrderBLL>().GenerateWorkOrder(old);
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
            ProductPlanDAL productPlanDAL = _provider.GetService<ProductPlanDAL>();
            var old = await productPlanDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "生产计划不存在");
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
            MZ_ProductPlan plan = new MZ_ProductPlan();
            plan.Id = old.Id;
            plan.Status = 5;
            await productPlanDAL.Update(plan);
            return BusResponse<string>.Success();
        }

        public virtual async Task<BusResponse<Dictionary<string, object>>> CreateFlowForm(MZ_ProductPlan data, IUserInfo user)
        {
            var mesConfig = await _provider.GetService<FactoryMesDAL>().Select(user.OrgId);
            if (mesConfig == null || mesConfig.PlanTemplateId < 1)
            {
                return BusResponse<Dictionary<string, object>>.Error(1, "未设置生产计划审核流程");
            }

            if (string.IsNullOrEmpty(mesConfig.PlanFlowInitJson))
            {
                return BusResponse<Dictionary<string, object>>.Success(new Dictionary<string, object>());
            }
            var sumbitUser = await _provider.GetService<UserDAL>().Select(user.UserId);
            MZ_Dept sumbitDept = null;
            if (sumbitUser != null)
            {
                sumbitDept = await _provider.GetService<DeptDAL>().Select(sumbitUser.dept_id);
            }

            var flowitems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProdPlanFlowItem>>(mesConfig.PlanFlowInitJson);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var fitem in flowitems)
            {
                dict.Add(fitem.id, fitem.GetRealValue(data, sumbitUser, sumbitDept));
            }
            return BusResponse<Dictionary<string, object>>.Success(dict);
        }
    }
}

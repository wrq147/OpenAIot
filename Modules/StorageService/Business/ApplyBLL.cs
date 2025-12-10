using AuthService;
using Common.IdGenerator;
using Common.Share;
using StorageService.DAL;
using StorageService.Model;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TemplateAction.Core;
using MyAccess.DB.Builder.WhereToSql;
using Common;
using Common.EventBus;
using FlowService.FlowNode.Builder;
using System.Collections.Generic;
using System.Linq;
using MyAccess.Aop;
using ProducerService.DAL;
using ProducerService.Model;

namespace StorageService.Business
{
    public class ApplyBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private LeaveApplyDAL _leaveApplyDAL;
        private LeaveApplyDetailDAL _leaveApplyDetailDAL;
        public ApplyBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, LeaveApplyDAL leaveApplyDAL, LeaveApplyDetailDAL leaveApplyDetailDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _leaveApplyDAL = leaveApplyDAL;
            _leaveApplyDetailDAL = leaveApplyDetailDAL;
        }

        public virtual async Task<BusResponse<Dictionary<string, object>>> CreateTaskForm(MZ_LeaveApply apply)
        {
            if (string.IsNullOrEmpty(apply.HouseId))
            {
                return BusResponse<Dictionary<string, object>>.Error(109, "请输入出库仓库");
            }
            var houseInfo = await _provider.GetService<StoreHouseDAL>().Select(apply.HouseId);
            if (houseInfo == null)
            {
                return BusResponse<Dictionary<string, object>>.Error(110, "仓库不存在");
            }
            if (string.IsNullOrEmpty(houseInfo.LeaveApplyFlowInitJson))
            {
                return BusResponse<Dictionary<string, object>>.Success(new Dictionary<string, object>());
            }
            var flowitems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LeaveApplyFlowItem>>(houseInfo.LeaveApplyFlowInitJson);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var fitem in flowitems)
            {
                dict.Add(fitem.id, fitem.GetRealValue(apply));
            }
            return BusResponse<Dictionary<string, object>>.Success(dict);
        }

        public async Task<string> GenerateApplyNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("LA");
        }
        public virtual async Task<int> WaitCount(IUserInfo user, string houseId = "")
        {
            Expression<Func<MZ_LeaveApply, bool>> expression = x => x.OrgId == user.OrgId && x.Status == 2 && x.OutStatus == 0;
            if (!string.IsNullOrEmpty(houseId))
            {
                expression = expression.And(x => x.HouseId == houseId);
            }
            return await _leaveApplyDAL.Count(expression);
        }
        public virtual async Task<PageObject<MZ_LeaveApply>> SelectByPage(In_LeaveApplyList query, IUserInfo user)
        {
            Expression<Func<MZ_LeaveApply, bool>> expression = x => x.OrgId == user.OrgId;
            if (query.OnlyMy == true)
            {
                expression = expression.And(x => x.ApplyUserId == user.UserId);
            }
            if (!string.IsNullOrEmpty(query.Key))
            {
                string isexistGroup = "EXISTS(select 1 from mz_leave_apply_detail ad left join mz_stockrecord_v sp on ad.HouseId=sp.HouseId and ad.TargetType=sp.TargetType and ad.TargetId=sp.TargetId where ad.OrgId=" + user.OrgId + " and sp.Name like '%" + StringHelper.SqlLikeFilter(query.Key) + "%' and ad.ApplyId=mz_leave_apply.Id)";
                expression = expression.And(x => x.ApplyNumber.StartsWith(query.Key) || SonSqlFun.SqlCondition(isexistGroup));
            }
            if (query.Status != null)
            {
                expression = expression.And(x => x.Status == query.Status);
            }
            if (query.OutStatus != null)
            {
                expression = expression.And(x => x.OutStatus == query.OutStatus);
            }
            if (!string.IsNullOrEmpty(query.HouseId))
            {
                expression = expression.And(x => x.HouseId == query.HouseId);
            }
            if (query.beginTime != null)
            {
                expression = expression.And(x => x.ApplyOn >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And(x => x.ApplyOn <= query.endTime);
            }
            var pagelist = await _leaveApplyDAL.SelectPage(expression, query, "ApplyOn desc");

            var housedict = await _provider.GetService<StoreHouseDAL>().NavigateDict<MZ_LeaveApply, string>(pagelist.List, x => !string.IsNullOrEmpty(x.HouseId), x => x.HouseId);
            var userdict = await _provider.GetService<UserDAL>().NavigateDict<MZ_LeaveApply, long>(pagelist.List, x => x.ApplyUserId != null && x.ApplyUserId > 0, x => x.ApplyUserId.Value);
            foreach (var item in pagelist.List)
            {
                if (housedict.TryGetValue(item.HouseId, out MZ_StoreHouse house))
                {
                    item.House = house;
                }
                if (userdict.TryGetValue(item.ApplyUserId.Value, out MZ_AdminInfo tmpuser))
                {
                    item.ApplyUserInfo = tmpuser;
                }
            }
            #region 产品批次初始化
            if (query.ShowDetail == true)
            {
                List<string> applyIds = pagelist.List.Select(x => x.Id).ToList();
                if (applyIds.Count > 0)
                {
                    List<MZ_LeaveApplyDetail> tldlist = await _leaveApplyDetailDAL.SelectList(x => applyIds.Contains(x.ApplyId));
                    var prolist = await _provider.GetService<ProductBatchDAL>().SelectListByIds(tldlist.Select(x => x.TargetId).ToList());
                    foreach (var item in tldlist)
                    {
                        var tmppro = prolist.Where(x => x.Id == item.TargetId).FirstOrDefault();
                        if (tmppro != null)
                        {
                            item.TargetNumber = tmppro.Number;
                            item.TargetName = tmppro.BatchName;
                            item.PhotoUrl = tmppro.PhotoUrl;
                        }
                    }

                    foreach (var item in pagelist.List)
                    {
                        item.List = tldlist.Where(x => x.ApplyId == item.Id).ToList();
                    }
                }
            }
            #endregion

            return pagelist;
        }
        public virtual async Task<BusResponse<MZ_LeaveApply>> Info(string id)
        {
            MZ_LeaveApply applyInfo = await _leaveApplyDAL.Select(id);
            if (applyInfo == null)
            {
                return BusResponse<MZ_LeaveApply>.Error(121, "申请单不存在");
            }

            applyInfo.House = await _provider.GetService<StoreHouseDAL>().Select(applyInfo.HouseId);
            applyInfo.List = await _leaveApplyDetailDAL.SelectList(x => x.ApplyId == applyInfo.Id);
            applyInfo.ApplyUserInfo = await _provider.GetService<UserDAL>().GetAdminById(applyInfo.ApplyUserId.Value);


            #region 产品批次初始化
            var prolist = await _provider.GetService<ProductBatchDAL>().SelectListByIds(applyInfo.List.Select(x => x.TargetId).ToList());
            foreach (var item in applyInfo.List)
            {
                var tmppro = prolist.Where(x => x.Id == item.TargetId).FirstOrDefault();
                if (tmppro != null)
                {
                    item.TargetNumber = tmppro.Number;
                    item.TargetName = tmppro.BatchName;
                    item.PhotoUrl = tmppro.PhotoUrl;
                }
            }
            #endregion

            return BusResponse<MZ_LeaveApply>.Success(applyInfo);
        }
        public virtual async Task<BusResponse<MZ_LeaveApply>> InfoByNumber(string number)
        {
            var applyList = await _leaveApplyDAL.SelectList(x => x.ApplyNumber == number);
            if (applyList.Count <= 0)
            {
                return BusResponse<MZ_LeaveApply>.Error(4, "申请单不存在");
            }
            applyList[0].List = await _leaveApplyDetailDAL.SelectList(x => x.ApplyId == applyList[0].Id);
            var applyInfo = applyList[0];
            applyInfo.House = await _provider.GetService<StoreHouseDAL>().Select(applyInfo.HouseId);
            applyInfo.ApplyUserInfo = await _provider.GetService<UserDAL>().GetAdminById(applyInfo.ApplyUserId.Value);

            #region 产品批次初始化
            var prolist = await _provider.GetService<ProductBatchDAL>().SelectListByIds(applyInfo.List.Select(x => x.TargetId).ToList());
            foreach (var item in applyInfo.List)
            {
                var tmppro = prolist.Where(x => x.Id == item.TargetId).FirstOrDefault();
                if (tmppro != null)
                {
                    item.TargetNumber = tmppro.Number;
                    item.TargetName = tmppro.BatchName;
                    item.PhotoUrl = tmppro.PhotoUrl;
                }
            }
            #endregion


            return BusResponse<MZ_LeaveApply>.Success(applyList[0]);
        }

        public virtual async Task<BusResponse<string>> Add(MZ_LeaveApply data, IUserInfo user)
        {
            data.Id = _snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            if (string.IsNullOrEmpty(data.ApplyNumber))
            {
                data.ApplyNumber = await GenerateApplyNumber();
            }
            data.ApplyOn = DateTime.Now;

            if (data.ApplyUserId == null)
            {
                return BusResponse<string>.Error(110, "申请人不能为空");
            }
            var userOrg = await _provider.GetService<OrgDAL>().SelectUserOrg(data.ApplyUserId.Value, user.OrgId);
            data.ApplyDeptId = userOrg.dept_id;
            data.Status = 0;
            data.OutStatus = 0;
            if (string.IsNullOrEmpty(data.HouseId))
            {
                return BusResponse<string>.Error(111, "请选择出库仓库");
            }
            foreach (var item in data.List)
            {
                item.ApplyId = data.Id;
                item.OrgId = data.OrgId;
                item.Id = MyAccess.Core.StringTool.GetGUID();
                item.HouseId = data.HouseId;
            }
            using (BLLTranScope scope = new BLLTranScope())
            {
                await _leaveApplyDAL.Insert(data);
                await _leaveApplyDetailDAL.Insert(data.List);
                // 完成
                await scope.CompleteAsync();
            }

            return BusResponse<string>.Success(data.Id);
        }

        public virtual async Task<BusResponse<string>> Edit(MZ_LeaveApply data, IUserInfo user)
        {
            MZ_LeaveApply old = await _leaveApplyDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "申请单不存在");
            }

            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }
            data.ApplyOn = DateTime.Now;
            data.Status = null;
            data.OutStatus = null;
            data.ApplyUserId = null;
            data.ApplyDeptId = null;
            bool isUpdateList = false;
            if (data.List != null)
            {
                foreach (var item in data.List)
                {
                    item.ApplyId = data.Id;
                    item.OrgId = data.OrgId;
                    item.Id = MyAccess.Core.StringTool.GetGUID();
                    item.HouseId = data.HouseId;
                }
            }
            if (isUpdateList)
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    await _leaveApplyDAL.Update(data);
                    await _leaveApplyDetailDAL.Delete(x => x.OrgId == old.OrgId && x.ApplyId == old.Id);
                    await _leaveApplyDetailDAL.Insert(data.List);
                    // 完成
                    await scope.CompleteAsync();
                }

            }
            else
            {
                await _leaveApplyDAL.Update(data);
            }

            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<string>> SubmitModel(In_SubmitStock data, IUserInfo user)
        {
            MZ_LeaveApply old = await _leaveApplyDAL.Select(data.id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "申请单不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }
            if (old.Status != 0)
            {
                return BusResponse<string>.Error(125, "状态错误");
            }

            var useHouse = await _provider.GetService<StoreHouseDAL>().Select(old.HouseId);
            if (useHouse == null)
            {
                return BusResponse<string>.Error(126, "仓库不存在");
            }

            if (useHouse.LeaveApplyTemplateId != null && useHouse.LeaveApplyTemplateId > 0)
            {
                List<LeaveApplyFlowItem> flowItems;
                if (string.IsNullOrEmpty(useHouse.LeaveApplyFlowInitJson))
                {
                    flowItems = new List<LeaveApplyFlowItem>();
                }
                else
                {
                    flowItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LeaveApplyFlowItem>>(useHouse.LeaveApplyFlowInitJson);
                }
                LeaveApplyFlowCreate flowcreate = new LeaveApplyFlowCreate();
                flowcreate.templateId = useHouse.LeaveApplyTemplateId.Value;
                flowcreate.model = data.model;
                flowcreate.assign = data.assign;
                if (!data.model.ContainsKey("@from"))
                {
                    flowcreate.model.Add("@from", old.ApplyNumber);
                }
                if (!data.model.ContainsKey("@fromtype"))
                {
                    flowcreate.model.Add("@fromtype", "出库申请单");
                }
                if (!data.model.ContainsKey("@FlowNumber"))
                {
                    flowcreate.model.Add("@FlowNumber", old.ApplyNumber);
                }

                flowcreate.UserId = user.UserId;
                flowcreate.flowId = data.flowId;

                foreach (var fitem in flowItems)
                {
                    if (!flowcreate.model.ContainsKey(fitem.id))
                    {
                        flowcreate.model.Add(fitem.id, fitem.GetRealValue(old));
                    }
                }
                var fcrsp = await BusUtility.Call("NewFlowTask", flowcreate);
                if (!fcrsp.IsSuccess())
                {
                    return BusResponse<string>.Error(144, fcrsp.Message);
                }
                old.FlowId = fcrsp.GetResult<long>();
            }

            try
            {
                if (old.FlowId > 0)
                {
                    MZ_LeaveApply apply = new MZ_LeaveApply();
                    apply.Id = old.Id;
                    apply.FlowId = old.FlowId;
                    apply.Status = 1;
                    await _leaveApplyDAL.Update(apply);
                }
                else
                {
                    MZ_LeaveApply apply = new MZ_LeaveApply();
                    apply.Id = old.Id;
                    apply.Status = 2;
                    await _leaveApplyDAL.Update(apply);
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
            MZ_LeaveApply old = await _leaveApplyDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "申请单不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }
            if (old.Status != 1)
            {
                return BusResponse<string>.Error(125, "状态错误");
            }
            if (old.FlowId > 0)
            {
                await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(old.FlowId.Value);
            }
            MZ_LeaveApply apply = new MZ_LeaveApply();
            apply.Id = old.Id;
            apply.Status = 4;
            await _leaveApplyDAL.Update(apply);
            return BusResponse<string>.Success();
        }

        public virtual async Task<BusResponse<string>> Remove(string id, IUserInfo user)
        {
            MZ_LeaveApply old = await _leaveApplyDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "申请单不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }
            if (old.Status != 0 && old.Status != 3 && old.Status != 4)
            {
                return BusResponse<string>.Error(125, "状态错误");
            }
            if (old.FlowId > 0)
            {
                await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(old.FlowId.Value);
            }
            await _leaveApplyDAL.Delete(old.Id);
            return BusResponse<string>.Success();
        }
    }
}

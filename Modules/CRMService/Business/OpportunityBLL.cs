using AuthService;
using Common;
using Common.IdGenerator;
using Common.Share;
using CRMService.DAL;
using CRMService.Model;
using DiscussService.DAL;
using DiscussService.Model;
using Microsoft.Extensions.Options;
using MyAccess.Aop;
using ProducerService.DAL;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace CRMService.Business
{
    public class OpportunityBLL
    {
        private IOptions<GeneralOption> _conf;
        private ITAServiceProvider _provider;
        private OperatorHelper _operator;
        private OpportunityDAL _opportunityDAL;
        private FollowDAL _followDAL;
        private OpportDetailDAL _opportDetailDAL;
        private SnowflakeHelper _snowflake;
        private PeriodDAL _periodDAL;
        private OrgDAL _orgDAL;
        private SubjectDAL _subjectDAL;
        private UserDAL _userDAL;
        private CustomerDAL _customerDAL;
        public OpportunityBLL(ITAServiceProvider provider, IOptions<GeneralOption> conf, OperatorHelper operatorHelper, OpportunityDAL opportunity,
            FollowDAL followDAL, OpportDetailDAL opportDetailDAL, PeriodDAL periodDAL, SnowflakeHelper snowflake, OrgDAL orgDAL,
            UserDAL userDAL, SubjectDAL subjectDAL, CustomerDAL customerDAL)
        {
            _provider = provider;
            _conf = conf;
            _operator = operatorHelper;
            _opportunityDAL = opportunity;
            _followDAL = followDAL;
            _opportDetailDAL = opportDetailDAL;
            _snowflake = snowflake;
            _periodDAL = periodDAL;
            _orgDAL = orgDAL;
            _userDAL = userDAL;
            _subjectDAL = subjectDAL;
            _customerDAL = customerDAL;
        }
        public virtual async Task<BusResponse<MZ_Opportunity>> Info(string id)
        {
            var info = await _opportunityDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_Opportunity>.Error(111, "商机不存在");
            }
            info.DetailList = await _opportDetailDAL.SelectList(x => x.OpportId == id);
            var idslist = info.DetailList.Select(x => x.ProductId).ToList();
            if (idslist.Count > 0)
            {
                var prolist = await _provider.GetService<ProductDAL>().SelectList(x => idslist.Contains(x.Id));
                foreach (var item in info.DetailList)
                {
                    var tmppro = prolist.Where(x => x.Id == item.ProductId).FirstOrDefault();
                    if (tmppro != null)
                    {
                        item.ProductInfo = tmppro;
                    }
                }
            }

            if (!string.IsNullOrEmpty(info.Helper))
            {
                var tmpstrs = info.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var ids = Array.ConvertAll(tmpstrs, s => long.Parse(s));
                var users = await _userDAL.GetUserListByIds(ids.ToList());
                info.HelperUsers = users;
            }
            if (info.LeaderId != null && info.LeaderId > 0)
            {
                var leader = await _userDAL.GetAdminById(info.LeaderId.Value);
                info.LeaderUser = leader;
            }
            if (!string.IsNullOrEmpty(info.ContactId))
            {
                var contactDAL = _provider.GetService<ContactDAL>();
                info.ContactUser = await contactDAL.Select(info.ContactId);
            }
            if (!string.IsNullOrEmpty(info.CustomerId))
            {
                var customerDAL = _provider.GetService<CustomerDAL>();
                var customer = await customerDAL.Select(info.CustomerId);
                if (customer != null)
                {
                    info.CustomerName = customer.CustomerName;
                    info.CustomerNumber = customer.CustomerNumber;
                }

            }
            return BusResponse<MZ_Opportunity>.Success(info);
        }
        public async Task<string> GenerateNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("SJ");
        }
        public virtual async Task<PageObject<MZ_Opportunity>> SelectList(In_OpportunityList query)
        {
            var user = _provider.GetUser();
            var scope = await user.GetScope(_provider);

            var opportList = await _opportunityDAL.SelectByPage(query, scope, user);

            //初始化客户名称
            var customerdict = await _customerDAL.NavigateDict<MZ_Opportunity, string>(opportList.List, x => !string.IsNullOrEmpty(x.CustomerId), x => x.CustomerId);
            //初始化联系人
            var contactDAL = _provider.GetService<ContactDAL>();
            var contactdict = await contactDAL.NavigateDict<MZ_Opportunity, string>(opportList.List, x => !string.IsNullOrEmpty(x.ContactId), x => x.ContactId);
            //初始化阶段名称
            var perioddict = await _periodDAL.NavigateDict<MZ_Opportunity, string>(opportList.List, x => !string.IsNullOrEmpty(x.Period), x => x.Period);

            List<string> allusers = new List<string>();
            foreach (var opp in opportList.List)
            {
                if (!string.IsNullOrEmpty(opp.Helper))
                {
                    var tids = opp.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    allusers.AddRange(tids);
                }
                if (opp.LeaderId != null && opp.LeaderId > 0)
                {
                    allusers.Add(opp.LeaderId.ToString());
                }
            }
            Dictionary<string, MZ_AdminInfo> adminDict = new Dictionary<string, MZ_AdminInfo>();
            if (allusers.Count > 0)
            {
                var tmpids = allusers.ConvertAll(t => long.Parse(t));
                var users = await _userDAL.GetUserListByIds(tmpids);
                foreach (var u in users)
                {
                    adminDict.Add(u.Id.ToString(), u);
                }
            }
            foreach (var opp in opportList.List)
            {
                if (!string.IsNullOrEmpty(opp.Helper))
                {
                    List<MZ_AdminInfo> tmpusers = new List<MZ_AdminInfo>();
                    var tids = opp.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var u in tids)
                    {
                        MZ_AdminInfo tmpuu;
                        if (adminDict.TryGetValue(u, out tmpuu))
                        {
                            tmpusers.Add(tmpuu);
                        }
                    }

                    opp.HelperUsers = tmpusers;
                }
                if (opp.LeaderId != null && opp.LeaderId > 0)
                {
                    MZ_AdminInfo tmpuu;
                    if (adminDict.TryGetValue(opp.LeaderId.ToString(), out tmpuu))
                    {
                        opp.LeaderUser = tmpuu;
                    }
                }

                if (!string.IsNullOrEmpty(opp.ContactId))
                {
                    MZ_Contact tmpuu;
                    if (contactdict.TryGetValue(opp.ContactId, out tmpuu))
                    {
                        opp.ContactUser = tmpuu;
                    }
                }

                MZ_Customer tmpCustomer;
                if (customerdict.TryGetValue(opp.CustomerId, out tmpCustomer))
                {
                    opp.CustomerName = tmpCustomer.CustomerName;
                }

                MZ_Period tmpPeriod;
                if (perioddict.TryGetValue(opp.Period, out tmpPeriod))
                {
                    opp.PeriodName = tmpPeriod.PeriodName;
                }
            }

            return opportList;
        }

        public virtual async Task<BusResponse<string>> Delete(string id)
        {
            MZ_Opportunity old = await _opportunityDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "商机不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }
            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {

                    //删除商机明细
                    MZ_OpportDetail detail = new MZ_OpportDetail();
                    detail.del_flag = "2";
                    await _opportDetailDAL.Update(detail, x => x.OpportId == id);

                    //删除商机
                    MZ_Opportunity data = new MZ_Opportunity();
                    data.SetUpdateBy(user);
                    data.del_flag = "2";
                    if (await _opportunityDAL.Update(data, x => x.Id == id && x.del_flag == "0") <= 0)
                    {
                        BusResponse<string>.Error(121, "商机状态错误");
                    }

                    //删除关联商机跟进
                    MZ_Follow follow = new MZ_Follow();
                    follow.del_flag = "2";
                    follow.SetUpdateBy(user);
                    await _followDAL.Update(follow, x => x.TargetType == 0 && x.OpportId == id);


                    // 完成
                    await scope.CompleteAsync();
                }

                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<string>> Forward(string id, string target)
        {
            MZ_Opportunity old = await _opportunityDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "商机不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }
            var conf = await _provider.GetService<CrmConfBLL>().CrmInfo(user.OrgId);
            List<MZ_Period> periods = await _periodDAL.SelectList(x => x.OrgId == user.OrgId, "Sort asc");

            int curpexIdx = -1;
            int nextIdx = -1;
            for (int i = 0; i < periods.Count; i++)
            {
                if (curpexIdx != -1 && nextIdx == -1 && periods[i].PeriodType == "ing")
                {
                    nextIdx = i;
                    break;
                }
                if (periods[i].Id == old.Period)
                {
                    curpexIdx = i;
                }
            }
            var curper = periods.Where(x => x.Id == old.Period).First();
            var nextper = periods.Where(x => x.Id == target).First();

            if (conf.EnableKPer == false)
            {
                //验证是否跨阶段推进
                if (curper.PeriodType == "ing" && nextIdx > -1 && nextper.Sort > curper.Sort && nextper.Id != periods[nextIdx].Id)
                {
                    return BusResponse<string>.Error(125, "无法跨阶段操作，如需请在CRM设置里启用");
                }
            }
            if (conf.EnableIngBack == false)
            {
                if (curper.PeriodType == "ing" && nextper.Sort < curper.Sort)
                {
                    return BusResponse<string>.Error(126, "进行中阶段无法回退，如需请在CRM设置里启用");
                }
            }
            if (conf.EnableWinBack == false)
            {
                if (curper.PeriodType != "ing" && nextper.PeriodType == "ing")
                {
                    return BusResponse<string>.Error(126, "终点阶段无法回退，如需请在CRM设置里启用");
                }
            }

            MZ_Opportunity data = new MZ_Opportunity();
            data.SetUpdateBy(user);
            data.Id = id;
            data.Period = nextper.Id;
            data.PeriodType = nextper.PeriodType;
            if (nextper.PeriodType != "ing")
            {
                data.Probability = nextper.Probability;
            }
            else
            {
                if (nextper.Probability > old.Probability)
                {
                    data.Probability = nextper.Probability;
                }
            }
            await _opportunityDAL.Update(data);

            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<string>> Add(MZ_Opportunity data)
        {
            var period = await _periodDAL.Select(data.Period);
            if (period == null)
            {
                return BusResponse<string>.Error(122, "阶段不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(124, "请切换到企业账号");
            }

            data.Id = _snowflake.NextId().ToString();
            if (string.IsNullOrEmpty(data.OpportNumber))
            {
                data.OpportNumber = await GenerateNumber();
            }

            var leaderDept = await _orgDAL.SelectUserOrg(data.LeaderId.Value, user.OrgId);
            data.DeptId = leaderDept.dept_id;
            data.del_flag = "0";
            data.OrgId = user.OrgId;

            data.PeriodType = period.PeriodType;
            data.Helper ??= string.Empty;
            data.LastFollowDate = null;
            data.LastFollowId = string.Empty;
            data.Remark ??= string.Empty;
            data.LoseRemark ??= string.Empty;
            data.ReturnReason ??= string.Empty;
            data.SetCreateBy(user);
            data.StartFollowDate = null;

            if (data.DetailList != null)
            {
                for (int i = 0; i < data.DetailList.Count; i++)
                {
                    data.DetailList[i].Sort = i;
                    data.DetailList[i].OrgId = user.OrgId;
                    data.DetailList[i].del_flag = "0";
                    data.DetailList[i].OpportId = data.Id;
                }
            }



            //添加关联的主题
            MZ_Subject subj = new MZ_Subject();
            subj.SetCreateBy(user);
            subj.Id = _snowflake.NextId().ToString();
            subj.OrgId = data.OrgId;
            subj.Title = data.OpportName;
            subj.SubjectContent = string.Empty;
            subj.TargetId = data.Id;
            subj.TargetType = "商机";

            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    if (data.DetailList != null && data.DetailList.Count > 0)
                    {
                        await _opportDetailDAL.Insert(data.DetailList);
                        await _opportunityDAL.Insert(data);
                        await _subjectDAL.Insert(subj);
                    }
                    else
                    {
                        await _opportunityDAL.Insert(data);
                        await _subjectDAL.Insert(subj);
                    }
                    await scope.CompleteAsync();
                }

                return BusResponse<string>.Success(data.Id);
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<string>> Edit(MZ_Opportunity data)
        {
            MZ_Opportunity old = await _opportunityDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "商机不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }

            data.SetUpdateBy(user);
            data.OrgId = null;
            if (data.LeaderId != null)
            {
                var leaderDept = await _orgDAL.SelectUserOrg(data.LeaderId.Value, user.OrgId);
                data.DeptId = leaderDept.dept_id;
            }
            else
            {
                data.DeptId = null;
            }

            data.LastFollowId = null;
            data.LastFollowDate = null;
            data.StartFollowDate = null;
            data.ReturnReason = null;
            data.LoseRemark = null;
            data.del_flag = null;

            if (data.OpportName != null)
            {
                MZ_Subject subj = new MZ_Subject();
                subj.Title = data.OpportName;
                await _subjectDAL.Update(subj, x => x.TargetId == data.Id);
            }

            try
            {
                if (data.DetailList != null && data.DetailList.Count > 0)
                {
                    for (int i = 0; i < data.DetailList.Count; i++)
                    {
                        var dditem = data.DetailList[i];
                        dditem.OpportId = data.Id;
                        dditem.OrgId = old.OrgId;
                        dditem.Sort = i;
                        dditem.del_flag = "0";
                    }
                    using (BLLTranScope scope = new BLLTranScope())
                    {
                        await _opportDetailDAL.Delete(x => x.OpportId == data.Id);
                        await _opportDetailDAL.Insert(data.DetailList);

                        await _opportunityDAL.Update(data);
                        // 完成
                        await scope.CompleteAsync();
                    }
                }
                else
                {
                    await _opportunityDAL.Update(data);
                }

                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }

    }
}

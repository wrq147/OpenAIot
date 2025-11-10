using AuthService;
using Common.IdGenerator;
using Common.Share;
using CRMService.DAL;
using CRMService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using MyAccess.DB.Builder.WhereToSql;


namespace CRMService.Business
{
    public class CrmReportBLL
    {
        private ITAServiceProvider _provider;
        private OperatorHelper _operatorHelper;
        private SnowflakeHelper _snowflake;
        private CustomerDAL _customerDAL;
        private OpportunityDAL _opportDAL;
        private FollowDAL _followDAL;
        private OpportDetailDAL _opportDetailDAL;
        private PeriodDAL _periodDAL;
        public CrmReportBLL(ITAServiceProvider provider, OperatorHelper operatorHelper, SnowflakeHelper snowflake,
            CustomerDAL customerDAL, OpportunityDAL opportDAL, FollowDAL followDAL, OpportDetailDAL oppDetailDAL,
            PeriodDAL periodDAL)
        {
            _provider = provider;
            _operatorHelper = operatorHelper;
            _snowflake = snowflake;
            _customerDAL = customerDAL;
            _opportDAL = opportDAL;
            _followDAL = followDAL;
            _opportDetailDAL = oppDetailDAL;
            _periodDAL = periodDAL;
        }

        public virtual async Task<Out_CrmStatistics> StatisticsInfo(int day)
        {
            Out_CrmStatistics res = new Out_CrmStatistics();
            var user = _provider.GetUser();
            DateTime? fromTime = null;
            DateTime? toTime = null;
            if (day == 0)
            {
                fromTime = DateTime.Today;
            }
            else if (day == 1)
            {
                fromTime = DateTime.Today.AddDays(-1);
                toTime = DateTime.Today;
            }
            else
            {
                fromTime = DateTime.Today.AddDays(-day);
            }
            List<string> keys = new List<string>();
            keys.Add(user.UserId.ToString());
            string tmatchsql = _customerDAL.GetUsingDbHelp().CreateCompatible().FullSearch("Helper", keys);

            var kfscope = await user.GetScope(_provider, "/CRMService/Customer/List");
            var opscope = await user.GetScope(_provider, "/CRMService/Opportunity/List");
            string tkfwhere = kfscope.GenerateFilter("DeptId", "LeaderId", "LeaderId=" + user.UserId + " or " + tmatchsql, false, false, false);
            string topwhere = opscope.GenerateFilter("DeptId", "LeaderId", "LeaderId=" + user.UserId + " or " + tmatchsql, false, false, false);

            Expression<Func<MZ_Customer, bool>> customerExpression = x => x.del_flag == "0" && x.OrgId == user.OrgId && SonSqlFun.SqlCondition(tkfwhere);
            Expression<Func<MZ_Opportunity, bool>> opportExpression = x => x.del_flag == "0" && x.OrgId == user.OrgId && SonSqlFun.SqlCondition(topwhere);
            Expression<Func<MZ_Follow, bool>> followClueExpression = x => x.del_flag == "0" && x.OrgId == user.OrgId && x.createId == user.UserId && x.TargetType == 1;
            Expression<Func<MZ_Follow, bool>> followCustomerExpression = x => x.del_flag == "0" && x.OrgId == user.OrgId && x.createId == user.UserId && x.TargetType == 0;
            Expression<Func<MZ_Follow, bool>> followOpportExpression = x => x.del_flag == "0" && x.OrgId == user.OrgId && x.createId == user.UserId && x.OpportId != null;
            Expression<Func<MZ_Opportunity, bool>> winExpression = x => x.del_flag == "0" && x.OrgId == user.OrgId && x.LeaderId == user.UserId && x.PeriodType == "win";
            if (fromTime != null)
            {
                customerExpression = customerExpression.And(x => x.create_time >= fromTime);
                opportExpression = opportExpression.And(x => x.create_time >= fromTime);
                followClueExpression = followClueExpression.And(x => x.create_time >= fromTime);
                followCustomerExpression = followCustomerExpression.And(x => x.create_time >= fromTime);
                followOpportExpression = followOpportExpression.And(x => x.create_time >= fromTime);
                winExpression = winExpression.And(x => x.update_time >= fromTime);
            }
            if (toTime != null)
            {
                customerExpression = customerExpression.And(x => x.create_time < toTime);
                opportExpression = opportExpression.And(x => x.create_time < toTime);
                followClueExpression = followClueExpression.And(x => x.create_time < toTime);
                followCustomerExpression = followCustomerExpression.And(x => x.create_time < toTime);
                followOpportExpression = followOpportExpression.And(x => x.create_time < toTime);
                winExpression = winExpression.And(x => x.update_time < toTime);
            }
            res.NewKfCount = await _customerDAL.Count(customerExpression);
            res.NewOpportCount = await _opportDAL.Count(opportExpression);
            res.MaybeTotalPrice = await _opportDetailDAL.CalMaybeTotalPrice(fromTime, toTime, null, user);
            res.FollowClueCount = await _followDAL.Count(followClueExpression);
            res.FollowCustomerCount = await _followDAL.Count(followCustomerExpression);
            res.FollowOpportCount = await _followDAL.Count(followOpportExpression);
            res.NewFollowCount = res.FollowClueCount + res.FollowCustomerCount;
            res.WinOpportCount = await _opportDAL.Count(winExpression);

            return res;
        }
        public virtual async Task<BusResponse<Out_CrmCount>> StatisticsCount()
        {
            var user = _provider.GetUser();
            Out_CrmCount res = new Out_CrmCount();
            var kfscope = await user.GetScope(_provider, "/CRMService/Customer/List");
            var clscope = await user.GetScope(_provider, "/CRMService/Clue/PriList");
            var opscope = await user.GetScope(_provider, "/CRMService/Opportunity/List");

            var clueDAL = _provider.GetService<ClueDAL>();
            List<string> keys = new List<string>();
            keys.Add(user.UserId.ToString());
            string tmatchsql = clueDAL.GetUsingDbHelp().CreateCompatible().FullSearch("Helper", keys);
            string tkfwhere = kfscope.GenerateFilter("DeptId", "LeaderId", "LeaderId=" + user.UserId + " or " + tmatchsql, false, false, false);
            string tclwhere = clscope.GenerateFilter("DeptId", "LeaderId", "LeaderId=" + user.UserId + " or " + tmatchsql, false, false, false);
            string topwhere = opscope.GenerateFilter("DeptId", "LeaderId", "LeaderId=" + user.UserId + " or " + tmatchsql, false, false, false);

            res.KfCount = await _customerDAL.Count(x => x.del_flag == "0" && x.OrgId == user.OrgId && SonSqlFun.SqlCondition(tkfwhere));
            res.ClueCount = await clueDAL.Count(x => x.del_flag == "0" && x.OrgId == user.OrgId && SonSqlFun.SqlCondition(tclwhere));
            res.OpportCount = await _opportDAL.Count(x => x.del_flag == "0" && x.OrgId == user.OrgId && SonSqlFun.SqlCondition(topwhere));
            return BusResponse<Out_CrmCount>.Success(res);
        }
        public virtual async Task<List<Out_PeriodCount>> PeriodCountList(int day)
        {
            DateTime? fromTime = null;
            DateTime? toTime = null;
            if (day == 0)
            {
                fromTime = DateTime.Today;
            }
            else if (day == 1)
            {
                fromTime = DateTime.Today.AddDays(-1);
                toTime = DateTime.Today;
            }
            else
            {
                fromTime = DateTime.Today.AddDays(-day);
            }
            var user = _provider.GetUser();
            List<Out_PeriodCount> periodList = new List<Out_PeriodCount>();
            List<MZ_Period> periods = await _periodDAL.SelectList(x => x.OrgId == user.OrgId, "Sort asc");
            foreach (var period in periods)
            {
                Out_PeriodCount periodCC = new Out_PeriodCount();
                periodCC.Id = period.Id;
                periodCC.Name = period.PeriodName;
                Expression<Func<MZ_Opportunity, bool>> oppExpression = x => x.del_flag == "0" && x.createId == user.UserId && x.Period == period.Id;
                if (fromTime != null)
                {
                    oppExpression = oppExpression.And(x => x.create_time >= fromTime);
                }
                if (toTime != null)
                {
                    oppExpression = oppExpression.And(x => x.create_time < toTime);
                }
                periodCC.Count = await _opportDAL.Count(oppExpression);
                periodCC.TotalPrice = await _opportDetailDAL.CalMaybeTotalPrice(fromTime, toTime, period.Id, user);
                periodList.Add(periodCC);
            }
            return periodList;
        }

    }
}

using CRMService.Model;
using Common;
using System;
using AuthService;
using Common.Share;
using System.Threading.Tasks;
using MyAccess.DB;
using MyAccess.Aop;
using System.Collections.Generic;

namespace CRMService.DAL
{
    public class CustomerDAL : BaseRepository<MZ_Customer>
    {
        public virtual async Task<PageObject<MZ_Customer>> SelectByPage(In_CustomerList query, DataScope scope, Data_ServerTokenInfo user, int overday)
        {

            return await new SqlBuilder(help).Query<MZ_Customer>().Append("select * from mz_customer where del_flag='0' and OrgId=").AppendParam(user.OrgId)
                .Then(query.Belong == 1, sq => sq.Append(" and LeaderId=0")).Then(query.Belong == 2, sq => sq.Append(" and LeaderId>0"))
                .Then(query.IsInvite == true, sq => sq.Append(" and BindOrgId>0"))
                .Then(query.IsInvite == false, sq => sq.Append(" and BindOrgId=0"))
                .Then(overday > 0, sq =>
                {
                    DateTime dayover = DateTime.Now.AddDays(-overday - 7);
                    sq.Append(" and LastFollowDate>").AppendParam(dayover);
                })
                .Then(query.beginTime != null, sq => sq.Append(" and create_time >= ").AppendParam(query.beginTime))
                .Then(query.endTime != null, sq => sq.Append(" and create_time <= ").AppendParam(query.endTime))
                .Then(scope != null, sql => {
                    List<string> keys = new List<string>();
                    keys.Add(user.UserId.ToString());
                    string tsqlmatch = sql.Sql.Comparable.FullSearch("Helper", keys);
                    sql.Append(scope.GenerateFilter("DeptId", "LeaderId", "LeaderId=" + user.UserId + " or " + tsqlmatch));
                })
            .GeneratePageObjectAsync(query, string.Empty);
        }
        public virtual async Task AutoToPublic(long orgId, int day)
        {
            await new SqlBuilder(help).Append("UPDATE mz_customer set LeaderId=0,DeptId=0,StartFollowDate=NULL,ReturnReason='超时未跟进',updateId=2,update_time=NOW() where OrgId=")
                .AppendParam(orgId).Append(" and LastFollowDate<").AppendParam(DateTime.Now.AddDays(-day)).DoAsync<DoExecSql>();
        }

        public async Task<Out_CustomerInfo> SelectCustomerByOrgId(long fromOrgId, long toOrgId)
        {
            using DbHelp db = CreateDB();
            var dqs = await new SqlBuilder(db).Append("select Id,OrgId,CustomerNumber,CustomerName,CustomerType,BindOrgId from mz_customer where OrgId=").AppendParam(fromOrgId).Append(" and BindOrgId=").AppendParam(toOrgId).DoAsync<DoQuerySql<Out_CustomerInfo>>();
            return dqs.ToFirst();
        }
    }
}

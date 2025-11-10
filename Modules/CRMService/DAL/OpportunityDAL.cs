using AuthService;
using Common;
using Common.Share;
using CRMService.Model;
using MyAccess.DB;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace CRMService.DAL
{
    public class OpportunityDAL : BaseRepository<MZ_Opportunity>
    {
        public virtual async Task<PageObject<MZ_Opportunity>> SelectByPage(In_OpportunityList query, DataScope scope, Data_ServerTokenInfo user)
        {
            return await new SqlBuilder(help).Query<MZ_Opportunity>().Append("select * from mz_opportunity where del_flag='0' and OrgId=").AppendParam(user.OrgId)
                .Then(query.beginTime != null, sq => sq.Append(" and create_time >= ").AppendParam(query.beginTime))
                .Then(query.endTime != null, sq => sq.Append(" and create_time <= ").AppendParam(query.endTime))
                .Then(scope != null, sql => {
                    List<string> keys = new List<string>();
                    keys.Add(user.UserId.ToString());
                    string tsqlmatch = sql.Sql.Comparable.FullSearch("Helper", keys);
                    sql.Append(scope.GenerateFilter("DeptId", "LeaderId", "LeaderId=" + user.UserId + " or " + tsqlmatch));
                })
            .GeneratePageObjectAsync(query, "create_time desc");
        }
    }
}

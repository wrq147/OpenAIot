using AuthService;
using Common;
using Common.Share;
using CRMService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.DAL
{
    public class ClueDAL : BaseRepository<MZ_Clue>
    {
        public virtual async Task<PageObject<MZ_Clue>> SelectByPage(In_ClueList query, DataScope scope, Data_ServerTokenInfo user)
        {

            return await new SqlBuilder(help).Query<MZ_Clue>().Append("select * from mz_clue where del_flag='0' and OrgId=").AppendParam(user.OrgId)
                .Then(query.Belong == 1, sq => sq.Append(" and LeaderId=0")).Then(query.Belong == 2, sq => sq.Append(" and LeaderId>0"))
                .Then(query.beginTime != null, sq => sq.Append(" and create_time >= ").AppendParam(query.beginTime))
                .Then(query.endTime != null, sq => sq.Append(" and create_time <= ").AppendParam(query.endTime))
                .Then(scope != null, sql =>
                {
                    List<string> keys = new List<string>();
                    keys.Add(user.UserId.ToString());
                    string tsqlmatch = sql.Sql.Comparable.FullSearch("Helper", keys);
                    sql.Append(scope.GenerateFilter("DeptId", "LeaderId", "LeaderId=" + user.UserId + " or " + tsqlmatch));
                })
            .GeneratePageObjectAsync(query, string.Empty);
        }

        public virtual async Task AutoToPublic(long orgId, int day)
        {
            await new SqlBuilder(help).Append("UPDATE mz_clue set LeaderId=0,DeptId=0,StartFollowDate=NULL,ReturnReason='超时未跟进',updateId=2,update_time=NOW() where OrgId=")
                .AppendParam(orgId).Append(" and LastFollowDate<").AppendParam(DateTime.Now.AddDays(-day)).DoAsync<DoExecSql>();
        }
    }
}

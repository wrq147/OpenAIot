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
    public class ContactDAL : BaseRepository<MZ_Contact>
    {
        public virtual async Task<PageObject<MZ_Contact>> SelectByPage(In_ContactList query, DataScope scope, Data_ServerTokenInfo user)
        {
            return await new SqlBuilder(help).Query<MZ_Contact>().Append("select * from mz_contact where del_flag='0' and OrgId=").AppendParam(user.OrgId)
                .Then(!string.IsNullOrEmpty(query.CustomerId), sq => sq.Append(" and CustomerId=").AppendParam(query.CustomerId))
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

        public virtual async Task ClearLeader()
        {
            await new SqlBuilder(help).Append("update mz_contact set LeaderId=0,DeptId=0,updateId=2,update_time=NOW() where EXISTS(select Id from mz_customer where Id=mz_contact.CustomerId and LeaderId=0)").DoAsync<DoExecSql>();
        }
    }
}

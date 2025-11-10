using AuthService.Model;
using Common;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.DAL
{
    public class GroupViewDAL : BaseRepository<MZ_GroupView>
    {
        public virtual async Task<int> UpdateSort(List<string> idList)
        {
            string casestr = " case";
            for (int i = 0; i < idList.Count; i++)
            {
                casestr += " when Id =" + help.AddParam(idList[i]) + " then " + i;
            }
            casestr += " end";
            return (await new SqlBuilder(help).Append("UPDATE mz_group_view SET Sort = " + casestr + " where Id in (").AppendParam(idList).Append(")").DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task SortIncrease(long orgId, int sort)
        {
            var doquery = await new SqlBuilder(help).Append("select Id from mz_group_view where OrgId=").AppendParam(orgId).Append(" and Sort=").AppendParam(sort).DoAsync<DoQuerySql<long>>();
            if (doquery.Count > 0)
            {
                await new SqlBuilder(help).Append("update mz_group_view set Sort=Sort+1 where OrgId=").AppendParam(orgId).Append(" and Sort>=").AppendParam(sort).DoAsync<DoExecSql>();
            }
        }
    }
}

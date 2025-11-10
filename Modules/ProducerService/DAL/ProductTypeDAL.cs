using Common;
using MyAccess.DB;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProducerService.DAL
{
    public class ProductTypeDAL : BaseRepository<MZ_ProductType>
    {
        public virtual async Task<int> UpdateSort(List<string> idList)
        {
            string casestr = " case";
            for (int i = 0; i < idList.Count; i++)
            {
                casestr += " when Id =" + help.AddParam(idList[i]) + " then " + i;
            }
            casestr += " end";
            return (await new SqlBuilder(help).Append("UPDATE mz_product_type SET Sort = " + casestr + " where Id in (").AppendParam(idList).Append(")").DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task SortIncrease(long orgId, int sort)
        {
            var doquery = await new SqlBuilder(help).Append("select Id from mz_product_type where OrgId=").AppendParam(orgId).Append(" and Sort=").AppendParam(sort).DoAsync<DoQuerySql<long>>();
            if (doquery.Count > 0)
            {
                await new SqlBuilder(help).Append("update mz_product_type set Sort=Sort+1 where OrgId=").AppendParam(orgId).Append(" and Sort>=").AppendParam(sort).DoAsync<DoExecSql>();
            }
        }
    }
}

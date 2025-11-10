using Common;
using IoTService.Models;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTService.DAL
{
    public class IotClassDAL : BaseRepository<MZ_IotClass>
    {
        public virtual async Task SortIncrease(long orgId, int sort)
        {
            var doquery = await new SqlBuilder(help).Append("select Id from mz_iot_class where OrgId=").AppendParam(orgId).Append(" and Sort=").AppendParam(sort).DoAsync<DoQuerySql<long>>();
            if (doquery.Count > 0)
            {
                await new SqlBuilder(help).Append("update mz_iot_class set Sort=Sort+1 where OrgId=").AppendParam(orgId).Append(" and Sort>=").AppendParam(sort).DoAsync<DoExecSql>();
            }
        }
        public virtual async Task<int> UpdateSort(List<string> idList)
        {
            string casestr = " case";
            for (int i = 0; i < idList.Count; i++)
            {
                casestr += " when Id =" + help.AddParam(idList[i]) + " then " + i;
            }
            casestr += " end";
            return (await new SqlBuilder(help).Append("UPDATE mz_iot_class SET Sort = " + casestr + " where Id in (").AppendParam(idList).Append(")").DoAsync<DoExecSql>()).RowCount;
        }

        public virtual async Task<int> UpdateChildrenPath(List<MZ_IotClass> list)
        {
            SqlBuilder sql = new SqlBuilder(help);
            string whenlist = string.Empty;
            string whenin = string.Empty;
            foreach (MZ_IotClass cls in list)
            {
                string tmpid = help.AddParam(cls.Id);
                whenlist = whenlist + " when " + tmpid + " then '" + cls.Path + "'";
                whenin += "," + tmpid;
            }
            whenin = whenin.Substring(1);
            sql.Append("update mz_iot_class set Path=case Id ").Append(whenlist).Append(" end").Append(" where Id in (" + whenin + ")");
            return (await sql.DoAsync<DoExecSql>()).RowCount;
        }
    }
}

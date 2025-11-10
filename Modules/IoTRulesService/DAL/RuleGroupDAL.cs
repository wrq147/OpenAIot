using Common;
using IoTRulesService.Model;
using IoTService.Models;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService.DAL
{
    public class RuleGroupDAL : BaseRepository<MZ_RuleGroup>
    {
        public virtual async Task<int> UpdateSort(List<string> idList)
        {
            string casestr = " case";
            for (int i = 0; i < idList.Count; i++)
            {
                casestr += " when Id =" + help.AddParam(idList[i]) + " then " + i;
            }
            casestr += " end";
            return (await new SqlBuilder(help).Append("UPDATE mz_rule_group SET Sort = " + casestr + " where Id in (").AppendParam(idList).Append(")").DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> UpdateChildrenPath(List<MZ_RuleGroup> list)
        {
            SqlBuilder sql = new SqlBuilder(help);
            string whenlist = string.Empty;
            string whenin = string.Empty;
            foreach (MZ_RuleGroup cls in list)
            {
                string tmpid = help.AddParam(cls.Id);
                whenlist = whenlist + " when " + tmpid + " then '" + cls.Path + "'";
                whenin += "," + tmpid;
            }
            whenin = whenin.Substring(1);
            sql.Append("update mz_rule_group set Path=case Id ").Append(whenlist).Append(" end").Append(" where Id in (" + whenin + ")");
            return (await sql.DoAsync<DoExecSql>()).RowCount;
        }
    }
}

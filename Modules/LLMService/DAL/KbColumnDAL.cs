using Common;
using Common.Share;
using LLMService.Controller;
using LLMService.Model;
using MyAccess.Aop;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LLMService.DAL
{
    public class KbColumnDAL : BaseRepository<MZ_KbColumn>
    {
        public virtual async Task<List<MZ_KbColumn>> SelectKbColumnList(string kbId)
        {
            Expression<Func<MZ_KbColumn, bool>> expression = (c) => c.KbId == kbId;

            var tmpSql = new SqlBuilder(help).Query<MZ_KbColumn>()
                .Include(x => x.Kb, x => x.KbId)
                .Where(expression).Append(" order by a.SortOrder asc,a.create_time desc");
            var listResult = await tmpSql.ToListAsync();
            return listResult;
        }

        public virtual async Task SortIncrease(string kbId, int sort)
        {
            var doquery = await new SqlBuilder(help).Append("select Id from llm_kb_column where KbId=").AppendParam(kbId).Append(" and SortOrder=").AppendParam(sort).DoAsync<DoQuerySql<long>>();
            if (doquery.Count > 0)
            {
                await new SqlBuilder(help).Append("update llm_kb_column set SortOrder=SortOrder+1 where KbId=").AppendParam(kbId).Append(" and SortOrder>=").AppendParam(sort).DoAsync<DoExecSql>();
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
            return (await new SqlBuilder(help).Append("UPDATE llm_kb_column SET SortOrder = " + casestr + " where Id in (").AppendParam(idList).Append(")").DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> UpdateChildrenPath(List<MZ_KbColumn> list)
        {
            SqlBuilder sql = new SqlBuilder(help);
            string whenlist = string.Empty;
            string whenin = string.Empty;
            foreach (MZ_KbColumn cls in list)
            {
                string tmpid = help.AddParam(cls.Id);
                whenlist = whenlist + " when " + tmpid + " then '" + cls.Path + "'";
                whenin += "," + tmpid;
            }
            whenin = whenin.Substring(1);
            sql.Append("update llm_kb_column set Path=case Id ").Append(whenlist).Append(" end").Append(" where Id in (" + whenin + ")");
            return (await sql.DoAsync<DoExecSql>()).RowCount;
        }
    }

}
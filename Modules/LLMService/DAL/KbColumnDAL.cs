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
        /// <summary>
        /// 获取栏目的最大排序号
        /// </summary>
        public virtual async Task<int> GetMaxSortOrder(string kbId)
        {
            return (await new SqlBuilder(help).Append("SELECT MAX(DeviceUpIdx) FROM llm_kb_column where KbId=").AppendParam(kbId)
                .DoAsync<DoQueryScalar>()).GetValueInt();
        }
    }

}
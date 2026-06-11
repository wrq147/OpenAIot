using Common;
using Common.Share;
using LLMService.Controller;
using LLMService.Model;
using MyAccess.Aop;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using SqlParser.Net.Ast;
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

    }

}
using Common;
using Common.Share;
using MESService.Model;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using NPOI.SS.Formula.Eval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MESService.DAL
{
    public class WorkBatchDAL : BaseRepository<MZ_WorkBatch>
    {
        public virtual async Task<PageObject<MZ_WorkBatch>> SelectByPage(In_WorkBatchList query, long orgId)
        {
            Expression<Func<MZ_WorkBatch, bool>> expression = (a) => a.OrgId == orgId;
            if (!string.IsNullOrEmpty(query.Key))
            {
                expression = expression.And((a) => a.Id.Contains(query.Key) || a.LNumber.Contains(query.Key));
            }
            var tmpSql = new SqlBuilder(help).Query<MZ_WorkBatch>().Where(expression);
            if (query.Items != null && query.Items.Length > 0)
            {
                //过滤扩展字段
                foreach (var item in query.Items)
                {
                    item.AppendFilter(tmpSql, string.Empty);
                }
            }

            return await tmpSql.GeneratePageObjectAsync(query);
        }

    }
}

using Common;
using Common.Share;
using MyAccess.Core;
using MyAccess.DB.Builder.WhereToSql;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ReportService.DAL
{
    public class ApiSourceDAL : BaseRepository<MZ_ApiSource>
    {
        public virtual async Task<PageObject<MZ_ApiSource>> SelectByPage(In_ApiSourceListPage query,long orgId)
        {
            RefAsync<int> total = 0;
            Expression<Func<MZ_ApiSource, bool>> expression = x => x.OrgId == 0 || x.OrgId == orgId;
            if (!string.IsNullOrEmpty(query.Name))
            {
                expression = expression.And(x => x.InterfaceName.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.ApiType))
            {
                expression = expression.And(x => x.ApiType == query.ApiType);
            }

            List<MZ_ApiSource> tlist;
            if (query.showAll)
            {
                tlist = await SelectList(expression, query.GetOrderBy());
            }
            else
            {
                tlist = await SelectPage(expression, query.pageNum, query.pageSize, total, query.GetOrderBy());
            }
            return new PageObject<MZ_ApiSource>()
            {
                List = tlist,
                Total = total
            };
        }
    }
}

using Common;
using Common.Share;
using MyAccess.Core;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ReportService.DAL
{
    public class DataSourceDAL : BaseRepository<MZ_DataSource>
    {
        public virtual async Task<PageObject<MZ_DataSource>> SelectByPage(In_DataSourceListPage query, long orgId)
        {
            RefAsync<int> total = 0;
            Expression<Func<MZ_DataSource, bool>> expression = x => x.OrgId == orgId;
            if (!string.IsNullOrEmpty(query.Name))
            {
                expression = expression.And(x => x.LinkName.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.DataType))
            {
                expression = expression.And(x => x.DatabaseType == query.DataType);
            }
            List<MZ_DataSource> tlist;
            if (query.showAll)
            {
                tlist = await SelectList(expression, query.GetOrderBy());
            }
            else
            {
                tlist = await SelectPage(expression, query.pageNum, query.pageSize, total, query.GetOrderBy());
            }
            return new PageObject<MZ_DataSource>()
            {
                List = tlist,
                Total = total
            };
        }
    }
}

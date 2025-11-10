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
    public class FileSourceDAL : BaseRepository<MZ_FileSource>
    {
        public virtual async Task<PageObject<MZ_FileSource>> SelectByPage(In_FileSourceListPage query)
        {
            RefAsync<int> total = 0;
            Expression<Func<MZ_FileSource, bool>> expression = x => x.OrgId == query.OrgId;
            if (!string.IsNullOrEmpty(query.Name))
            {
                expression = expression.And(x => x.FileName.Contains(query.Name));
            }

            List<MZ_FileSource> tlist;
            if (query.showAll)
            {
                tlist = await SelectList(expression, query.GetOrderBy());
            }
            else
            {
                tlist = await SelectPage(expression, query.pageNum, query.pageSize, total, query.GetOrderBy());
            }
            return new PageObject<MZ_FileSource>()
            {
                List = tlist,
                Total = total
            };
        }
    }
}

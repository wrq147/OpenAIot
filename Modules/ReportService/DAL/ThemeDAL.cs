using Common;
using Common.Share;
using MyAccess.Core;
using MyAccess.DB.Builder.WhereToSql;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.DAL
{
    public class ThemeDAL : BaseRepository<MZ_ReportTheme>
    {
        public virtual async Task<PageObject<MZ_ReportTheme>> SelectByPage(In_ThemeListPage query)
        {
            RefAsync<int> total = 0;
            List<MZ_ReportTheme> tlist;
            Expression<Func<MZ_ReportTheme, bool>> expression = x => true;
            if (!string.IsNullOrEmpty(query.Name))
            {
                expression = expression.And(x => x.ThemeName.Contains(query.Name));
            }
            if (query.showAll)
            {
                tlist = await SelectList(expression, query.GetOrderBy());
            }
            else
            {
                tlist = await SelectPage(expression, query.pageNum, query.pageSize, total, query.GetOrderBy());
            }
            return new PageObject<MZ_ReportTheme>()
            {
                List = tlist,
                Total = total
            };
        }
    }
}

using Common.Share;
using MyAccess.Core;
using MyAccess.DB.Builder.WhereToSql;
using ReportService.DAL;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Business
{
    public class ThemeBLL
    {
        private ThemeDAL _themeDAL;
        public ThemeBLL(ThemeDAL themeDAL)
        {
            _themeDAL = themeDAL;
        }
        public virtual async Task<PageObject<MZ_ReportTheme>> ListPage(In_ThemeListPage query)
        {
            return await _themeDAL.SelectByPage(query);
        }
    }
}

using AuthService.Controller;
using Common;
using Common.Share;
using ReportService.Business;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Route;

namespace ReportService.Controller
{
    /// <summary>
    /// 报表主题接口
    /// </summary>
    public class Theme : AbstractLoginedController
    {
        private ThemeBLL _themeBLL;
        public Theme(ThemeBLL themeBLL)
        {
            _themeBLL = themeBLL;
        }

        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_ReportTheme>>> List(In_ThemeListPage query)
        {
            return this.Success(await _themeBLL.ListPage(query));
        }
    }
}

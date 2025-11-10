using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Models
{
    public class In_ThemeListPage : BaseQueryParam
    {
        /// <summary>
        /// 过滤主题名称
        /// </summary>
        public string Name { get; set; }
    }
}

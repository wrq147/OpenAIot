using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Models
{
    public class In_ReportWarnPage : BaseQueryParam
    {
        /// <summary>
        /// 报表Id
        /// </summary>
        public string ReportId { get; set; }
    }
}

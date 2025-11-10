using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Models
{
    public class In_DataSourceListPage : BaseQueryParam
    {
        /// <summary>
        /// 过滤链接名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据库类型：mysql、sqlserver
        /// </summary>
        public string DataType { get; set; }
    }
}

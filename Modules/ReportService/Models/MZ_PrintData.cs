using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Models
{
    [TableName("mz_print_data")]
    public class MZ_PrintData
    {
        /// <summary>
        /// ID
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 数据源名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据源标识
        /// </summary>
        public string DataMap { get; set; }
        /// <summary>
        /// 接口数据源Id
        /// </summary>
        public string ApiId { get; set; }
    }
}

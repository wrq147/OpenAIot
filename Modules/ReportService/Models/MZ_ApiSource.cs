using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Models
{
    /// <summary>
    /// Api数据源
    /// </summary>
    [TableName("mz_api_source")]
    public class MZ_ApiSource : BaseEntity
    {
        /// <summary>
        /// 编码Id
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID（0代表所有用户可见）
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 接口类型：0为BI报表、1为打印模板
        /// </summary>
        public string ApiType { get; set; }
        /// <summary>
        /// 接口名称
        /// </summary>
        public string InterfaceName { get; set; }
        /// <summary>
        /// 接口地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 接口Method
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 请求的Param
        /// </summary>
        public string ParamJson { get; set; }
        /// <summary>
        /// 参数类型:PARAM、JSON、FORM
        /// </summary>
        public string ParamType { get; set; }
        /// <summary>
        /// 请求的Header
        /// </summary>
        public string HeaderJson { get; set; }
    }
}

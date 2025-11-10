using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    /// <summary>
    /// 企业扩展设置
    /// </summary>
    [TableName("mz_org_ext")]
    public class MZ_OrgExt
    {
        [ID(false)]
        public long? OrgId { get; set; }
        /// <summary>
        /// 扩展字段
        /// </summary>
        public string ExtField { get; set; }
        /// <summary>
        /// 扩展值
        /// </summary>
        public string ExtValue { get; set; }
    }
}

using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    [TableName("mz_admin_ext")]
    public class MZ_AdminExt
    {
        [ID(false)]
        public long? UserId { get; set; }
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

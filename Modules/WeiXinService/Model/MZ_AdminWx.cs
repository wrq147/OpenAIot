using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiXinService.Model
{
    /// <summary>
    /// 账号与微信的关联表
    /// </summary>
    [TableName("mz_admin_wx")]
    public class MZ_AdminWx
    {
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 微信的AppId
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 绑定的微信unionId
        /// </summary>
        public string UnionId { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedOn { get; set; }
    }
}

using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    /// <summary>
    /// 组织的应用主题
    /// </summary>
    [TableName("mz_org_style")]
    public class MZ_OrgStyle
    {
        /// <summary>
        /// 组织编号
        /// </summary>
        [ID(false)]
        public long? OrgId { get; set; }
        /// <summary>
        /// 主题Id
        /// </summary>
        [ID(false)]
        public string StyleId { get; set; }
        /// <summary>
        /// 来源方式：0自购，1为继承，2为管理员分配
        /// </summary>
        public int? FrowWay { get; set; }
        /// <summary>
        /// 是否使用中
        /// </summary>
        public bool? IsUsing { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}

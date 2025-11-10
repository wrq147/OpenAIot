using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    /// <summary>
    /// 应用主题
    /// </summary>
    [TableName("mz_app_style")]
    public class MZ_AppStyle : BaseEntity
    {
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 主题名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 主题展示图
        /// </summary>
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 主题Json
        /// </summary>
        public string StyleJson { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否公开
        /// </summary>
        public bool IsPublic { get; set; }
    }
}

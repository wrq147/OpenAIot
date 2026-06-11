using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscussService.Model
{
    /// <summary>
    /// 主题
    /// </summary>
    [TableName("mz_subject")]
    public class MZ_Subject : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 关联对象Id
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 主题类型
        /// </summary>
        public string TargetType { get; set; }
        /// <summary>
        /// 主题内容
        /// </summary>
        public string SubjectContent { get; set; }
    }
}

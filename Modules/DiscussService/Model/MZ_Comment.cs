using AuthService;
using Common.Attr;
using FluentMigrator.Infrastructure;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscussService.Model
{
    /// <summary>
    /// 评论表
    /// </summary>
    [TableName("mz_comment")]
    public class MZ_Comment
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
        /// 评论人
        /// </summary>
        public long? UserId { get; set; }
        /// <summary>
        /// 主题Id
        /// </summary>
        public string SubjectId { get; set; }
        /// <summary>
        /// 父评论Id
        /// </summary>
        public string ParentCommentId { get; set; }
        /// <summary>
        /// 父评论用户Id
        /// </summary>
        public long? ParentCommentUserId { get; set; }
        /// <summary>
        /// 回复内容（必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入回复内容")]
        public string Content { get; set; }
        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int? PraiseNum { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateOn { get; set; }
        /// <summary>
        /// 关联的主题
        /// </summary>
        public MZ_Subject Subject { get; set; }
        /// <summary>
        /// 评论人信息
        /// </summary>
        public MZ_AdminInfo UserInfo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    /// <summary>
    /// 评论事件
    /// </summary>
    public class DiscussEvent
    {
        /// <summary>
        /// 评论人
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 评论人姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 目标Id
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 评论类型
        /// </summary>
        public string TargetType { get; set; }
        /// <summary>
        /// 评论Id
        /// </summary>
        public string CommentId { get; set; }
    }
}

using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscussService.Model
{
    public class In_CommentList : BaseQueryParam
    {
        /// <summary>
        /// 过滤评论类型
        /// </summary>
        public string TargetType { get; set; }
        /// <summary>
        /// 过滤目标Id
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 过滤我的评论
        /// </summary>
        public bool? IsMy { get; set; }
        /// <summary>
        /// 过滤回复我的
        /// </summary>
        public bool? IsMyReply { get; set; }
    }
}

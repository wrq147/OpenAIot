using Common.Share;
using System;

namespace MessageService.Model
{
    public class In_MessageList : BaseQueryParam
    {
        /// <summary>
        /// 0 未读，1 已读，-1全部
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 过滤类型：click_type
        /// </summary>
        public int? t { get; set; }
    }
}

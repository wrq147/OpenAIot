using Common.Share;
using System;

namespace CardService.Model
{
    /// <summary>
    /// 名片列表请求参数
    /// </summary>
    public class In_Card_List : BaseQueryParam
    {
        /// <summary>
        /// 所属用户过滤
        /// </summary>
        public long? UserId { get; set; }
        /// <summary>
        /// 所属组织过滤
        /// </summary>
        public long? OrgId { get; set; }
    }
}

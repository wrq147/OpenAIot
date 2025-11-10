using Common.Share;
using System;

namespace CardService.Model
{
    /// <summary>
    /// 浏览记录列表请求参数
    /// </summary>
    public class In_Record_List : BaseQueryParam
    {
        /// <summary>
        /// 访问者名片过滤
        /// </summary>
        public long? VisitCardId { get; set; }
    }
}

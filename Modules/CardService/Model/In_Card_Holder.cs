using Common.Share;
using System;

namespace CardService.Model
{
    /// <summary>
    /// 通讯录列表过滤参数
    /// </summary>
    public class In_Card_Holder : BaseQueryParam
    {
        /// <summary>
        /// 通讯录的搜索关键字
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 过滤用户
        /// </summary>
        public long? UserId { get; set; }
    }
}

using Common.Share;
using System;
using System.ComponentModel.DataAnnotations;

namespace CardService.Model
{
    /// <summary>
    /// 案例查询
    /// </summary>
    public class In_Card_Case : BaseQueryParam
    {
        /// <summary>
        /// 过滤名片
        /// </summary>
        [Required(ErrorMessage = "名片Id为必填项")]
        public long? CardId { get; set; }
        /// <summary>
        /// 过滤组织
        /// </summary>
        public long? OrgId { get; set; }
    }
}

using Common.Share;
using System;
using System.ComponentModel.DataAnnotations;

namespace CardService.Model
{
    public class In_Card_Case_M : BaseQueryParam
    {
        /// <summary>
        /// 过滤组织
        /// </summary>
        [Required(ErrorMessage = "组织Id为必填项")]
        public long? OrgId { get; set; }
        /// <summary>
        /// 过滤案例关键字
        /// </summary>
        public string key { get; set; }
    }
}

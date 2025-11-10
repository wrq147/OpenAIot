using Common.Share;
using System;

namespace IoTService.Models
{
    public class In_UpdatePage : BaseQueryParam
    {
        /// <summary>
        /// 过滤组织（前端不用传）
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 过滤产品Id
        /// </summary>
        public string ProductId { get; set; }
    }
}

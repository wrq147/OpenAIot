using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayService.Model
{
    /// <summary>
    /// 支付记录列表查询入参
    /// </summary>
    public class In_PayDetailList : BaseQueryParam
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 所属企业ID
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 支付状态（0未支付，1支付成功，2支付失败，3退款中，4退款失败，5退款成功）
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderId { get; set; }
    }

}

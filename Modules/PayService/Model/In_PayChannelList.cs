using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayService.Model
{
    /// <summary>
    /// 支付渠道列表查询入参
    /// </summary>
    public class In_PayChannelList : BaseQueryParam
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
        /// 所属企业ID（0表示系统渠道）
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 渠道标识符（支付宝、微信）
        /// </summary>
        public string ChannelLabel { get; set; }
    }
}

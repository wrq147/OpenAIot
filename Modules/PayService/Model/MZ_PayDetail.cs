using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayService.Model
{
    /// <summary>
    /// 统一交易记录表
    /// </summary>
    [TableName("mz_pay_detail")]
    public class MZ_PayDetail
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID]
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long OrgId { get; set; }

        /// <summary>
        /// 支付人
        /// </summary>
        public long PayUser { get; set; }

        /// <summary>
        /// 支付渠道Id
        /// </summary>
        public string PayChannelId { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public int OrderType { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 0未支付，1支付成功，2支付失败，3退款中，4退款失败，5退款成功
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 交易手续费
        /// </summary>
        public decimal Fee { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }

}

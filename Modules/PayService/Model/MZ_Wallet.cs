using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayService.Model
{
    /// <summary>
    /// 钱包表
    /// </summary>
    [TableName("mz_wallet")]
    public class MZ_Wallet
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [ID]
        public long Id { get; set; }

        /// <summary>
        /// 钱包当前余额
        /// </summary>
        public decimal TotalBalance { get; set; }

        /// <summary>
        /// 钱包货币类型，ISO 4217标准代码
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }
}

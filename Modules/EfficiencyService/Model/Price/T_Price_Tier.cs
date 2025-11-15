using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyAccess.DB.Attr;
using Common.Share;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;

namespace EfficiencyService.Model
{
    [TableName("t_price_tier")]
    public class T_Price_Tier : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }

        /// <summary>
        /// 政策明细编码
        /// </summary>
        public string DetilId { get; set; }

        /// <summary>
        /// 阶梯级别（1代表第一档 2代表第二档）
        /// </summary>
        public string TierLevel { get; set; }

        /// <summary>
        /// 最小用电量
        /// </summary>
        public double MinKwh { get; set; }

        /// <summary>
        /// 最大用电量
        /// </summary>
        public double MaxKwh { get; set; }

        /// <summary>
        /// 电价
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
    }
}

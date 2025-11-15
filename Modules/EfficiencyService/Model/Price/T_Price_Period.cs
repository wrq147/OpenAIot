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
    [TableName("t_price_period")]
    public class T_Price_Period : BaseEntity
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
        /// 时段类型（1代表尖 2代表峰 3代表平 4代表谷 5代表全天）
        /// </summary>
        public string TimePeriod { get; set; }

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

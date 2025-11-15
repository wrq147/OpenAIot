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
    [TableName("t_prod_config")]
    public class T_Prod_Config
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }

        /// <summary>
        /// 上次开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 上次结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 累计阶梯值
        /// </summary>
        public double TotalUse { get; set; }

        /// <summary>
        /// 累计开始时间
        /// </summary>
        public DateTime TotalTime { get; set; }

    }
}

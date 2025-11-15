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
    [TableName("t_price_policydetil")]
    public class T_Price_PolicyDetil : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }

        /// <summary>
        /// 政策编码
        /// </summary>
        public string PolicyId { get; set; }

        /// <summary>
        /// 生效月份
        /// </summary>
        public string PolicyMonth { get; set; }

        /// <summary>
        /// 计费方式（1代表分时 2代表不分时 3代表阶梯）
        /// </summary>
        public string PolicyType { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 计费周期（1月 2季度 3年）
        /// </summary>
        public string CycleType { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
    }
}

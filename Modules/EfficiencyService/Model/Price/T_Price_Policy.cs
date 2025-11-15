using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyAccess.DB.Attr;
using Common.Share;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EfficiencyService.Model
{
    [TableName("t_price_policy")]
    public class T_Price_Policy : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 政策名称
        /// </summary>
        public string PolicyName { get; set; }

        /// <summary>
        /// 排放因子编码
        /// </summary>
        public string FactorId { get; set; }

        /// <summary>
        /// 能源类型
        /// </summary>
        public string EnergyType { get; set; }

        /// <summary>
        /// 一级计量单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 二级计量单位
        /// </summary>
        public string LageUnit { get; set; }

        /// <summary>
        /// 来源说明
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
    }
}

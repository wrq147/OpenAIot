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
    [TableName("t_prod_production")]
    public class T_Prod_Production : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long OrgId { get; set; }

        /// <summary>
        /// 设施编码
        /// </summary>
        public string FacilityId { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 统计日
        /// </summary>
        public DateTime DDate { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 产量
        /// </summary>
        public double OutPut { get; set; }

        /// <summary>
        /// 产值
        /// </summary>
        public double OutValue { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
    }
}

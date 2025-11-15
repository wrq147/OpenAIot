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
    [TableName("t_eng_factortype")]
    public class T_ENG_FactorType : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 排放分类（0代表目录 1代表分类）
        /// </summary>
        public bool FactorType { get; set; }

        /// <summary>
        /// 小数位数
        /// </summary>
        public int FactorDigits { get; set; }

        /// <summary>
        /// 因子名称
        /// </summary>
        public string NameTitle { get; set; }

        /// <summary>
        /// 数值名称
        /// </summary>
        public string FactorTitle { get; set; }

        /// <summary>
        /// 因子单位
        /// </summary>
        public string FactorUnit { get; set; }

        /// <summary>
        /// 活动水平单位
        /// </summary>
        public string ActivityUnit { get; set; }

        /// <summary>
        /// 单位换算
        /// </summary>
        public double ActivityConversion { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
    }
}

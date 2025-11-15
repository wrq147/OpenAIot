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
    [TableName("t_eng_factor")]
    public class T_ENG_Factor : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 排放因子分类编码
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// 因子名称
        /// </summary>
        public string FactorName { get; set; }

        /// <summary>
        /// 因子数值
        /// </summary>
        public double EmissionFactor { get; set; }

        /// <summary>
        /// 平均低位发热量
        /// </summary>
        public double AvgCalorific { get; set; }

        /// <summary>
        /// 折标准煤系数
        /// </summary>
        public double EqCoal { get; set; }

        /// <summary>
        /// 来源说明
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Share;
using MyAccess.DB.Attr;

namespace EfficiencyService.Model
{
    public class In_Factor
    {
        /// <summary>
        /// 年份
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 排放因子类型
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

    }

    public class In_Factor2
    {
        /// <summary>
        /// 编码
        /// </summary>
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
    }
}

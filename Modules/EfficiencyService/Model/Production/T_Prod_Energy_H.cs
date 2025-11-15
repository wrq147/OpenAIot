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
    [TableName("t_prod_energy_h")]
    public class T_Prod_Energy_H
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
        /// 设备编码
        /// </summary>
        public string EquipmentId { get; set; }

        /// <summary>
        /// 排放因子编码
        /// </summary>
        public string FactorId { get; set; }

        /// <summary>
        /// 排放因子名称
        /// </summary>
        public string FactorName { get; set; }

        /// <summary>
        /// 统计日
        /// </summary>
        public DateTime DDate { get; set; }

        /// <summary>
        /// 统计时段
        /// </summary>
        public string TTime { get; set; }

        /// <summary>
        /// 表码期初值
        /// </summary>
        public double InitVale { get; set; }

        /// <summary>
        /// 表码期末值
        /// </summary>
        public double EndVale { get; set; }

        /// <summary>
        /// 能源消耗量
        /// </summary>
        public double UseVale { get; set; }

        /// <summary>
        /// 一级单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 二级单位
        /// </summary>
        public string LageUnit { get; set; }
    }

    public class T_Prod_Energy_Hour
    {
        /// <summary>
        /// 统计日
        /// </summary>
        public DateTime DDate { get; set; }

        /// <summary>
        /// 统计时段
        /// </summary>
        public string TTime { get; set; }

        /// <summary>
        /// 能源消耗量
        /// </summary>
        public double UseVale { get; set; }
    }

    public class Out_Energy_Hour
    {
        /// <summary>
        /// 时段类型
        /// </summary>
        public string PolicyType { get; set; }

        /// <summary>
        /// 能源消耗量
        /// </summary>
        public double UseVale { get; set; }

        /// <summary>
        /// 一级单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 二级单位
        /// </summary>
        public string LageUnit { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public double CostVale { get; set; }

        /// <summary>
        /// 碳排放
        /// </summary>
        public double CarbonEmission { get; set; }

        /// <summary>
        /// 折标煤
        /// </summary>
        public double ConvertCoal { get; set; }

        /// <summary>
        /// 时段明细
        /// </summary>
        public List<Out_Energy_Time> Times { get; set; }
    }

    public class Out_Energy_Time
    {
        /// <summary>
        /// 统计时段
        /// </summary>
        public string TTime { get; set; }

        /// <summary>
        /// 能源消耗量
        /// </summary>
        public double UseVale { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public double CostVale { get; set; }

        /// <summary>
        /// 碳排放
        /// </summary>
        public double CarbonEmission { get; set; }

        /// <summary>
        /// 折标煤
        /// </summary>
        public double ConvertCoal { get; set; }
    }
    public class Out_Energy_Strategy
    {
        /// <summary>
        /// 统计时段
        /// </summary>
        public string TTime { get; set; }

        /// <summary>
        /// 能源消耗量
        /// </summary>
        public double UseVale { get; set; }

        /// <summary>
        /// 节约成本
        /// </summary>
        public double SaveCost { get; set; }

        /// <summary>
        /// 当地电价
        /// </summary>
        public List<Out_Energy_Strategy_D> Policy { get; set; }
    }

    public class Out_Energy_Strategy_D
    { 
        /// <summary>
        /// 时段类型
        /// </summary>
        public string PolicyType { get; set; }

        /// <summary>
        /// 统计时段
        /// </summary>
        public List<string> TTime { get; set; }

        /// <summary>
        /// 电力单价
        /// </summary>
        public double Price { get; set; }
    }
}

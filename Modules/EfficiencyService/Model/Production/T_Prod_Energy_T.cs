using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyAccess.DB.Attr;
using Common.Share;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;

namespace EfficiencyService.Model.Production
{
    [TableName("t_prod_energy_t")]
    public class T_Prod_Energy_T
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
        /// 一级单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 二级单位
        /// </summary>
        public string LageUnit { get; set; }

        /// <summary>
        /// 能源消耗量
        /// </summary>
        public double UseVale { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public double CostVale { get; set; }

        /// <summary>
        /// 碳排放量
        /// </summary>
        public double CarbonEmission { get; set; }

        /// <summary>
        /// 折标准煤
        /// </summary>
        public double ConvertCoal { get; set; }

        /// <summary>
        /// 时段类型（尖/峰/平/谷）
        /// </summary>
        public string TimePeriod { get; set; }
    }

    public class T_Prod_Energy_T2 : T_Prod_Energy_T
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }
    }
    public class Energy_T_Reuslt
    {
        /// <summary>
        /// 时段类型（尖/峰/平/谷）
        /// </summary>
        public string TimePeriod { get; set; }

        /// <summary>
        /// 一级单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 二级单位
        /// </summary>
        public string LageUnit { get; set; }

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
        /// 成本
        /// </summary>
        public double CostVale { get; set; }

        /// <summary>
        /// 碳排放量
        /// </summary>
        public double CarbonEmission { get; set; }

        /// <summary>
        /// 折标准煤
        /// </summary>
        public double ConvertCoal { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// 明细
        /// </summary>
        public List<Energy_T_Reuslt_Detail> Details { get; set; }
    }

    public class Energy_T_Reuslt_Detail
    {
        /// <summary>
        /// 时段
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
        /// 碳排放量
        /// </summary>
        public double CarbonEmission { get; set; }

        /// <summary>
        /// 折标准煤
        /// </summary>
        public double ConvertCoal { get; set; }
    }

    public class Energy_F_Reuslt
    {
        /// <summary>
        /// 一级单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 二级单位
        /// </summary>
        public string LageUnit { get; set; }

        /// <summary>
        /// 能源消耗量
        /// </summary>
        public double UseVale { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public double CostVale { get; set; }

        /// <summary>
        /// 碳排放量
        /// </summary>
        public double CarbonEmission { get; set; }

        /// <summary>
        /// 折标准煤
        /// </summary>
        public double ConvertCoal { get; set; }

        /// <summary>
        /// 设施明细
        /// </summary>
        public List<Energy_F_F> Facilitys { get; set; }
        
        /// <summary>
        /// 时段明细
        /// </summary>
        public List<Energy_F_D> Times { get; set; }

        /// <summary>
        /// 分时时段明细
        /// </summary>
        public List<Energy_T_Reuslt> TimePeriods { get; set; }
    }

    public class Energy_F_F
    {
        /// <summary>
        /// 设施名称
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// 能源消耗量
        /// </summary>
        public double UseVale { get; set; }
    }

    public class Energy_F_D
    {
        /// <summary>
        /// 时段
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
        /// 碳排放量
        /// </summary>
        public double CarbonEmission { get; set; }

        /// <summary>
        /// 折标准煤
        /// </summary>
        public double ConvertCoal { get; set; }
    }
}

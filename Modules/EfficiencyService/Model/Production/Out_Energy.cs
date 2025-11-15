using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model
{
    public class Out_Energy : T_Prod_Energy
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }
    }

    public class Out_EnergyDay
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }

        /// <summary>
        /// 能源名称
        /// </summary>
        public string FactorName { get; set; }

        /// <summary>
        /// 能源类型
        /// </summary>
        public string FactorId { get; set; }

        /// <summary>
        /// 统计日
        /// </summary>
        public string DDate { get; set; }

        /// <summary>
        /// 能源消耗量
        /// </summary>
        public double UseVale { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public double CostVale { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 数据来源 1 人工,2 系统
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// 一级单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 二级单位
        /// </summary>
        public string LageUnit { get; set; }

        /// <summary>
        /// 碳排放
        /// </summary>
        public double CarbonEmission { get; set; }

        /// <summary>
        /// 折标煤
        /// </summary>
        public double ConvertCoal { get; set; }

    }

    public class Out_FacilityDay
    {
        /// <summary>
        /// 设施编码
        /// </summary>
        public string FacilityId { get; set; }

        /// <summary>
        /// 设施名称
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// 能源名称
        /// </summary>
        public string FactorName { get; set; }

        /// <summary>
        /// 能源类型
        /// </summary>
        public string FactorId { get; set; }

        /// <summary>
        /// 统计日
        /// </summary>
        public string DDate { get; set; }

        /// <summary>
        /// 能源消耗量
        /// </summary>
        public double UseVale { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public double CostVale { get; set; }

        /// <summary>
        /// 数据来源
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 一级单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 二级单位
        /// </summary>
        public string LageUnit { get; set; }

        /// <summary>
        /// 碳排放
        /// </summary>
        public double CarbonEmission { get; set; }

        /// <summary>
        /// 折标煤
        /// </summary>
        public double ConvertCoal { get; set; }

    }

    public class Out_Bench
    {
        /// <summary>
        /// 设施名称
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// 能源名称
        /// </summary>
        public string FactorName { get; set; }

        /// <summary>
        /// 能源消耗量
        /// </summary>
        public double UseVale { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public double CostVale { get; set; }

        /// <summary>
        /// 一级单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 二级单位
        /// </summary>
        public string LageUnit { get; set; }

        /// <summary>
        /// 碳排放
        /// </summary>
        public double CarbonEmission { get; set; }

        /// <summary>
        /// 折标煤
        /// </summary>
        public double ConvertCoal { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string ProductUnit { get; set; }

        /// <summary>
        /// 产量
        /// </summary>
        public double OutPut { get; set; }

        /// <summary>
        /// 产值
        /// </summary>
        public double OutValue { get; set; }

        /// <summary>
        /// 能耗能效
        /// </summary>
        public double UseEfficiency { get; set; }

        /// <summary>
        /// 成本能效
        /// </summary>
        public double CostEfficiency { get; set; }

        /// <summary>
        /// 时段明细
        /// </summary>
        public List<Out_BenchDetail> Details { get; set; }
    }

    public class Out_BenchDetail
    {
        /// <summary>
        /// 时间段
        /// </summary>
        public string DDate { get; set; }
        /// <summary>
        /// 设施名称
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// 能源名称
        /// </summary>
        public string FactorName { get; set; }

        /// <summary>
        /// 能源消耗量
        /// </summary>
        public double UseVale { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public double CostVale { get; set; }

        /// <summary>
        /// 一级单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 二级单位
        /// </summary>
        public string LageUnit { get; set; }

        /// <summary>
        /// 碳排放
        /// </summary>
        public double CarbonEmission { get; set; }

        /// <summary>
        /// 折标煤
        /// </summary>
        public double ConvertCoal { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string ProductUnit { get; set; }
        /// <summary>
        /// 产量
        /// </summary>
        public double OutPut { get; set; }

        /// <summary>
        /// 产值
        /// </summary>
        public double OutValue { get; set; }

        /// <summary>
        /// 能耗能效
        /// </summary>
        public double UseEfficiency { get; set; }

        /// <summary>
        /// 成本能效
        /// </summary>
        public double CostEfficiency { get; set; }
    }
}

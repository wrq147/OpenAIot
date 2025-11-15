using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model.Common
{
    public class Out_Facility
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设施名称
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// 设施代码
        /// </summary>
        public string FacilityCode { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string Manager { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 父节点ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 设施分类（False代表目录级 Ture代表绑定设备级）
        /// </summary>
        public bool FacilityType { get; set; }

        /// <summary>
        /// 子设施
        /// </summary>
        public List<Out_Facility> Children { get; set; }

        /// <summary>
        /// 绑定设备
        /// </summary>
        public List<T_Com_Equipment> Equipments { get; set; }
    }

    public class Out_FacilityEnergy
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设施名称
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string Manager { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 父节点ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 设施分类（False代表目录级 Ture代表绑定设备级）
        /// </summary>
        public bool FacilityType { get; set; }

        /// <summary>
        /// 子设施
        /// </summary>
        public List<Out_FacilityEnergy> Children { get; set; }

        /// <summary>
        /// 设施下的设备
        /// </summary>
        public List<Out_EquipmentEnergy> Equipments { get; set; }

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

        /// <summary>
        /// 设备分类（False代表设施级 Ture代表设备级）
        /// </summary>
        public bool EquipmentType { get; set; }
    }

    public class Out_EquipmentEnergy
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }

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
}

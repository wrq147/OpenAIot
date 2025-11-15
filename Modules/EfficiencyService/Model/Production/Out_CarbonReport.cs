using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model
{
    public class Out_CarbonReportType
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 排放类别序号
        /// </summary>
        public string ClassNo { get; set; }

        /// <summary>
        /// 排放类别名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 排放范围（1代表范围1 2代表范围2 3代表范围3）
        /// </summary>
        public string RangeId { get; set; }

        /// <summary>
        /// 子排放类型
        /// </summary>
        public Dictionary<string,List<T_OrgClassCarbonReport>> OrgClasses { get; set; }
    }

    public class T_OrgClassCarbonReport
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 类别编码
        /// </summary>
        public string ClassId { get; set; }

        /// <summary>
        /// 子类别编码
        /// </summary>
        public string SubClassId { get; set; }

        /// <summary>
        /// 设施编码
        /// </summary>
        public string FacilityId { get; set; }

        /// <summary>
        /// 排放因子编码
        /// </summary>
        public string FactorId { get; set; }

        /// <summary>
        /// 排放类型编码
        /// </summary>
        public string FactorType { get; set; }

        /// <summary>
        /// 活动数据来源（1代表计量设备 2代表手工录入 3供应链数据）
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// 关联设备编码  123,456
        /// </summary>
        public string EquipmentIds { get; set; }

        /// <summary>
        /// 子类型名称
        /// </summary>
        public string SubClassName { get; set; }

        /// <summary>
        /// 设施名称
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// 因子名称
        /// </summary>
        public string FactorName { get; set; }

        /// <summary>
        /// 因子值
        /// </summary>
        public double EmissionFactor { get; set; }

        /// <summary>
        /// 排放类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 排放因子单位
        /// </summary>
        public string FactorUnit { get; set; }

        /// <summary>
        /// 活动数据单位
        /// </summary>
        public string ActivityUnit { get; set; }

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
        /// 产值
        /// </summary>
        public double OutValue { get; set; }
    }
    public class In_CarbonReport
    {
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 起始日期
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { get; set; }
    }

}

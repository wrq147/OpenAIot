using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model
{
    public class Out_EquipmentStatistics
    {
        /// <summary>
        /// 设备数
        /// </summary>
        public int EquipemntCount { get; set; }
        /// <summary>
        /// 计量设备数
        /// </summary>
        public int DataCount { get; set; }
        /// <summary>
        /// 联网设备数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 在线数
        /// </summary>
        public int OnlineCount { get; set; }
        /// <summary>
        /// 离线数
        /// </summary>
        public int OfflineCount { get; set; }
        /// <summary>
        /// 未知数
        /// </summary>
        public int UnknowCount { get; set; }
        /// <summary>
        /// 待处理事件数
        /// </summary>
        public int EventCount { get; set; }

        /// <summary>
        /// 设备类型分布
        /// </summary>
        public List<Out_EquipemntType> EquipemntTypes { get; set; }

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
    public class Out_EquipemntType
    { 
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 类型编码
        /// </summary>
        public string EnergyType { get; set; }

        /// <summary>
        /// 类型数量
        /// </summary>
        public int TypeCount { get; set; }

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
        /// 碳排放量
        /// </summary>
        public double CarbonEmission { get; set; }

        /// <summary>
        /// 折标准煤
        /// </summary>
        public double ConvertCoal { get; set; }

    }
}

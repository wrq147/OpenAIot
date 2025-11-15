using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyAccess.DB.Attr;

namespace EfficiencyService.Model
{
    public class In_EnergyHand
    {
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long OrgId { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string EquipmentId { get; set; }

        /// <summary>
        /// 统计日
        /// </summary>
        public string DDate { get; set; }


        /// <summary>
        /// 表码期初值
        /// </summary>
        public double InitVale { get; set; }

        /// <summary>
        /// 表码期末值
        /// </summary>
        public double EndVale { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public double CostVale { get; set; }
    }

    public class In_EnergyHandUpdate
    {

        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 表码期初值
        /// </summary>
        public double InitVale { get; set; }

        /// <summary>
        /// 表码期末值
        /// </summary>
        public double EndVale { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public double CostVale { get; set; }
    }

    public class InCalEnergy
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public string beginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string endDate { get; set; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public long orgId { get; set; }
    }

    public class InEnergyHour
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public string beginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string endDate { get; set; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public long orgId { get; set; }

        /// <summary>
        /// 设施编码
        /// </summary>
        public string FacilityId { get; set; }
    }

    public class InEnergyHourList
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public string beginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string endDate { get; set; }
        /// <summary>
        /// 设备编码
        /// </summary>
        public string EquipmentId { get; set; }
        /// <summary>
        /// 设施编码
        /// </summary>
        public string FacilityId { get; set; }
        /// <summary>
        /// 排放因子
        /// </summary>
        public string FactorId { get; set; }
        /// <summary>
        /// 时间类型
        /// </summary>
        public string TimeType { get; set; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public long orgId { get; set; }
    }
}

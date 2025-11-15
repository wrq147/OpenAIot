using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Share;

namespace EfficiencyService.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class InEnergyPageList : BaseQueryParam
    {
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 设施编码
        /// </summary>
        public string FacilityId { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string EquipmentId { get; set; }

        /// <summary>
        ///  数据来源 1 人工,2 系统
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// 排放因子编码
        /// </summary>
        public string FactorId { get; set; }

        /// <summary>
        /// 起始日期
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InEnergyDayPageList : BaseQueryParam
    {
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 设施编码
        /// </summary>
        public string FacilityId { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string EquipmentId { get; set; }

        /// <summary>
        /// 数据来源 1:代表人工采集 2代表系统对接
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// 能源消耗
        /// </summary>
        public double UseVale { get; set; }

        /// <summary>
        /// 排放因子编码
        /// </summary>
        public string FactorId { get; set; }

        /// <summary>
        /// 起始日期
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 运算符
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 日期类型
        /// </summary>
        public string DateType { get; set; }
    }

    public class BenchEnergyDate
    {
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 对标类型
        /// </summary>
        public string BenchType { get; set; }

        /// <summary>
        /// 设施/设备编码
        /// </summary>
        public string Ids { get; set; }

        /// <summary>
        /// 排放因子编码
        /// </summary>
        public string FactorId { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 起始日期
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 日期类型
        /// </summary>
        public string DateType { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InEnergyTree
    {
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 排放因子编码
        /// </summary>
        public string FactorId { get; set; }

        /// <summary>
        /// 起始日期
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string EquipmentId { get; set; }
    }
}

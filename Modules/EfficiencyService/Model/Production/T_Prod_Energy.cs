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
    [TableName("t_prod_energy")]
    public class T_Prod_Energy : BaseEntity
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
        /// 计费政策编码
        /// </summary>
        public string PolicyId { get; set; }

        /// <summary>
        /// 计费政策名称
        /// </summary>
        public string PolicyName { get; set; }

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
        /// 碳排放量
        /// </summary>
        public double CarbonEmission { get; set; }

        /// <summary>
        /// 折标准煤
        /// </summary>
        public double ConvertCoal { get; set; }

        /// <summary>
        /// 时段类型（1代表尖 2代表峰 3代表平 4代表谷 5代表全天）
        /// </summary>
        public string TimePeriod { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }

        /// <summary>
        /// 数据来源 1,2
        /// </summary>
        public string DataSource { get; set; }
    }

    public class Out_EnergyList : T_Prod_Energy
    {
        /// <summary>
        /// 设施编码
        /// </summary>
        public string FacilityId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }
    }
}

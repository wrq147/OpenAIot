using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Share;
using MyAccess.DB.Attr;

namespace EfficiencyService.Model
{
    public class In_Policy
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
        /// 政策名称
        /// </summary>
        public string PolicyName { get; set; }

        /// <summary>
        /// 排放因子
        /// </summary>
        public string FactorId { get; set; }

        /// <summary>
        /// 能源类型
        /// </summary>
        public string EnergyType { get; set; }

        /// <summary>
        /// 能源类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 一级计量单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 二级计量单位
        /// </summary>
        public string LageUnit { get; set; }

        /// <summary>
        /// 来源说明
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }

        /// <summary>
        /// 政策详情
        /// </summary>
        public List<In_PolicyDetil> PolicyDetils { get; set; }
    }
    public class In_PolicyDetil 
    {
        /// <summary>
        /// 生效月份
        /// </summary>
        public string PolicyMonth { get; set; }

        /// <summary>
        /// 计费方式（1代表分时 2代表不分时 3代表阶梯）
        /// </summary>
        public string PolicyType { get; set; }

        /// <summary>
        /// 尖价格 
        /// </summary>
        public double JianPrice { get; set; }

        /// <summary>
        /// 峰价格  
        /// </summary>
        public double FengPrice { get; set; }

        /// <summary>
        /// 平价格
        /// </summary>
        public double PingPrice { get; set; }

        /// <summary>
        /// 谷价格  
        /// </summary>
        public double GuPrice { get; set; }

        /// <summary>
        /// 尖峰平谷时段
        /// </summary>
        public List<In_PriceTime> PriceTimes { get; set; }

        /// <summary>
        /// 全天电价
        /// </summary>
        public double DayPrice { get; set; }


        /// <summary>
        /// 计费周期（1月 2季度 3年）
        /// </summary>
        public string CycleType { get; set; }

        /// <summary>
        /// 阶梯价格
        /// </summary>
        public List<In_PriceTier> PriceTiers { get; set; }
    }

    public class In_PriceTime
    {

        /// <summary>
        /// 时段类型（1代表尖 2代表峰 3代表平 4代表谷 5代表全天）
        /// </summary>
        public string TimePeriod { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }
    }
    public class In_PriceTier
    {

        /// <summary>
        /// 阶梯级别（1代表第一档 2代表第二档 3代表第三档）
        /// </summary>
        public string TierLevel { get; set; }

        /// <summary>
        /// 最小用电量
        /// </summary>
        public double MinKwh { get; set; }

        /// <summary>
        /// 最大用电量
        /// </summary>
        public double MaxKwh { get; set; }

        /// <summary>
        /// 电价
        /// </summary>
        public double Price { get; set; }
    }

    public class In_PolicyList : BaseQueryParam
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
        /// 政策名称
        /// </summary>
        public string PolicyName { get; set; }

        /// <summary>
        /// 能源类型
        /// </summary>
        public string EnergyType { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string Unit { get; set; }
    }
}

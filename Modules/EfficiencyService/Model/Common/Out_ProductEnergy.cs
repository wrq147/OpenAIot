using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model
{
    public class Out_ProductEnergy : T_Com_Product
    {
        /// <summary>
        /// 产量
        /// </summary>
        public double OutPut { get; set; }

        /// <summary>
        /// 消耗能源
        /// </summary>
        public double UseVale { get; set; }

        /// <summary>
        /// 能源一级单位
        /// </summary>
        public string EUnit { get; set; }

        /// <summary>
        /// 能源二级单位
        /// </summary>
        public string LageEUnit { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 类型编码
        /// </summary>
        public string EnergyType { get; set; }
    }

    /// <summary>
    ///分页查询产品能效参数
    /// </summary>
    public class In_ProductEnergyPageList : BaseQueryParam
    {
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 类型编码
        /// </summary>
        public string EnergyType { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public string beginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string endDate { get; set; }
    }
}

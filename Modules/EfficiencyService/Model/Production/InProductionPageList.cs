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
    public class InProductionPageList : BaseQueryParam
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
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 产品类型
        /// </summary>
        public string ProductType { get; set; }

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
    public class InProductionDayPageList : BaseQueryParam
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
        /// 产量
        /// </summary>
        public double OutPut { get; set; }


        /// <summary>
        /// 运算符
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 日期类型
        /// </summary>
        public string DateType { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InFacilityProductList
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
        /// 起始日期
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { get; set; }

    }
}

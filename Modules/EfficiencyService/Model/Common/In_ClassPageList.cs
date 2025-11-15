using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Share;
using MyAccess.DB.Attr;

namespace EfficiencyService.Model
{
    public class In_ClassPageLis: BaseQueryParam
    {
        /// <summary>
        /// 子类型名称
        /// </summary>
        public string SubClassName { get; set; }
    }

    public class In_ModelPageLis : BaseQueryParam
    {
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品生命周期边界
        /// </summary>
        public string ProductBorder { get; set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public long? createId { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductModel { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 不为空返回碳排放量
        /// </summary>
        public string CarbonEmission { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? beginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? endDate { get; set; }
    }
}

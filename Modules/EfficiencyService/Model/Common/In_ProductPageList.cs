using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Share;
using MyAccess.DB.Attr;

namespace EfficiencyService.Model
{
    /// <summary>
    ///分页查询产品参数
    /// </summary>
    public class In_ProductPageList : BaseQueryParam
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
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 产品形态 1代表成品 2代表配套件
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductModel { get; set; }
    }
}

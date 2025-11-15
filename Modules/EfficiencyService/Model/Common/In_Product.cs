using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model
{
    /// <summary>
    /// 产品信息参数
    /// </summary>
    public class In_Product
    {
        /// <summary>
        /// 编码，新增不用传
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductModel { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品简称或缩写
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
        /// 零售单价（元）
        /// </summary>
        public double ProductPrice { get; set; }

        /// <summary>
        /// 是否上市 是/否
        /// </summary>
        public string OnMarket { get; set; }

        /// <summary>
        /// 上市时间
        /// </summary>
        public DateTime? MarketTime { get; set; }


        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 产品说明
        /// </summary>
        public string Memo { get; set; }
    }
}

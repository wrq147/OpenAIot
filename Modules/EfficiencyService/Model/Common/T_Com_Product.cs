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
    [TableName("t_com_product")]
    public class T_Com_Product: BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
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
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 是否上市 是/否
        /// </summary>
        public string OnMarket { get; set; }

        /// <summary>
        /// 上市时间
        /// </summary>
        public DateTime MarketTime { get; set; }

        /// <summary>
        /// 产品说明
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
    }
}

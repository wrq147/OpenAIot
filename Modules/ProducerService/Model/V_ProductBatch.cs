using Common.Attr;
using MyAccess.DB.Attr;
using System;

namespace ProducerService.Model
{
    [TableName("mz_product_batch_v")]
    public class V_ProductBatch: MZ_ProductBatch
    {
        /// <summary>
        /// 产品编号(sku编码)
        /// </summary>
        [DataIgnore]
        public string SkuNumber { get; set; }
        /// <summary>
        /// 关联的物联产品Id
        /// </summary>
        [DataIgnore]
        public string IOTProductId { get; set; }
        /// <summary>
        /// 产品标签：U半成品，F成品
        /// </summary>
        [DataIgnore]
        public string ProductLabel { get; set; }
        /// <summary>
        /// 产品分组Id，为空所属全部
        /// </summary>
        [DataIgnore]
        public string TypeId { get; set; }
        /// <summary>
        /// 产品分组
        /// </summary>
        [DataIgnore]
        public string TypeName { get; set; }
        /// <summary>
        /// 产品分组对应的产品属性，可为空
        /// </summary>
        [DataIgnore]
        public string Prop { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        [DataIgnore]
        public string ProductName { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        [DataIgnore]
        public string Unit { get; set; }
        /// <summary>
        /// 产品规格
        /// </summary>
        [DataIgnore]
        public string Specs { get; set; }
        /// <summary>
        /// 成本单价
        /// </summary>
        [DataIgnore]
        public decimal? Price { get; set; }
        /// <summary>
        /// 销售单价
        /// </summary>
        [DataIgnore]
        public decimal? SalesPrice { get; set; }
        /// <summary>
        /// 工艺路线
        /// </summary>
        [DataIgnore]
        public string Route { get; set; }
        /// <summary>
        /// 供应商Id
        /// </summary>
        [DataIgnore]
        public string Supplier { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        [DataIgnore]
        public string SupplierName { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        [DataIgnore]
        public string Remark { get; set; }
    }
}

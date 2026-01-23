using AuthService.Fields;
using AuthService.Model;
using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace ProducerService.Model
{
    /// <summary>
    /// 产品表（半成品、成品）
    /// </summary>
    [TableName("mz_product")]
    public class MZ_Product : BaseEntity, IFieldEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        [OnlySeriaize]
        public long? OrgId { get; set; }
        /// <summary>
        /// 产品编号(sku编码)
        /// </summary>
        public string SkuNumber { get; set; }
        /// <summary>
        /// 关联的物联协议Id
        /// </summary>
        public string IOTProductId { get; set; }
        /// <summary>
        /// 产品标签：M原材料，U半成品，F成品
        /// </summary>
        public string ProductLabel { get; set; }
        /// <summary>
        /// 产品分组Id，为空所属全部
        /// </summary>
        public string TypeId { get; set; }
        /// <summary>
        /// 产品分组对应的产品属性，可为空
        /// </summary>
        public string Prop { get; set; }
        /// <summary>
        /// 生产来源：自制，外购，委外
        /// </summary>
        public string ProductFrom { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 包装单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 产品规格
        /// </summary>
        public string Specs { get; set; }
        /// <summary>
        /// 总计量
        /// </summary>
        public decimal? Total { get; set; }
        /// <summary>
        /// 最小计量单位
        /// </summary>
        public string MinUnit { get; set; }
        /// <summary>
        /// 成本单价
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 销售单价
        /// </summary>
        public decimal? SalesPrice { get; set; }
        /// <summary>
        /// 工艺路线Id
        /// </summary>
        public string Route { get; set; }
        /// <summary>
        /// 工艺路线名称
        /// </summary>
        [DataIgnore]
        public string RouteName { get; set; }
        /// <summary>
        /// 供应商Id
        /// </summary>
        public string Supplier { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        [DataIgnore]
        public string SupplierName { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 产品分组名称
        /// </summary>
        [DataIgnore]
        public string TypeName { get; set; }
  
        [DataIgnore]
        public Dictionary<string, object> ExtVals { get; set; }
        /// <summary>
        /// 扩展的关联对象
        /// </summary>
        [DataIgnore]
        [OnlySeriaize]
        public Dictionary<string, object> ExtObjects { get; set; }

        public string GetFormId()
        {
            return this.Id;
        }

        public string GetFormName()
        {
            return "产品";
        }
    }
}

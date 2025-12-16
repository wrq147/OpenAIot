using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace ProducerService.Model
{
    /// <summary>
    /// 产品表（半成品、成品）
    /// </summary>
    [TableName("mz_product")]
    public class MZ_Product : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        [JsonConverter(typeof(OnlySeriaize))]
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
        public string StrExt1 { get; set; }
        public string StrExt2 { get; set; }
        public string StrExt3 { get; set; }
        public string StrExt4 { get; set; }
        public string StrExt5 { get; set; }
        public string StrExt6 { get; set; }
        public string StrExt7 { get; set; }
        public string StrExt8 { get; set; }
        public string StrExt9 { get; set; }
        public string StrExt10 { get; set; }
        public string StrExt11 { get; set; }
        public string StrExt12 { get; set; }
        public string StrExt13 { get; set; }
        public string StrExt14 { get; set; }
        public string StrExt15 { get; set; }
        public string StrExt16 { get; set; }
        public string StrExt17 { get; set; }
        public string StrExt18 { get; set; }
        public string StrExt19 { get; set; }
        public string StrExt20 { get; set; }
        public string StrExt21 { get; set; }
        public string StrExt22 { get; set; }
        public string StrExt23 { get; set; }
        public string StrExt24 { get; set; }
        public string StrExt25 { get; set; }
        public string StrExt26 { get; set; }
        public string StrExt27 { get; set; }
        public string StrExt28 { get; set; }
        public string StrExt29 { get; set; }
        public string StrExt30 { get; set; }
        public string StrExt31 { get; set; }
        public string StrExt32 { get; set; }
        public string StrExt33 { get; set; }
        public string StrExt34 { get; set; }
        public string StrExt35 { get; set; }
        public string StrExt36 { get; set; }
        public string StrExt37 { get; set; }
        public string StrExt38 { get; set; }
        public string StrExt39 { get; set; }
        public string StrExt40 { get; set; }
        public string StrExt41 { get; set; }
        public string StrExt42 { get; set; }
        public string StrExt43 { get; set; }
        public string StrExt44 { get; set; }
        public string StrExt45 { get; set; }
        public string StrExt46 { get; set; }
        public string StrExt47 { get; set; }
        public string StrExt48 { get; set; }
        public string StrExt49 { get; set; }
        public string StrExt50 { get; set; }
        public double? NumExt1 { get; set; }
        public double? NumExt2 { get; set; }
        public double? NumExt3 { get; set; }
        public double? NumExt4 { get; set; }
        public double? NumExt5 { get; set; }
        public double? NumExt6 { get; set; }
        public double? NumExt7 { get; set; }
        public double? NumExt8 { get; set; }
        public double? NumExt9 { get; set; }
        public double? NumExt10 { get; set; }
        public double? NumExt11 { get; set; }
        public double? NumExt12 { get; set; }
        public double? NumExt13 { get; set; }
        public double? NumExt14 { get; set; }
        public double? NumExt15 { get; set; }
        public double? NumExt16 { get; set; }
        public double? NumExt17 { get; set; }
        public double? NumExt18 { get; set; }
        public double? NumExt19 { get; set; }
        public double? NumExt20 { get; set; }
        public double? NumExt31 { get; set; }
        public double? NumExt32 { get; set; }
        public double? NumExt33 { get; set; }
        public double? NumExt34 { get; set; }
        public double? NumExt35 { get; set; }
        public double? NumExt36 { get; set; }
        public double? NumExt37 { get; set; }
        public double? NumExt38 { get; set; }
        public double? NumExt39 { get; set; }
        public double? NumExt40 { get; set; }
        public double? NumExt41 { get; set; }
        public double? NumExt42 { get; set; }
        public double? NumExt43 { get; set; }
        public double? NumExt44 { get; set; }
        public double? NumExt45 { get; set; }
        public double? NumExt46 { get; set; }
        public double? NumExt47 { get; set; }
        public double? NumExt48 { get; set; }
        public double? NumExt49 { get; set; }
        public double? NumExt50 { get; set; }
        /// <summary>
        /// 产品分组名称
        /// </summary>
        [DataIgnore]
        public string TypeName { get; set; }
        /// <summary>
        /// 扩展的关联对象
        /// </summary>
        [DataIgnore]
        [JsonConverter(typeof(OnlySeriaize))]
        public Dictionary<string, object> ExtObjects { get; set; }

    }
}

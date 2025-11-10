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
        /// 关联的物联产品Id
        /// </summary>
        public string IOTProductId { get; set; }
        /// <summary>
        /// 关联的物联产品名称
        /// </summary>
        [DataIgnore]
        public string IOTProductName { get; set; }
        /// <summary>
        /// 产品标签：U半成品，F成品
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
        /// 扩展字符串字段1
        /// </summary>
        public string StrExt1 { get; set; }
        /// <summary>
        /// 扩展字符串字段2
        /// </summary>
        public string StrExt2 { get; set; }
        /// <summary>
        /// 扩展字符串字段3
        /// </summary>
        public string StrExt3 { get; set; }
        /// <summary>
        /// 扩展字符串字段4
        /// </summary>
        public string StrExt4 { get; set; }
        /// <summary>
        /// 扩展字符串字段5
        /// </summary>
        public string StrExt5 { get; set; }
        /// <summary>
        /// 扩展字符串字段6
        /// </summary>
        public string StrExt6 { get; set; }
        /// <summary>
        /// 扩展字符串字段7
        /// </summary>
        public string StrExt7 { get; set; }
        /// <summary>
        /// 扩展字符串字段8
        /// </summary>
        public string StrExt8 { get; set; }
        /// <summary>
        /// 扩展字符串字段9
        /// </summary>
        public string StrExt9 { get; set; }
        /// <summary>
        /// 扩展字符串字段10
        /// </summary>
        public string StrExt10 { get; set; }
        /// <summary>
        /// 扩展字符串字段11
        /// </summary>
        public string StrExt11 { get; set; }
        /// <summary>
        /// 扩展字符串字段12
        /// </summary>
        public string StrExt12 { get; set; }
        /// <summary>
        /// 扩展字符串字段13
        /// </summary>
        public string StrExt13 { get; set; }
        /// <summary>
        /// 扩展字符串字段14
        /// </summary>
        public string StrExt14 { get; set; }
        /// <summary>
        /// 扩展字符串字段15
        /// </summary>
        public string StrExt15 { get; set; }
        /// <summary>
        /// 扩展字符串字段16
        /// </summary>
        public string StrExt16 { get; set; }
        /// <summary>
        /// 扩展字符串字段17
        /// </summary>
        public string StrExt17 { get; set; }
        /// <summary>
        /// 扩展字符串字段18
        /// </summary>
        public string StrExt18 { get; set; }
        /// <summary>
        /// 扩展字符串字段19
        /// </summary>
        public string StrExt19 { get; set; }
        /// <summary>
        /// 扩展字符串字段20
        /// </summary>
        public string StrExt20 { get; set; }
        /// <summary>
        /// 扩展字符串字段21
        /// </summary>
        public string StrExt21 { get; set; }
        /// <summary>
        /// 扩展字符串字段22
        /// </summary>
        public string StrExt22 { get; set; }
        /// <summary>
        /// 扩展字符串字段23
        /// </summary>
        public string StrExt23 { get; set; }
        /// <summary>
        /// 扩展字符串字段24
        /// </summary>
        public string StrExt24 { get; set; }
        /// <summary>
        /// 扩展字符串字段25
        /// </summary>
        public string StrExt25 { get; set; }
        /// <summary>
        /// 扩展字符串字段26
        /// </summary>
        public string StrExt26 { get; set; }
        /// <summary>
        /// 扩展字符串字段27
        /// </summary>
        public string StrExt27 { get; set; }
        /// <summary>
        /// 扩展字符串字段28
        /// </summary>
        public string StrExt28 { get; set; }
        /// <summary>
        /// 扩展字符串字段29
        /// </summary>
        public string StrExt29 { get; set; }
        /// <summary>
        /// 扩展字符串字段30
        /// </summary>
        public string StrExt30 { get; set; }
        /// <summary>
        /// 扩展数字字段1
        /// </summary>
        public double? NumExt1 { get; set; }
        /// <summary>
        /// 扩展数字字段2
        /// </summary>
        public double? NumExt2 { get; set; }
        /// <summary>
        /// 扩展数字字段3
        /// </summary>
        public double? NumExt3 { get; set; }
        /// <summary>
        /// 扩展数字字段4
        /// </summary>
        public double? NumExt4 { get; set; }
        /// <summary>
        /// 扩展数字字段5
        /// </summary>
        public double? NumExt5 { get; set; }
        /// <summary>
        /// 扩展数字字段6
        /// </summary>
        public double? NumExt6 { get; set; }
        /// <summary>
        /// 扩展数字字段7
        /// </summary>
        public double? NumExt7 { get; set; }
        /// <summary>
        /// 扩展数字字段8
        /// </summary>
        public double? NumExt8 { get; set; }
        /// <summary>
        /// 扩展数字字段9
        /// </summary>
        public double? NumExt9 { get; set; }
        /// <summary>
        /// 扩展数字字段10
        /// </summary>
        public double? NumExt10 { get; set; }
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

using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace ProducerService.Model
{
    /// <summary>
    /// 供应商
    /// </summary>
    [TableName("mz_supplier")]
    public class MZ_Supplier : BaseEntity
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
        /// 供应商编号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }
        /// <summary>
        /// 供应商全称
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 付款期限,单位天
        /// </summary>
        public int? PayTerm { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double? Lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double? Lat { get; set; }
        /// <summary>
        /// 经纬度的geo编码
        /// </summary>
        public string Geo { get; set; }
        /// <summary>
        /// 省市区代码
        /// </summary>
        public string AddressCode { get; set; }
        /// <summary>
        /// 地址名称
        /// </summary>
        public string AddressName { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string AddressDetail { get; set; }
        /// <summary>
        /// 状态：0为停用，1为正常
        /// </summary>
        [JsonConverter(typeof(OnlyDeserialize))]
        public string Status { get; set; }
        /// <summary>
        /// 状态名称
        /// </summary>
        [DataIgnore]
        [JsonConverter(typeof(OnlySeriaize))]
        public string StatusName { get; set; }
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
        /// 扩展的关联对象
        /// </summary>
        [DataIgnore]
        [JsonConverter(typeof(OnlySeriaize))]
        public Dictionary<string, object> ExtObjects { get; set; }
    }
}

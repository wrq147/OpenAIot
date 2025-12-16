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
        /// 扩展的关联对象
        /// </summary>
        [DataIgnore]
        [JsonConverter(typeof(OnlySeriaize))]
        public Dictionary<string, object> ExtObjects { get; set; }
    }
}

using AuthService.Fields;
using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace ProducerService.Model
{
    /// <summary>
    /// 供应商
    /// </summary>
    [TableName("mz_supplier")]
    public class MZ_Supplier : BaseEntity, IFieldEntity
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
        [OnlyDeserialize]
        public string Status { get; set; }
        /// <summary>
        /// 状态名称
        /// </summary>
        [DataIgnore]
        [OnlySeriaize]
        public string StatusName { get; set; }
        /// <summary>
        /// 扩展值
        /// </summary>
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
            return "供应商";
        }
    }
}

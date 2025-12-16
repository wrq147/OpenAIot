using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MESService.Model
{
    /// <summary>
    /// 产品工序
    /// </summary>
    [TableName("mz_product_oper")]
    public class MZ_ProductOper : BaseEntity
    {
        /// <summary>
        /// Id编号
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        [JsonConverter(typeof(OnlySeriaize))]
        public long? OrgId { get; set; }
        /// <summary>
        /// 工序使用的设备，多台设备用逗号分隔
        /// </summary>
        public string DeviceIds { get; set; }
        /// <summary>
        /// 工序名称
        /// </summary>
        public string OperName { get; set; }
        /// <summary>
        /// 允许提交的人员
        /// </summary>
        public string AssignedUser { get; set; }
        /// <summary>
        /// 报工数配比:‌生产计划数 × 报工数配比 = 工序计划数‌
        /// </summary>
        public decimal? PropOf { get; set; }
        /// <summary>
        /// 预计工时(分钟)
        /// </summary>
        public decimal? WorkTime { get; set; }
        /// <summary>
        /// 计件、计时
        /// </summary>
        public string PriceMethod { get; set; }
        /// <summary>
        /// 工资单价
        /// </summary>
        public decimal? UnitPrice { get; set; }
        /// <summary>
        /// 不良品项列表
        /// </summary>
        public string DefectJson { get; set; }
        /// <summary>
        /// 工序的报工表单权限，存储json格式（[{"id":"自定义报工字段Id","title":"字段名称","perm":"R只读.E可编辑,H隐藏"}]）
        /// </summary>
        public string ReportFields { get; set; }
        /// <summary>
        /// 报工表单初始化配置，存储json格式（[{"sid":"自定义工序字段Id","tid":"自定义报工字段Id"}]）,工序与报工字段的类型要一致
        /// </summary>
        public string FieldsInit { get; set; }
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

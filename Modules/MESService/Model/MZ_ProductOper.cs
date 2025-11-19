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

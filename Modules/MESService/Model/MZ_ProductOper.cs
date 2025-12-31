using AuthService.Fields;
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
    public class MZ_ProductOper : BaseEntity, IFieldEntity
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
        /// 扩展的关联对象
        /// </summary>
        [DataIgnore]
        [JsonConverter(typeof(OnlySeriaize))]
        public Dictionary<string, object> ExtObjects { get; set; }
        public Dictionary<string, object> ExtVals { get; set; }

        public string GetFormId()
        {
            return this.Id;
        }

        public string GetFormName()
        {
            return "工序";
        }
    }
}

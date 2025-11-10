using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;

namespace DictService.Model
{
    [TableName("mz_dict_data")]
    public class MZ_DictData : BaseEntity
    {
        /// <summary>
        /// 字典编码
        /// </summary>
        [ID(true)]
        [JsonProperty(PropertyName = "dictCode")]
        public virtual long? dict_code { get; set; }
        /// <summary>
        /// 字典排序
        /// </summary>
        [JsonProperty(PropertyName = "dictSort")]
        public virtual int? dict_sort { get; set; }
        /// <summary>
        /// 字典标签
        /// </summary>
        [JsonProperty(PropertyName = "label")]
        public virtual string dict_label { get; set; }
        /// <summary>
        /// 字典键值
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public virtual string dict_value { get; set; }
        /// <summary>
        /// 字典类型
        /// </summary>
        [JsonProperty(PropertyName = "dictType")]
        public virtual string dict_type { get; set; }
        /// <summary>
        /// 样式属性（其他样式扩展）
        /// </summary>
        [JsonProperty(PropertyName = "cssClass")]
        public virtual string css_class { get; set; }
        /// <summary>
        /// 表格字典样式
        /// </summary>
        [JsonProperty(PropertyName = "listClass")]
        public virtual string list_class { get; set; }
        /// <summary>
        /// 是否默认（Y是 N否）
        /// </summary>
        public virtual string is_default { get; set; }
        /// <summary>
        /// 状态（0正常 1停用）
        /// </summary>
        public virtual string status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string remark { get; set; }

    }
}

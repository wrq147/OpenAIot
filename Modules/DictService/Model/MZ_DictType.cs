using Common.Share;
using MyAccess.DB.Attr;
using System.Text.Json.Serialization;

namespace DictService.Model
{
    [TableName("mz_dict_type")]
    public class MZ_DictType: BaseEntity
    {
        /// <summary>
        /// 字典主键
        /// </summary>
        [ID(true)]
        [JsonPropertyName("dictId")]
        public virtual long? dict_id { get; set; }
        /// <summary>
        /// 字典名称
        /// </summary>
        [JsonPropertyName("dictName")]
        public virtual string dict_name { get; set; }
        /// <summary>
        /// 字典类型
        /// </summary>
        [JsonPropertyName("dictType")]
        public virtual string dict_type { get; set; }
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

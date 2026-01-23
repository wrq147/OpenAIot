using Common.Share;
using MyAccess.DB.Attr;
using System.Text.Json.Serialization;

namespace AuthService
{
    [TableName("mz_config")]
    public class MZ_Config: BaseEntity
    {
        /// <summary>
        /// 参数主键
        /// </summary>
        [ID(true)]
        [JsonPropertyName("configId")]
        public int? config_id { get; set; }
        /// <summary>
        /// 参数名称
        /// </summary>
        [JsonPropertyName("configName")]
        public string config_name { get; set; }
        /// <summary>
        /// 参数键名
        /// </summary>
        [JsonPropertyName("configKey")]
        public string config_key { get; set; }
        /// <summary>
        /// 参数键值
        /// </summary>
        [JsonPropertyName("configValue")]
        public string config_value { get; set; }
        /// <summary>
        /// 系统内置（Y是 N否）
        /// </summary>
        [JsonPropertyName("configType")]
        public string config_type { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
    }
}

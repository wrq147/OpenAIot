using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;

namespace AuthService
{
    [TableName("mz_config")]
    public class MZ_Config: BaseEntity
    {
        /// <summary>
        /// 参数主键
        /// </summary>
        [ID(true)]
        [JsonProperty(PropertyName = "configId")]
        public int? config_id { get; set; }
        /// <summary>
        /// 参数名称
        /// </summary>
        [JsonProperty(PropertyName = "configName")]
        public string config_name { get; set; }
        /// <summary>
        /// 参数键名
        /// </summary>
        [JsonProperty(PropertyName = "configKey")]
        public string config_key { get; set; }
        /// <summary>
        /// 参数键值
        /// </summary>
        [JsonProperty(PropertyName = "configValue")]
        public string config_value { get; set; }
        /// <summary>
        /// 系统内置（Y是 N否）
        /// </summary>
        [JsonProperty(PropertyName = "configType")]
        public string config_type { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
    }
}

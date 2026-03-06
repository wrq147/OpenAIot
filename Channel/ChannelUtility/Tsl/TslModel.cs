using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChannelUtility.Tsl
{
    /// <summary>
    /// 物模型
    /// </summary>
    public class TslModel
    {
        /// <summary>
        /// 属性列表
        /// </summary>
        public List<BaseProperty> properties { get; set; }
        /// <summary>
        /// 功能列表
        /// </summary>
        public List<BaseFunc> functions { get; set; }
        /// <summary>
        /// 事件列表
        /// </summary>
        public List<BaseEvent> events { get; set; }
        /// <summary>
        /// 标签列表（默认包含position标签，表示位置）
        /// </summary>
        public List<BaseTagInfo> tags { get; set; }
        /// <summary>
        /// modbus配置
        /// </summary>
        public ModbusInfo modbus { get; set; }
        /// <summary>
        /// 固件列表,最新的放前面
        /// </summary>
        public List<FirmwareInfo> firmwares { get; set; }

        public static readonly JsonSerializerOptions TSLOptions = new JsonSerializerOptions
        {
            Converters = { new TslJsonConverter(), new TslStringToByteConverter(), new TslNumberToStringConverter(), new JsonObjectConverter() },
            TypeInfoResolver = TslJsonSerializerContext.Default
        };
        public static TslModel CreateFrom(string tsl)
        {
            try
            {
                return JsonSerializer.Deserialize<TslModel>(tsl, TSLOptions);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

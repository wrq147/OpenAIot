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


        /// <summary>
        /// 原数据转换成显示数据
        /// </summary>
        /// <param name="input"></param>
        /// <param name="getProp"></param>
        /// <returns></returns>
        public Dictionary<string, object> RawToProp(IDictionary<string, object> input, Func<string, object> getProp)
        {
            Dictionary<string, object> newout = new Dictionary<string, object>();
            foreach (var bp in properties)
            {
                object val;
                if (input.TryGetValue(bp.code, out val))
                {
                    newout.Add(bp.code, bp.option.RawTo(val, getProp));
                }
            }
            return newout;
        }




        /// <summary>
        /// 获取显示用设备属性
        /// </summary>
        /// <param name="input"></param>
        /// <param name="validate"></param>
        /// <returns></returns>
        public async Task<List<DeviceProperty>> PropertyList(Dictionary<string, DevicePropertyValue> input, Func<BaseProperty, Task<bool>>? validate = null)
        {
            List<DeviceProperty> list = new List<DeviceProperty>();
            foreach (BaseProperty bp in properties)
            {
                DevicePropertyValue dpv;
                if (input.TryGetValue(bp.code, out dpv))
                {
                    if (validate != null)
                    {
                        if (!await validate.Invoke(bp))
                        {
                            continue;
                        }
                    }

                    DeviceProperty property = new DeviceProperty();
                    property.Code = bp.code;
                    property.Name = bp.name;
                    property.Value = dpv.val;
                    property.Unit = string.Empty;
                    property.Description = bp.description;
                    switch (bp.option.type)
                    {
                        case "int":
                            property.Unit = ((IntOption)bp.option).unit;
                            break;
                        case "float":
                            property.Unit = ((FloatOption)bp.option).unit;
                            break;
                        case "boolean":
                            {
                                bool tmpb = Convert.ToBoolean(dpv.val);
                                if (tmpb)
                                {
                                    property.Value = ((BooleanOption)bp.option).trueText;
                                }
                                else
                                {
                                    property.Value = ((BooleanOption)bp.option).falseText;
                                }
                            }
                            break;
                        case "date":
                            {
                                long tmpl = Convert.ToInt64(dpv.val);
                                if (tmpl == 0)
                                {
                                    property.Value = string.Empty;
                                }
                                else
                                {
                                    var dto = DateTimeOffset.FromUnixTimeMilliseconds(tmpl);
                                    property.Value = dto.LocalDateTime.ToString(((DateOption)bp.option).format);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    property.UpdatedOn = dpv.date;
                    property.OptionType = bp.option.type;

                    list.Add(property);
                }
            }
            return list;
        }
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

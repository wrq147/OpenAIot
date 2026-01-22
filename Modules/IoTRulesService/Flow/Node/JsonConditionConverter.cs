using IoTRulesService.Flow.Node.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Node
{
    /// <summary>
    /// 条件转换
    /// </summary>
    public class JsonConditionConverter : JsonConverter<BaseCondition>
    {
        // 类型映射字典（替代原switch分支，易维护、易扩展）
        private static readonly Dictionary<string, Type> _conditionTypeMap = new(StringComparer.Ordinal)
    {
        { "float", typeof(DoubleCondition) },
        { "int", typeof(LongCondition) },
        { "date", typeof(DateCondition) },
        { "boolean", typeof(BoolCondition) },
        { "enum", typeof(EnumCondition) },
        { "string", typeof(StringCondition) }
    };
        /// <summary>
        /// 反序列化：根据valueType字段 + 字典映射创建对应子类实例并填充数据
        /// </summary>
        public override BaseCondition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            string rawJson = root.GetRawText();

            if (!root.TryGetProperty("valueType", out var typeElement) || typeElement.ValueKind != JsonValueKind.String)
            {
                return null;
            }
            string typeName = typeElement.GetString();
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            if (!_conditionTypeMap.TryGetValue(typeName, out Type targetType))
            {
                return null;
            }

            object nodeInstance = JsonSerializer.Deserialize(rawJson, targetType, options);
            BaseCondition target = nodeInstance as BaseCondition;

            return target;
        }

        /// <summary>
        /// 序列化：按实例的实际子类类型输出完整JSON（修复原WriteValue的缺陷）
        /// </summary>
        public override void Write(Utf8JsonWriter writer, BaseCondition value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}

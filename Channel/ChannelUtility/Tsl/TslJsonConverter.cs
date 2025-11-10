using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChannelUtility.Tsl
{
    public class TslJsonConverter : JsonConverter<BaseValueOption>
    {
        // 类型映射表，避免使用反射，支持AOT
        private static readonly Dictionary<string, Type> _typeMap = new()
        {
            { "int", typeof(IntOption) },
            { "float", typeof(FloatOption) },
            { "boolean", typeof(BooleanOption) },
            { "string", typeof(StringOption) },
            { "enum", typeof(EnumOption) },
            { "date", typeof(DateOption) },
            { "file", typeof(FileOption) },
            { "geo", typeof(GeoOption) }
        };
        private static readonly JsonSerializerOptions _nestedOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonObjectConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        public override BaseValueOption? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 首先将JSON加载到文档中
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            // 获取type字段值
            if (!root.TryGetProperty("type", out var typeElement))
            {
                throw new JsonException("缺少Type字段");
            }
            var typeName = typeElement.GetString();
            // 查找对应的类型
            if (!_typeMap.TryGetValue(typeName, out var type))
            {
                throw new JsonException($"未知的类型: {typeName}");
            }

            // 反序列化为具体类型
            var json = root.GetRawText();
            var result = JsonSerializer.Deserialize(json, type, _nestedOptions);

            return result as BaseValueOption;
        }

        public override void Write(Utf8JsonWriter writer, BaseValueOption value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, _nestedOptions);
        }
    }
}

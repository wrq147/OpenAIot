using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChannelUtility
{
    public class JsonObjectConverter : JsonConverter<object>
    {
        private static readonly JsonSerializerOptions _objectOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonObjectConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    // 优先尝试转换为int
                    if (reader.TryGetInt32(out int intValue))
                    {
                        return intValue;
                    }
                    // 尝试转换为double
                    else if (reader.TryGetDouble(out double doubleValue))
                    {
                        return doubleValue;
                    }
                    // 处理长整数
                    else if (reader.TryGetInt64(out long longValue))
                    {
                        return longValue;
                    }
                    // 处理其他数字类型
                    else
                    {
                        return reader.GetDecimal();
                    }

                case JsonTokenType.String:
                    return reader.GetString();

                case JsonTokenType.True:
                    return true;

                case JsonTokenType.False:
                    return false;

                case JsonTokenType.Null:
                    return null;

                case JsonTokenType.StartObject:
                    // 使用嵌套选项处理对象，避免递归调用当前转换器
                    return JsonSerializer.Deserialize<IDictionary<string, object>>(ref reader, _objectOptions);

                case JsonTokenType.StartArray:
                    // 处理数组
                    var list = new List<object>();
                    reader.Read(); // 移动到数组第一个元素
                    while (reader.TokenType != JsonTokenType.EndArray)
                    {
                        list.Add(Read(ref reader, typeof(object), _objectOptions)!);
                        reader.Read();
                    }
                    return list;

                default:
                    // 处理其他类型
                    throw new JsonException($"不支持的JSON类型：{reader.TokenType}");
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}

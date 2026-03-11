using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChannelUtility
{
    public class JsonObjectConverter : JsonConverter<object>
    {

        private object? ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
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
                    var expando = new ExpandoObject();
                    var expandoDict = (IDictionary<string, object>)expando;

                    reader.Read(); // 跳过StartObject，移动到第一个属性名
                    while (reader.TokenType != JsonTokenType.EndObject)
                    {
                        // 读取属性名
                        if (reader.TokenType != JsonTokenType.PropertyName)
                        {
                            throw new JsonException($"预期属性名，实际为 {reader.TokenType}");
                        }
                        string propName = reader.GetString()!;
                        reader.Read(); // 跳过属性名，移动到属性值

                        // 递归读取属性值（嵌套对象/数组也会走当前逻辑）
                        object? propValue = ReadValue(ref reader, options);
                        expandoDict[propName] = propValue!;

                        reader.Read(); // 移动到下一个属性名/EndObject
                    }
                    return expando;

                case JsonTokenType.StartArray:
                    // 处理数组
                    var list = new List<object>();
                    reader.Read(); // 移动到数组第一个元素
                    while (reader.TokenType != JsonTokenType.EndArray)
                    {
                        list.Add(Read(ref reader, typeof(object), options)!);
                        reader.Read();
                    }
                    return list;

                default:
                    // 处理其他类型
                    throw new JsonException($"不支持的JSON类型：{reader.TokenType}");
            }
        }
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return ReadValue(ref reader, options);
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

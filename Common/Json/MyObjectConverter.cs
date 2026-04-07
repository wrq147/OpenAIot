using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Common.Json
{
    public class MyObjectConverter : JsonConverter<object>
    {
        // 用于处理嵌套对象的选项，避免递归调用当前转换器
        private object? ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                // 数字类型：优先转为int/long/double/decimal
                case JsonTokenType.Number:
                    if (reader.TryGetInt32(out int intValue)) return intValue;
                    if (reader.TryGetInt64(out long longValue)) return longValue;
                    if (reader.TryGetDouble(out double doubleValue)) return doubleValue;
                    return reader.GetDecimal();

                // 基础类型
                case JsonTokenType.String: return reader.GetString();
                case JsonTokenType.True: return true;
                case JsonTokenType.False: return false;
                case JsonTokenType.Null: return null;

                // 核心修改：直接构建ExpandoObject，无字典中转
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

                // 数组：转为List<object>，元素递归处理
                case JsonTokenType.StartArray:
                    var list = new List<object>();
                    reader.Read(); // 跳过StartArray，移动到第一个元素
                    while (reader.TokenType != JsonTokenType.EndArray)
                    {
                        list.Add(ReadValue(ref reader, options)!);
                        reader.Read(); // 移动到下一个元素/EndArray
                    }
                    return list;

                // 其他未处理类型
                default:
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
            if (value is ExpandoObject expandoObj)
            {
                writer.WriteStartObject();
                foreach (var (key, val) in (IDictionary<string, object?>)expandoObj)
                {
                    writer.WritePropertyName(key);
                    JsonSerializer.Serialize(writer, val, options);
                }
                writer.WriteEndObject();
                return;
            }
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}

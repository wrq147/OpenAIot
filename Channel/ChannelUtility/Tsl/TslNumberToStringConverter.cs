using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChannelUtility.Tsl
{
    public class TslNumberToStringConverter : JsonConverter<string>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(string);
        }
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 处理数值类型的值
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out int intValue))
                {
                    return intValue.ToString();
                }
                else if (reader.TryGetDouble(out double doubleValue))
                {
                    return doubleValue.ToString();
                }
                else if (reader.TryGetInt64(out long longValue))
                {
                    return longValue.ToString();
                }
                else
                {
                    return reader.GetDecimal().ToString();
                }
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
            else
            {
                throw new JsonException($"不支持的键类型：{reader.TokenType}");
            }
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}

using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ChannelUtility.Tsl
{
    public class TslStringToByteConverter: JsonConverter<byte>
    {
        public override byte Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 处理字符串类型的值
            if (reader.TokenType == JsonTokenType.String)
            {
                string value = reader.GetString();
                if (string.IsNullOrWhiteSpace(value))
                {
                    // 处理空字符串，返回默认值
                    return default;
                }

                try
                {
                    // 尝试将字符串转换为目标数值类型
                    return byte.Parse(value);
                }
                catch (Exception ex)
                {
                    throw new JsonException($"无法将字符串 '{value}' 转换为 {typeof(byte)} 类型", ex);
                }
            }
            // 直接处理数值类型
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetByte();
            }

            // 如果无法转换，抛出异常
            throw new JsonException($"无法将JSON值转换为 {typeof(byte)} 类型");
        }

        public override void Write(Utf8JsonWriter writer, byte value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}

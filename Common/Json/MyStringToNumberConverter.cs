using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Json
{
    public class MyStringToNumberConverter : JsonConverter<object>
    {
        // 定义支持的数值类型列表
        private readonly HashSet<Type> _supportedUnderlyingTypes = new HashSet<Type>
    {
        typeof(int), typeof(long), typeof(double), typeof(decimal),
        typeof(float), typeof(short), typeof(byte), typeof(uint),
        typeof(ulong), typeof(ushort), typeof(sbyte)
    };

        // 判断是否是可空类型
        private bool IsNullableType(Type typeToConvert) => Nullable.GetUnderlyingType(typeToConvert) != null;

        // 获取类型的基础类型（int? → int，long → long）
        private Type GetUnderlyingType(Type typeToConvert) => Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;

        // 关键：识别所有支持的数值类型（含可空版本）
        public override bool CanConvert(Type typeToConvert)
        {
            Type underlyingType = GetUnderlyingType(typeToConvert);
            return _supportedUnderlyingTypes.Contains(underlyingType);
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            bool isNullable = IsNullableType(typeToConvert);
            Type underlyingType = GetUnderlyingType(typeToConvert);

            // 处理null值（仅可空类型允许）
            if (reader.TokenType == JsonTokenType.Null)
            {
                if (isNullable)
                {
                    return null!;
                }
                throw new JsonException($"非可空类型 {typeToConvert.Name} 不支持null值");
            }

            string rawValue = null!;
            // 处理字符串类型（如"123"、"3.14"）
            if (reader.TokenType == JsonTokenType.String)
            {
                rawValue = reader.GetString()!;
                if (string.IsNullOrWhiteSpace(rawValue))
                {
                    if (isNullable)
                    {
                        return null!;
                    }
                    throw new JsonException($"无法将空字符串转换为非可空类型 {underlyingType.Name}");
                }
            }
            // 处理数值类型（直接读取为字符串，统一转换逻辑）
            else if (reader.TokenType == JsonTokenType.Number)
            {
                object value = underlyingType switch
                {
                    Type t when t == typeof(int) => reader.GetInt32(),
                    Type t when t == typeof(long) => reader.GetInt64(),
                    Type t when t == typeof(double) => reader.GetDouble(),
                    Type t when t == typeof(decimal) => reader.GetDecimal(),
                    Type t when t == typeof(float) => reader.GetSingle(),
                    Type t when t == typeof(short) => reader.GetInt16(),
                    Type t when t == typeof(byte) => reader.GetByte(),
                    Type t when t == typeof(uint) => reader.GetUInt32(),
                    Type t when t == typeof(ulong) => reader.GetUInt64(),
                    Type t when t == typeof(ushort) => reader.GetUInt16(),
                    Type t when t == typeof(sbyte) => reader.GetSByte(),
                    _ => throw new JsonException($"不支持的数值类型 {underlyingType.Name}")
                };
                return value;
            }
            else
            {
                throw new JsonException($"不支持将 {reader.TokenType} 类型转换为 {typeToConvert.Name}");
            }

            try
            {
                object convertedValue = Convert.ChangeType(rawValue, underlyingType);
                return isNullable ? convertedValue : convertedValue;
            }
            catch (Exception ex)
            {
                throw new JsonException($"字符串 '{rawValue}' 转换为 {underlyingType.Name} 失败", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            // 处理可空类型的null值
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            // 统一按数值类型写入
            Type valueType = value.GetType();
            switch (value)
            {
                case int intVal:
                    writer.WriteNumberValue(intVal);
                    break;
                case long longVal:
                    writer.WriteNumberValue(longVal);
                    break;
                case decimal decVal:
                    writer.WriteNumberValue(decVal);
                    break;
                case double dblVal:
                    writer.WriteNumberValue(dblVal);
                    break;
                case float fltVal:
                    writer.WriteNumberValue(fltVal);
                    break;
                case short shortVal:
                    writer.WriteNumberValue(shortVal);
                    break;
                case byte byteVal:
                    writer.WriteNumberValue(byteVal);
                    break;
                case uint uintVal:
                    writer.WriteNumberValue(uintVal);
                    break;
                case ulong ulongVal:
                    writer.WriteNumberValue(ulongVal);
                    break;
                case ushort ushortVal:
                    writer.WriteNumberValue(ushortVal);
                    break;
                case sbyte sbyteVal:
                    writer.WriteNumberValue(sbyteVal);
                    break;
                default:
                    // 兜底转换为double
                    writer.WriteNumberValue(Convert.ToDouble(value));
                    break;
            }
        }

        public override bool HandleNull => true;
    }
}

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyNetQ;

/// <summary>
///     JsonSerializer based on System.Text.Json which uses it dynamically
/// </summary>
public class JsonSerializer : ISerializer
{
    private static readonly Encoding Encoding = new UTF8Encoding(false);
    private const int DefaultBufferSize = 1024;
    private readonly JsonSerializerOptions serializerOptions;

    /// <summary>
    ///     Creates SystemTextJsonSerializer
    /// </summary>
    /// <param name="options">The options to control serialization behavior, or null to use default options</param>
    public JsonSerializer(JsonSerializerOptions options = null)
    {
        // 设置默认序列化选项
        serializerOptions = options ?? new JsonSerializerOptions
        {
            WriteIndented = false,
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonObjectConverter() }
        };
    }

    /// <inheritdoc />
    public IMemoryOwner<byte> MessageToBytes(Type messageType, object message)
    {
        Preconditions.CheckNotNull(messageType, nameof(messageType));

        var buffer = new ArrayBufferWriter<byte>();

        using (var writer = new Utf8JsonWriter(buffer, new JsonWriterOptions
        {
            Indented = serializerOptions.WriteIndented,
            Encoder = serializerOptions.Encoder
        }))
        {
            // 序列化对象，包含类型信息以便反序列化
            System.Text.Json.JsonSerializer.Serialize(writer, message, messageType, serializerOptions);
        }

        return new ArrayMemoryOwner(buffer.WrittenMemory);
    }

    /// <inheritdoc />
    public object BytesToMessage(Type messageType, in ReadOnlyMemory<byte> bytes)
    {
        Preconditions.CheckNotNull(messageType, nameof(messageType));

        return System.Text.Json.JsonSerializer.Deserialize(bytes.Span, messageType, serializerOptions);
    }

    // 自定义内存所有者实现
    private class ArrayMemoryOwner : IMemoryOwner<byte>
    {
        private readonly ReadOnlyMemory<byte> _memory;

        // 修正：接受ReadOnlyMemory<byte>参数
        public ArrayMemoryOwner(ReadOnlyMemory<byte> memory)
        {
            _memory = memory;
        }

        // 实现IMemoryOwner<byte>的Memory属性
        public Memory<byte> Memory => _memory.ToArray(); // 转换为可写内存

        public void Dispose()
        {
            // 清理资源
        }
    }


    // 处理多态类型的转换器
    private class JsonObjectConverter : JsonConverter<object>
    {
        // 用于处理嵌套对象的选项，避免递归调用当前转换器
        private static readonly JsonSerializerOptions _nestedOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
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
                    return System.Text.Json.JsonSerializer.Deserialize<IDictionary<string, object>>(ref reader, options);

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
                    return System.Text.Json.JsonSerializer.Deserialize(ref reader, typeToConvert, _nestedOptions);
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            System.Text.Json.JsonSerializer.Serialize(writer, value, _nestedOptions);
        }
    }
}

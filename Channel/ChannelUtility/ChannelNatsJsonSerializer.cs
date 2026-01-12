using NATS.Client.Core;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChannelUtility
{
    public class ChannelNatsJsonSerializer<T> : INatsSerializer<T>
    {
        public static readonly INatsSerializer<T> Default = new ChannelNatsJsonSerializer<T>();
        private readonly INatsSerializer<T>? _next;

        public ChannelNatsJsonSerializer(INatsSerializer<T>? next = default)
        {
            _next = next;
        }
        public INatsSerializer<T> CombineWith(INatsSerializer<T> next) => new ChannelNatsJsonSerializer<T>(next);

        public void Serialize(IBufferWriter<byte> bufferWriter, T value)
        {
            if (value is string str)
            {
                Encoding.UTF8.GetBytes(str, bufferWriter);
                return;
            }

            Utf8JsonWriter writer = new Utf8JsonWriter(bufferWriter);
            JsonSerializer.Serialize(writer, value, JsonMessageSerializerConfig.ObjectOptions);
        }

        /// <inheritdoc />
        public T? Deserialize(in ReadOnlySequence<byte> buffer)
        {
            if (buffer.Length == 0)
            {
                return default;
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)Encoding.UTF8.GetString(buffer);
            }

            var reader = new Utf8JsonReader(buffer);
            return JsonSerializer.Deserialize<T>(ref reader, JsonMessageSerializerConfig.ObjectOptions);
        }
    }
}

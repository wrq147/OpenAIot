using Common.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Common.Json
{
    public class SerializationControlContractResolver : DefaultJsonTypeInfoResolver
    {
        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

            foreach (JsonPropertyInfo property in jsonTypeInfo.Properties)
            {
                // 检查是否标记了 OnlyDeserialize（仅反序列化）
                bool hasOnlyDeserialize = property.AttributeProvider.IsDefined(
                    typeof(OnlyDeserializeAttribute), inherit: false);

                // 检查是否标记了 OnlySerialize（仅序列化）
                bool hasOnlySerialize = property.AttributeProvider.IsDefined(
                    typeof(OnlySeriaizeAttribute), inherit: false);

                // 处理「仅反序列化」：序列化时跳过，反序列化正常
                if (hasOnlyDeserialize)
                {
                    property.ShouldSerialize = (obj, value) => false;
                }

                // 处理「仅序列化」：反序列化时禁止赋值，序列化正常
                if (hasOnlySerialize)
                {
                    // 核心：将 Set 方法置为 null，反序列化时无法给该属性赋值
                    property.Set = null;
                }
            }

            return jsonTypeInfo;
        }
    }
}

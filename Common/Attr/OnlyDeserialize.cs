using Newtonsoft.Json;
using System;

namespace Common.Attr
{
    /// <summary>
    /// 只反串行化(只能添加修改，不会输出给用户）
    /// </summary>
    public class OnlyDeserialize : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
        public override bool CanWrite => false;
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader,objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // 不允许序列化，抛出异常
            throw new NotSupportedException("序列化操作不被允许。");
        }
    }
}

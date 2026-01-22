using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace AuthService.Fields
{
    public class JsonFieldConverter : JsonConverter<FieldBase>
    {
        private static readonly Dictionary<string, Type> _typeMap = new()
        {
            { "附件", typeof(AttachField) },
            { "超链接", typeof(HyperlinkField) },
            { "复选框", typeof(CheckBoxField) },
            { "图片", typeof(ImageField) },
            { "数字", typeof(NumberField) },
            { "关联对象", typeof(ObjectField) },
            { "单选框", typeof(RadioField) },
            { "文本", typeof(TextField) },
            { "时间", typeof(TimeField) }
        };
        /// <summary>
        /// 反序列化：根据type字段创建对应子类实例并填充数据
        /// </summary>
        public override FieldBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 首先将JSON加载到文档中
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            // 获取type字段值
            if (!root.TryGetProperty("type", out var typeElement))
            {
                throw new JsonException("缺少Type字段");
            }
            var typeName = typeElement.GetString();
            // 查找对应的类型
            if (!_typeMap.TryGetValue(typeName, out var type))
            {
                throw new JsonException($"未知的类型: {typeName}");
            }

            // 反序列化为具体类型
            var json = root.GetRawText();
            var result = JsonSerializer.Deserialize(json, type, options);

            return result as FieldBase;
        }

        /// <summary>
        /// 序列化：按实例的实际子类类型输出完整JSON（修复原WriteValue的缺陷）
        /// </summary>
        public override void Write(Utf8JsonWriter writer, FieldBase value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
   
}

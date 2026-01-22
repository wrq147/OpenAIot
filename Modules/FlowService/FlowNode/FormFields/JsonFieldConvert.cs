using Common.Json;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlowService.FlowNode.FormFields
{
    public class JsonFieldConvert : JsonConverter<FormField>
    {
        // 类型映射表，避免使用反射，支持AOT
        private static readonly Dictionary<string, Type> _typeMap = new()
        {
            { "TextInput", typeof(InputProps) },
            { "TextareaInput", typeof(InputProps) },
            { "AmountInput", typeof(AmountInputProps) },
            { "DateTime", typeof(DateTimeProps) },
            { "DateTimeRange", typeof(DateTimeRangeProps) },
            { "DeptPicker", typeof(DeptPickerProps) },
            { "Description", typeof(DescriptionProps) },
            { "FileUpload", typeof(FileUploadProps) },
            { "ImageUpload", typeof(ImageUploadProps) },
            { "Location", typeof(LocationProps) },
            { "MultipleSelect", typeof(MultipleSelectProps) },
            { "NumberInput", typeof(NumberInputProps) },
            { "SelectInput", typeof(SelectInputProps) },
            { "SignPannel", typeof(SignPannelProps) },
            { "SpanLayout", typeof(SpanLayoutProps) },
            { "TableList", typeof(TableListProps) },
            { "UserPicker", typeof(UserPickerProps) },
            { "DevicPicker", typeof(DeviceProps) },
            { "ParamInput", typeof(ParamInputProps) }
        };
        private static readonly JsonSerializerOptions _nestedOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new MyObjectConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        public override FormField Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var formField = new FormField
            {
                id = root.TryGetProperty("id", out var idElem) ? idElem.GetString() : null,
                title = root.TryGetProperty("title", out var titleElem) ? titleElem.GetString() : null,
                name = root.TryGetProperty("name", out var nameElem) ? nameElem.GetString() : null,
                icon = root.TryGetProperty("icon", out var iconElem) ? iconElem.GetString() : null,
                valueType = root.TryGetProperty("valueType", out var valueTypeElem) ? valueTypeElem.GetString() : null
            };

            if (string.IsNullOrEmpty(formField.name))
            {
                throw new JsonException("FormField的name属性不能为空");
            }

            if (root.TryGetProperty("props", out var propsElem) && propsElem.ValueKind != JsonValueKind.Null)
            {
                if (!_typeMap.TryGetValue(formField.name, out var propsType))
                {
                    throw new JsonException($"未知的name：{formField.name}，无对应props类型");
                }
                var props = JsonSerializer.Deserialize(propsElem.GetRawText(), propsType, options);

                formField.props = props as BaseProps ?? throw new JsonException("props类型转换失败");
            }

            return formField;
        }

        public override void Write(Utf8JsonWriter writer, FormField value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), _nestedOptions);
        }
    }
   
}

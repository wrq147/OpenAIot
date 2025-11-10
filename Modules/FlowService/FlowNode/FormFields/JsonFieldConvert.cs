using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace FlowService.FlowNode.FormFields
{
    /// <summary>
    /// 表单字段转换
    /// </summary>
    public class JsonFieldConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FormField);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(FormField))
            {
                var jsonObject = JObject.Load(reader);
                FormField target = new FormField();
                BaseProps props = null;
                JToken clsType;
                if (jsonObject.TryGetValue("name", out clsType))
                {
                    switch (clsType.ToString())
                    {
                        case "TextInput":
                            props = new InputProps();
                            break;
                        case "TextareaInput":
                            props = new InputProps();
                            break;
                        case "AmountInput":
                            props = new AmountInputProps();
                            break;
                        case "DateTime":
                            props = new DateTimeProps();
                            break;
                        case "DateTimeRange":
                            props = new DateTimeRangeProps();
                            break;
                        case "DeptPicker":
                            props = new DeptPickerProps();
                            break;
                        case "Description":
                            props = new DescriptionProps();
                            break;
                        case "FileUpload":
                            props = new FileUploadProps();
                            break;
                        case "ImageUpload":
                            props = new ImageUploadProps();
                            break;
                        case "Location":
                            props = new LocationProps();
                            break;
                        case "MultipleSelect":
                            props = new MultipleSelectProps();
                            break;
                        case "NumberInput":
                            props = new NumberInputProps();
                            break;
                        case "SelectInput":
                            props = new SelectInputProps();
                            break;
                        case "SignPannel":
                            props = new SignPannelProps();
                            break;
                        case "SpanLayout":
                            props = new SpanLayoutProps();
                            break;
                        case "TableList":
                            props = new TableListProps();
                            break;
                        case "UserPicker":
                            props = new UserPickerProps();
                            break;
                        case "DevicPicker":
                            props = new DeviceProps();
                            break;
                        case "ParamInput":
                            props = new ParamInputProps();
                            break;
                    }
                }
                target.props = props;
                serializer.Populate(jsonObject.CreateReader(), target);
                return target;
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}

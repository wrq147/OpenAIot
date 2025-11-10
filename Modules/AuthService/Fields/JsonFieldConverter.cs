using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;


namespace AuthService.Fields
{
    public class JsonFieldConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(FieldBase).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            object target = null;
            JToken clsType;
            if (jsonObject.TryGetValue("type", out clsType))
            {
                switch (clsType.ToString())
                {
                    case "附件":
                        target = new AttachField();
                        break;
                    case "复选框":
                        target = new CheckBoxField();
                        break;
                    case "超链接":
                        target = new HyperlinkField();
                        break;
                    case "图片":
                        target = new ImageField();
                        break;
                    case "数字":
                        target = new NumberField();
                        break;
                    case "关联对象":
                        target = new ObjectField();
                        break;
                    case "单选框":
                        target = new RadioField();
                        break;
                    case "文本":
                        target = new TextField();
                        break;
                    case "时间":
                        target = new TimeField();
                        break;

                }
            }
            if (target == null)
            {
                return target;
            }
            serializer.Populate(jsonObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}

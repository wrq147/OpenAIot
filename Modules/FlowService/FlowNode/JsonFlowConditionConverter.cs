using FlowService.FlowNode.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace FlowService.FlowNode
{
    /// <summary>
    /// 条件转换
    /// </summary>
    public class JsonFlowConditionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(BaseCondition).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            object target = null;
            JToken clsType;
            if (jsonObject.TryGetValue("valueType", out clsType))
            {
                switch (clsType.ToString())
                {
                    case "Number":
                        target = new NumberCondition();
                        break;
                    case "Date":
                        target = new DateCondition();
                        break;
                    case "User":
                        target = new UserCondition();
                        break;
                    case "Dept":
                        target = new DeptCondition();
                        break;
                    case "String":
                        target = new StringCondition();
                        break;
                    case "Boolean":
                        target = new BoolCondition();
                        break;
                }
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

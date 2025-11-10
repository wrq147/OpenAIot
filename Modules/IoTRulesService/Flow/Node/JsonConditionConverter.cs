using IoTRulesService.Flow.Node.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Node
{
    /// <summary>
    /// 条件转换
    /// </summary>
    public class JsonConditionConverter : JsonConverter
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
                    case "float":
                        target = new DoubleCondition();
                        break;
                    case "int":
                        target = new LongCondition();
                        break;
                    case "date":
                        target = new DateCondition();
                        break;
                    case "boolean":
                        target = new BoolCondition();
                        break;
                    case "enum":
                        target = new EnumCondition();
                        break;
                    case "string":
                        target = new StringCondition();
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

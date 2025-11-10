using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace IoTRulesService.Flow.Node
{
    public class JsonNodeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(RuleBaseNode).IsAssignableFrom(objectType);
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
                    case "ROOT":
                        target = new RootNode();
                        break;
                    case "EMPTY":
                        target = new EmptyNode();
                        break;
                    case "CONDITIONS":
                        target = new ConditionGroupNode();
                        break;
                    case "CONDITION":
                        target = new ConditionNode();
                        break;
                    case "CONCURRENTS":
                        target = new ConcurrentGroupNode();
                        break;
                    case "CONCURRENT":
                        target = new ConcurrentNode();
                        break;
                    case "DELAY":
                        target = new DelayNode();
                        break;
                    case "DATAWRITE":
                        target = new DataWriteNode();
                        break;
                    case "CONVERSION":
                        target = new ConvertNode();
                        break;
                    case "TRIGGER":
                        target = new HttpNode();
                        break;
                    case "FUNC":
                        target = new FuncNode();
                        break;
                    case "WARN":
                        target = new WarnNode();
                        break;
                    case "METRONOME":
                        target = new CountNode();
                        break;
                    case "EXCEPT":
                        target = new ExceptNode();
                        break;
                    case "CLEARDELTA":
                        target = new ClearDeltaNode();
                        break;
                    case "TIMESCHEDULER":
                        target = new TimeSchedulerNode();
                        break;
                    case "TAG":
                        target = new TagNode();
                        break;
                    case "REDIRECT":
                        target = new RedirectNode();
                        break;
                    case "PID":
                        target = new PIDNode();
                        break;
                    case "SETPROP":
                        target = new SetPropNode();
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

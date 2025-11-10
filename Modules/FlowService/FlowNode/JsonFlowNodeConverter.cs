using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace FlowService.FlowNode
{
    /// <summary>
    /// 工作流节点转换
    /// </summary>
    public class JsonFlowNodeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(FlowBaseNode).IsAssignableFrom(objectType);
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
                    case "APPROVAL":
                        target = new SPNode();
                        break;
                    case "USER":
                        target = new OperatorNode();
                        break;
                    case "CONDITIONS":
                        target = new ConditionGroupNode();
                        break;
                    case "CONDITION":
                        target = new ConditionNode();
                        break;
                    case "CC":
                        target = new CSNode();
                        break;
                    case "EMPTY":
                        target = new EmptyNode();
                        break;
                    case "TRIGGER":
                        target = new TriggerNode();
                        break;
                    case "DELAY":
                        target = new DelayNode();
                        break;
                    case "CONCURRENTS":
                        target = new ConcurrentGroupNode();
                        break;
                    case "CONCURRENT":
                        target = new ConcurrentNode();
                        break;
                    case "DATAXE":
                        target = new DataNode();
                        break;
                    case "FUNC":
                        target = new FuncNode();
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

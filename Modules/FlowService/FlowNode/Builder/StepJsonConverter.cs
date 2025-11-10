using FlowService.FlowNode.Builder.Step;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TemplateAction.Core;

namespace FlowService.FlowNode.Builder
{
    /// <summary>
    /// 节点json转换
    /// </summary>
    public class StepJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(WorkflowStep).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            JToken typestr; 
            jsonObject.TryGetValue("Type", out typestr); 
            object target = Activator.CreateInstance(Type.GetType(typestr.ToString()));
            serializer.Populate(jsonObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}

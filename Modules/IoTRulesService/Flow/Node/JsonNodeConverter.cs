using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IoTRulesService.Flow.Node
{
 

    public class JsonNodeConverter : JsonConverter<RuleBaseNode>
    {
        // 类型映射字典（替代原switch分支，更易扩展和维护）
        private static readonly Dictionary<string, Type> _nodeTypeMap = new(StringComparer.Ordinal)
{
    { "ROOT", typeof(RootNode) },
    { "EMPTY", typeof(EmptyNode) },
    { "CONDITIONS", typeof(ConditionGroupNode) },
    { "CONDITION", typeof(ConditionNode) },
    { "CONCURRENTS", typeof(ConcurrentGroupNode) },
    { "CONCURRENT", typeof(ConcurrentNode) },
    { "DELAY", typeof(DelayNode) },
    { "DATAWRITE", typeof(DataWriteNode) },
    { "CONVERSION", typeof(ConvertNode) },
    { "TRIGGER", typeof(HttpNode) },
    { "FUNC", typeof(FuncNode) },
    { "WARN", typeof(WarnNode) },
    { "NOTICE", typeof(NoticeNode) },
    { "METRONOME", typeof(CountNode) },
    { "EXCEPT", typeof(ExceptNode) },
    { "CLEARDELTA", typeof(ClearDeltaNode) },
    { "TIMESCHEDULER", typeof(TimeSchedulerNode) },
    { "TAG", typeof(TagNode) },
    { "REDIRECT", typeof(RedirectNode) },
    { "PID", typeof(PIDNode) },
    { "SETPROP", typeof(SetPropNode) }
};
        /// <summary>
        /// 反序列化：根据type字段 + 字典映射创建对应子类实例并填充数据
        /// </summary>
        public override RuleBaseNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            string rawJson = root.GetRawText();

            if (!root.TryGetProperty("type", out var typeElement) || typeElement.ValueKind != JsonValueKind.String)
            {
                return null;
            }
            string typeName = typeElement.GetString();
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            if (!_nodeTypeMap.TryGetValue(typeName, out Type targetType))
            {
                return null;
            }

            object nodeInstance = JsonSerializer.Deserialize(rawJson, targetType, options);
            RuleBaseNode target = nodeInstance as RuleBaseNode;

            return target;
        }

        /// <summary>
        /// 序列化：按实例的实际子类类型输出完整JSON（修复原WriteValue的缺陷）
        /// </summary>
        public override void Write(Utf8JsonWriter writer, RuleBaseNode value, JsonSerializerOptions options)
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

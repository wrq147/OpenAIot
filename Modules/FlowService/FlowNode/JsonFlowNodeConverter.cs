using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlowService.FlowNode
{
    /// <summary>
    /// 工作流节点转换
    /// </summary>
    public class JsonFlowNodeConverter : JsonConverter<FlowBaseNode>
    {
        // 类型映射字典（替代原switch分支，更易扩展和维护）
        private static readonly Dictionary<string, Type> _nodeTypeMap = new(StringComparer.Ordinal)
    {
        { "ROOT", typeof(RootNode) },
        { "APPROVAL", typeof(SPNode) },
        { "USER", typeof(OperatorNode) },
        { "CONDITIONS", typeof(ConditionGroupNode) },
        { "CONDITION", typeof(ConditionNode) },
        { "CC", typeof(CSNode) },
        { "EMPTY", typeof(EmptyNode) },
        { "TRIGGER", typeof(TriggerNode) },
        { "DELAY", typeof(DelayNode) },
        { "CONCURRENTS", typeof(ConcurrentGroupNode) },
        { "CONCURRENT", typeof(ConcurrentNode) },
        { "DATAXE", typeof(DataNode) },
        { "FUNC", typeof(FuncNode) }
    };

        /// <summary>
        /// 反序列化：根据type字段 + 字典映射创建对应子类实例并填充数据
        /// </summary>
        public override FlowBaseNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            FlowBaseNode target = nodeInstance as FlowBaseNode;

            return target;
        }

        /// <summary>
        /// 序列化：按实例的实际子类类型输出完整JSON（修复原WriteValue的缺陷）
        /// </summary>
        public override void Write(Utf8JsonWriter writer, FlowBaseNode value, JsonSerializerOptions options)
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

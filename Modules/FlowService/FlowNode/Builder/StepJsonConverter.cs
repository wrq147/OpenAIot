using FlowService.FlowNode.Builder.Step;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TemplateAction.Core;

namespace FlowService.FlowNode.Builder
{
    /// <summary>
    /// 节点json转换
    /// </summary>
    public class StepJsonConverter : JsonConverter<WorkflowStep>
    {
        /// <summary>
        /// 反序列化：根据Type字段创建对应子类实例并填充数据
        /// </summary>
        public override WorkflowStep Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 加载JSON文档，保留完整的步骤数据
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            string rawJson = root.GetRawText();

            // 获取Type字段值（核心：用于确定具体子类）
            if (!root.TryGetProperty("Type", out var typeElement) || typeElement.ValueKind != JsonValueKind.String)
            {
                throw new JsonException("WorkflowStep缺少必填的Type字段，或Type字段不是字符串类型");
            }
            string typeName = typeElement.GetString();
            if (string.IsNullOrEmpty(typeName))
            {
                throw new JsonException("WorkflowStep的Type字段值为空");
            }

            // 根据Type名称获取具体子类类型（注意：Type.GetType需要完整类型名，如"命名空间.ApprovalStep"）
            Type targetType = Type.GetType(typeName);
            if (targetType == null)
            {
                throw new JsonException($"无法找到类型：{typeName}，请检查Type字段值或程序集引用");
            }


            // 反序列化为具体子类实例
            object stepInstance = JsonSerializer.Deserialize(rawJson, targetType, options);

            // 转换为WorkflowStep并返回
            return stepInstance as WorkflowStep ??
                   throw new JsonException($"类型{typeName}转换为WorkflowStep失败");
        }

        /// <summary>
        /// 序列化：按实例的实际子类类型输出完整JSON
        /// </summary>
        public override void Write(Utf8JsonWriter writer, WorkflowStep value, JsonSerializerOptions options)
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

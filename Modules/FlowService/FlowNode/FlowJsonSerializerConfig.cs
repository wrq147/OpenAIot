using Common.Json;
using FlowService.FlowNode.Builder;
using FlowService.FlowNode.FormFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlowService.FlowNode
{
    public static class FlowJsonSerializerConfig
    {
        public static readonly JsonSerializerOptions FieldOptions = new JsonSerializerOptions
        {
            Converters = { new JsonFieldConvert(), new MyStringToNumberConverter(), new MyNumberToStringConverter(), new MyObjectConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        public static readonly JsonSerializerOptions StepOptions = new JsonSerializerOptions
        {
            Converters = { new StepJsonConverter(), new MyStringToNumberConverter(), new MyNumberToStringConverter(), new MyObjectConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        public static readonly JsonSerializerOptions NodeOptions = new JsonSerializerOptions
        {
            Converters = { new JsonFlowNodeConverter(), new JsonFlowConditionConverter(), new MyStringToNumberConverter(), new MyNumberToStringConverter(), new MyObjectConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}

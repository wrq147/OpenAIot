using Common.Json;
using IoTRulesService.Flow.Builder;
using IoTRulesService.Flow.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace IoTRulesService
{
    public static class RuleJsonConfig
    {
        public static readonly JsonSerializerOptions StepOptions = new JsonSerializerOptions
        {
            Converters = { new StepJsonConverter(), new MyStringToNumberConverter(), new MyNumberToStringConverter(), new MyObjectConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        public static readonly JsonSerializerOptions NodeOptions = new JsonSerializerOptions
        {
            Converters = { new JsonNodeConverter(), new JsonConditionConverter(), new MyStringToNumberConverter(), new MyNumberToStringConverter(), new MyObjectConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}

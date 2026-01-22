using Common.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthService.Fields
{
    public static class FieldJsonSerializerConfig
    {
        public static readonly JsonSerializerOptions FieldOptions = new JsonSerializerOptions
        {
            Converters = { new JsonFieldConverter(), new MyStringToNumberConverter(), new MyNumberToStringConverter(), new MyObjectConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}

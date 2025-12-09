using System.Text.Encodings.Web;
using System.Text.Json;


namespace Common.Json
{
    public class MyDefaultTextJsonConfig
    {
        public static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            Converters = { new MyObjectConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}

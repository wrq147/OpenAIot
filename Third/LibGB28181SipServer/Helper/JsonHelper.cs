using GB28181SipGate.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace GB28181SipGate.Helper
{
    public class JsonHelper
    {
        public static string ToJson<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, DefaultTextJsonConfig.DefaultOptions);
        }
        public static T ToObject<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}

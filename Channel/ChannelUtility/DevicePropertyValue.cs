using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility
{
    public class DevicePropertyValue
    {
        public object val { get; set; }
        public DateTime date { get; set; }
        public static Dictionary<string, DevicePropertyValue> FromDict(IDictionary<string, object> data, DateTime time)
        {
            Dictionary<string, DevicePropertyValue> rt = new Dictionary<string, DevicePropertyValue>();
            foreach (var kvp in data)
            {
                rt.Add(kvp.Key, new DevicePropertyValue()
                {
                    val = kvp.Value,
                    date = time
                });
            }
            return rt;
        }
        public static Dictionary<string, object> ToDict(IDictionary<string, DevicePropertyValue> data)
        {
            Dictionary<string, object> rt = new Dictionary<string, object>();
            foreach (var kvp in data)
            {
                rt.Add(kvp.Key, kvp.Value.val);
            }
            return rt;
        }
        public static Dictionary<string, DevicePropertyValue> FromDictStr(IDictionary<string, string> data)
        {
            var dict = new Dictionary<string, DevicePropertyValue>();
            foreach (var kvp in data)
            {
                if (kvp.Key.StartsWith("$"))
                {
                    dict.Add(kvp.Key, new DevicePropertyValue()
                    {
                        val = kvp.Value,
                        date = DateTime.Now
                    });
                }
                else
                {
                    dict.Add(kvp.Key, System.Text.Json.JsonSerializer.Deserialize<DevicePropertyValue>(kvp.Value, JsonMessageSerializerConfig.ObjectOptions));
                }
            }
            return dict;
        }
    }
}
